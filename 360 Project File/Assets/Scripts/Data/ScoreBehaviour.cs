using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreSO", menuName = "ScriptableObjects/ScoreSO")]
public class ScoreBehaviour : ScriptableObject
{
    [SerializeField] private int score;

    public void addScore(int sc)
    {
        score += sc;
    }
    public int getScore()
    {
        return score;
    }
    public void setScore(int sc)
    {
        score = sc;
    }

}

[System.Serializable]
public class Score
{
    [SerializeField] private int rewardScore;
    ScoreBehaviour scoreBehaviour;
    public void addScore()
    {
        scoreBehaviour.addScore(rewardScore);
        Debug.Log("Score: " + scoreBehaviour.getScore());
    }

    public int getAddScore()
    {
        return rewardScore;
    }
}
