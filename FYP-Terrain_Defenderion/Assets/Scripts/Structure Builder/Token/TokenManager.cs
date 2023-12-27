using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TokenManager : MonoBehaviour
{
    [SerializeField] private int tokens = 0;
    [SerializeField] private TMP_Text tokenDisplayText;
    [SerializeField] private DisplayBehaviour displayBehaviour;
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
    }
#endregion
    // Start is called before the first frame update
    void Awake()
    {
    }
    private void Start()
    {
        displayBehaviour = DisplayBehaviour.Singleton;
        initialTokens = tokens;
    }
    public static int GetTokenCost(int blockID)
    {
        BlockSO blockData = BlockManager.BlockData;
        int cost = 0;
        cost = blockData.blockData[blockID].tokenCost;
        return cost;
    }
    // Update is called once per frame
    void Update()
    {
        
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

                displayBehaviour.StartFadeInText("Unable to afford " + itemName + ". Need " + -(getTokens() - cost) + " more tokens", Color.red);

                return false;
            }
        

        return false;
    }
}
