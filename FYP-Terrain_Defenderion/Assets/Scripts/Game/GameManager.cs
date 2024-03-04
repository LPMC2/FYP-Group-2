using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    public GManager gManager;
    public static GameManager Singleton;
    [SerializeField] private LoadFunction[] loadState;
    [SerializeField] private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>();
    private float originMoveSpeed = default;
    //[SerializeField]
    //private NetworkManager m_networkManager = default;
    [SerializeField]
    private GameObject m_UIObject;
    //[SerializeField]
    //private NetworkSO networkData = default;
    [SerializeField]
    private Vector3 startPosition = default;
    [SerializeField]
    private float delayTeleportTime = 1f;
    [SerializeField]
    private GameObject player = default;
    //DynamicMoveProvider dynamicMove = default;
    //public static NetworkObjectManager objectSpawner = default;
    public List<GameObject> currentPlayers = new List<GameObject>();
    [SerializeField]
    private TMP_Text ipAddress = default;
    [SerializeField] private TMP_Text actionBar;
    bool gameStarted = false;
    private bool isEnded = false;
    [SerializeField]
    private UnityEvent m_EnterLobbyEvent;
    [SerializeField]
    private UnityEvent m_EnterGameEvent;
    [SerializeField]
    private UnityEvent m_Custom_EndGameEvent;
    [Header("Display Settings")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private DisplayManager m_DisplayManager;
    [SerializeField] private TimeManager m_TimeManager;
    public GameObject HealthBarPrefab { get { return healthBarPrefab; } }
    public void SetIpAddressText(string address)
    {
        ipAddress.text = "IP: " + address;
    }
    //public void SetPlayers( )
    //{
    //    currentPlayers.Clear();
    //    GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
    //    foreach(GameObject targetGameObject in gameObjects)
    //    {
    //        if(targetGameObject.GetComponent<NetworkPlayer>() != null)
    //        {
    //            currentPlayers.Add(targetGameObject);
    //        }
    //    }
    //    NetworkObjectManager.Singleton.UpdateManager(this);
    //}
    // Start is called before the first frame update
    //public NetworkVariable<int> PlayerID { get {return playerId; }}
    //public void AddPlayerId() 
    //{
    //    playerId.Value++;
    //}
    public void SetEnableTextFalse()
    {
        actionBar.enabled = false;
    }
    public void SetEnableTextTrue()
    {
        actionBar.enabled = true;
    }
    //private NetworkVariable<int> playerId =  new NetworkVariable<int>();
    private float initialLeftRaycastDis = 0f;
    private float initialRightRaycastDis = 0f;
    void Start()
    {
        isEnded = false;
        gameStarted = false;

        gameState.Value = GameState.UI;
        m_EnterLobbyEvent.Invoke();
       
    }
    private void Awake()
    {
        gManager.StageLevel.Value = 0;
        Singleton = this;
        
    }
    public void StartGame()
    {
        
     
        StartCoroutine(DelayTeleport(startPosition));
        //player.transform.position = startPosition;
        AddStageLevel();
        gameStarted = true;
        gameState.Value = GameState.Default;
        if (m_UIObject != null)
        {
            Animator animator = m_UIObject.GetComponent<Animator>();
            animator.Play("FadeIn");
        }
        m_EnterGameEvent.Invoke();

    }
    public IEnumerator DelayTeleport(Vector3 pos)
    {

        yield return new WaitForSeconds(delayTeleportTime);
        player.transform.position = pos;

    }

    public void AddStageLevel()
    {
        gManager.StageLevel.Value++;
        if (gManager.StartStageEvent.Length > -1)
            InvokeUnityEvent(gManager.StartStageEvent[gManager.StageLevel.Value]);

    }

    public IEnumerator EndGame()
    {
        Animator animator = null;
        if (m_UIObject != null)
        {
            animator = m_UIObject.GetComponent<Animator>();
            animator.Play("GameEnd");
        }
        m_TimeManager.Reset();
        TMP_Text initialText = m_TimeManager.DisplayText;
        m_TimeManager.SetDisplayTMP_Text(m_DisplayManager.GetDisplay("Subtitle").DisplayController.TMP_DisplayText);
        yield return new WaitForSeconds(0.1f);

        m_TimeManager.ActiveReturnTimer();
        gameStarted = false;
        yield return new WaitForSeconds(m_TimeManager.ReturnTime);
        m_TimeManager.SetDisplayTMP_Text(initialText);
        if(animator != null)
            animator.Play("FadeIn");
        if(player != null && player.GetComponent<PositionManager>() != null)
        StartCoroutine(DelayTeleport(player.GetComponent<PositionManager>().GetInitialPosition()));
        gManager.StageLevel.Value = 0;
       
        gameState.Value = GameState.UI;
        m_EnterLobbyEvent.Invoke();
        m_Custom_EndGameEvent.Invoke();
        //NetworkManager.Singleton.Shutdown();
    }
    public void LoseGame()
    {

        m_DisplayManager.GetDisplay("Title").DisplayController.StartFadeInText( "You Lose!", Color.red, 0.5f, 5f,0.5f );
        if (gameState.Value == GameState.Default)
        {
            gameState.Value = GameState.Lose;
            isEnded = true;
        }

    }
    public void WinGame()
    {
        m_DisplayManager.GetDisplay("Title").DisplayController.StartFadeInText( "You won the game!", Color.green, 0.5f, 5f, 0.5f);
        if (gameState.Value == GameState.Default)
        {
            gameState.Value = GameState.Win;
            isEnded= true;
        }
    }
    public void DetectGameState()
    {
        if(StageManager.Singleton.WinStage)
        {
            WinGame();
        }else
        {
            LoseGame();
        }
    }
    public static void InvokeUnityEvent(UnityEvent unityEvent)
    {
        //Return if have no value inside UnityEvent
        if (unityEvent.GetPersistentEventCount() == 0) return;
        bool hasPersistentTarget = false;
        //Check if unityEvent have any function inside
        for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
        {
            if (unityEvent.GetPersistentTarget(i) != null)
            {
                hasPersistentTarget = true;
            }
        }
        if(hasPersistentTarget)
        {
            unityEvent.Invoke();
        }
    }
    // Update is called once per frame
    void Update()
    {
        //NetworkDetection();
        if (gameStarted == false) return;
        if (gManager.LoopingStageEvent.Length != 0 && gManager.LoopingStageEvent.Length <= gManager.StageLevel.Value)
        {
            InvokeUnityEvent(gManager.LoopingStageEvent[gManager.StageLevel.Value]);
        }
        if(isEnded)
        {
            switch(gameState.Value)
            {
                case GameState.Lose:
                    StartCoroutine(EndGame());
                    break;
                case GameState.Win:
                    StartCoroutine(EndGame());
                    break;

            }
            isEnded = false;
        }
    }


    #region loadFunctions
    public void StartLoadStage()
    {
        StartCoroutine(LoadStage());
    }
    public IEnumerator LoadStage()
    {
        foreach (LoadFunction load in loadState)
        {
            yield return StartCoroutine(LoadFunction(load));
        }
        AddStageLevel();
    }
    private IEnumerator LoadFunction(LoadFunction loadFunction)
    {
        loadFunction.loadEvent.Invoke();
        yield return new WaitForSecondsRealtime(0.75f);
    }
    
    #endregion
    //public void NetworkDetection()
    //{
    //    if(networkData != null && networkData.startNetwork == true)
    //    {
    //        ConnectServer();
    //        StartGame();
    //        networkData.startNetwork = false;
    //    }
    //}
    //private void ConnectServer()
    //{
    //    if(networkData.startClient)
    //    {
    //        NetworkManager.Singleton.StartClient();
    //        networkData.startClient = false;
    //    }
    //    if(networkData.startHost)
    //    {
    //        NetworkManager.Singleton.StartHost();
    //        networkData.startHost = false;
    //    }
    //}
}
[System.Serializable]
public struct GManager 
{
    public NetworkVariable<int> StageLevel;
    [TextArea]
    public string[] noteForStage;
    [Header("Unity Event Functions must be void\nNote: Each Element is a different stage")]
    [SerializeField]
    private UnityEvent[] m_StartStageEvent;
    public UnityEvent[] StartStageEvent { get {return m_StartStageEvent; } }
    [SerializeField]
    private UnityEvent[] m_LoopingStageEvent;
    public UnityEvent[] LoopingStageEvent { get {return m_LoopingStageEvent; } }
}
[System.Serializable]
public class LoadFunction
{
    public LoadState loadState;
    public UnityEvent loadEvent;

}
public enum GameState
{
    Default,
    Win,
    Lose,
    UI
}
public enum LoadState
{
    LoadScene,
    RandomGeneration,
    StructureGeneration,
    InventoryGeneration,
    PlayerSet,
    Custom


}
