using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenBehaviour : MonoBehaviour
{
    [SerializeField] private int tokens = 0;
# region Getter and Setter
  public int getTokens()
    {
        return tokens;
    }
    public void setTokens(int value)
    {
        tokens = value;
    }
#endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
