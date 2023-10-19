using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerHelper : Singleton<VideoPlayerHelper>
{
    [Header("Material")]
    [SerializeField]
    private Material m_VideoMaterialRef;

    [Header("UI")]
    [SerializeField]
    private GameObject m_Container;
    [SerializeField]
    private Image m_Renderer;

    private Material m_VideoMaterial;
    private VideoPlayer m_VideoPlayer;
    private UnityAction m_CompleteAction;

    protected override void Awake()
    {
        base.Awake();
        m_VideoMaterial = Instantiate(m_VideoMaterialRef);
        m_Renderer.material = m_VideoMaterial;
        m_VideoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
        => m_Container.SetActive(false);

    private void OnEnable()
    {
        m_VideoPlayer.prepareCompleted += OnVideoPrepared;
        m_VideoPlayer.loopPointReached += OnLoopPointReached;
    }

    private void OnDisable()
    {
        m_VideoPlayer.prepareCompleted -= OnVideoPrepared;
        m_VideoPlayer.loopPointReached -= OnLoopPointReached;
    }

    public void PlayOneshot(string url, UnityAction onComplete)
    {
        m_VideoPlayer.url = url;
        m_VideoPlayer.Prepare();
        m_CompleteAction = onComplete;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        m_VideoMaterial.mainTexture = m_VideoPlayer.texture;
        m_VideoPlayer.Play();
        m_Container.SetActive(true);
    }

    private void OnLoopPointReached(VideoPlayer source)
    {
        m_Container.SetActive(false);
        m_CompleteAction?.Invoke();
        m_CompleteAction = null;
    }
}
