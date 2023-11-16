using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuizAnsBehaviour : MonoBehaviour
{
    [SerializeField] private int page;
    [SerializeField] private int ansId;
    [SerializeField] private string text;
    [SerializeField] QuizSO quizSO;
    [SerializeField] ScoreBehaviour scoreSO;

    public void setAns(int id, string targetText, int pg, QuizSO targetQuizSO)
    {
        ansId = id;
        text = targetText;
        page = pg;
        quizSO = targetQuizSO;
    }
    public void selectAns()
    {
        DisSelect();
        Toggle btn = gameObject.GetComponent<Toggle>();
        quizSO.questions[page].inputAnswer = ansId;
        btn.interactable = false;
    }
    public void checkSelect()
    {
        if(quizSO.questions[page].inputAnswer == ansId)
        {
            Toggle btn = gameObject.GetComponent<Toggle>();
            btn.isOn = true;
            btn.interactable = false;
        }
    
    }
    private void DisSelect()
    {
        GameObject parentContent = gameObject.transform.parent.gameObject;

        foreach (Transform child in parentContent.transform)
        {
            Toggle btn = child.GetComponent<Toggle>();
            if (btn != null)
            {
                btn.isOn = false;
                btn.interactable = true;
            }
        }
    }
    public void checkAns()
    {
        quizSO.checkSingleAns(page);
    }
}
