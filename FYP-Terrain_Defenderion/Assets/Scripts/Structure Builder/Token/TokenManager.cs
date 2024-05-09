using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TokenManager : MonoBehaviour
{
    [SerializeField] private int tokens = 0;
    [SerializeField] private TMP_Text tokenDisplayText;
    [SerializeField] private DisplayBehaviour displayBehaviour;
    [SerializeField] private bool displayToken = false;
    Color initialColor;
    public int initialTokens {get; private set; }
# region Getter and Setter
  public int getTokens()
    {
        return tokens;
    }
    public void setTokens(int value)
    {
        tokens = value;
    }
    public void addTokens(int value)
    {
        tokens += value;
        UpdateTokenDisplay();
    }
#endregion
    // Start is called before the first frame update
    void Awake()
    {
    }
    private void Start()
    {
        initialTokens = tokens;
        if(tokenDisplayText != null)
        initialColor = tokenDisplayText.color;
        UpdateTokenDisplay();
    }
    private void UpdateTokenDisplay()
    {
        if (tokenDisplayText != null && displayToken)
        {
            tokenDisplayText.text = "Tokens Left: " + tokens.ToString();
        }
    }
    public static int GetTokenCost(int blockID)
    {
        BlockSO blockData = BlockManager.BlockData;
        if (blockID < 0 || blockID > blockData.blockData.Length) return 0;
        int cost = 0;
        cost = blockData.blockData[blockID].tokenCost;
        return cost;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    Coroutine resetCoroutine;
    private IEnumerator Reset(string text)
    {
        Debug.Log("Test1");
        yield return new WaitForSeconds(displayBehaviour.TotalDuration * 1.1f);
        tokenDisplayText.text = text;
        tokenDisplayText.alpha = 1f;
        tokenDisplayText.color = initialColor;

        Debug.Log("Test2");
    }
    public bool isTokenAffordable(int cost, string itemName = null)
    {
        
            if (getTokens() - cost >= 0)
            {
                addTokens(-cost);
               
                return true;
            }
            else
            {
                string InitialText = tokenDisplayText.text = "Tokens Left: " + tokens.ToString();
                displayBehaviour.StartFadeInText("Unable to afford " + itemName + ". Need " + -(getTokens() - cost) + " more tokens", Color.red);
                if (resetCoroutine != null)
                {
                    StopCoroutine(resetCoroutine);
                }
                resetCoroutine = StartCoroutine(Reset(InitialText));
                return false;
            }
        

        return false;
    }
}
