using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreSO", menuName = "ScriptableObjects/ScoreSO")]
public class ScoreBehaviour : ScriptableObject
{
    [SerializeField] private int score;
    public int correctScore = 1;
    public float timeBonusMultiplier = 1f;
    private int correctCalScore;
    private float timeBonusScore;
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
    public void reset()
    {
        correctCalScore = 0;
        timeBonusScore = 0;
        score = 0;
    }
    public void calculateScore(int correctAmount, float timeRemain, float TotalTime)
    {
        correctCalScore = correctAmount * correctScore;
        timeBonusScore = 1 + (timeRemain/TotalTime) * timeBonusMultiplier;
        score = Mathf.RoundToInt(correctCalScore * timeBonusScore);
    }
    public int getCorrectScore()
    {
        return correctCalScore;
    }
    public float getTimeBonusScore()
    {
        return (float)Mathf.Ceil(timeBonusScore * 100) / 100;
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
