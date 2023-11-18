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
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite normalSprite;
    private Toggle toggle;
    private Image toggleTargetImage;

    private void Start()
    {
        // Get the Toggle component attached to this object
        toggle = GetComponent<Toggle>();
        toggleTargetImage = toggle.targetGraphic.GetComponent<Image>();
        normalSprite = toggleTargetImage.sprite;
        // Subscribe to the onValueChanged event
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }
    private void Update()
    {
        if (quizSO.questions[page].AnswerType == AnsType.Multiple_Choice)
        {
            if (toggle.isOn == false)
            {
                toggleTargetImage.sprite = selectedSprite;
            }

        }
    }
    public void setAns(int id, string targetText, int pg, QuizSO targetQuizSO)
    {
        ansId = id;
        text = targetText;
        page = pg;
        quizSO = targetQuizSO;
    }
    public void selectAns()
    {
        if (quizSO.questions[page].AnswerType != AnsType.Multiple_Choice)
        {
            DisSelect();
            quizSO.questions[page].inputAnswer = arrayBehaviour.ResetArray(quizSO.questions[page].inputAnswer);

        } else
        {
            if (toggle.interactable == true && toggle.isOn == true)
            {
                // The toggle has been untoggled
                for (int i = 0; i < quizSO.questions[page].inputAnswer.Length; i++)
                {
                    if (quizSO.questions[page].inputAnswer[i] == ansId)
                    {

                        quizSO.questions[page].inputAnswer = arrayBehaviour.RemoveArray(quizSO.questions[page].inputAnswer, i);

                    }
                }
                
                Debug.Log("Toggle untoggled!");
                return;
            }
        }

        
        quizSO.questions[page].inputAnswer = arrayBehaviour.addArray(quizSO.questions[page].inputAnswer);
        quizSO.questions[page].inputAnswer[quizSO.questions[page].inputAnswer.Length-1] = ansId;
        if (quizSO.questions[page].AnswerType != AnsType.Multiple_Choice)
        {
            quizSO.questions[page].inputAnswer = arrayBehaviour.RemoveArray(quizSO.questions[page].inputAnswer, 0);
            toggle.interactable = false;
            
        } else 
        {
            
        }
    }
    public void checkSelect()
    {
        for (int i = 0; i < quizSO.questions[page].inputAnswer.Length; i++)
        {
            if (quizSO.questions[page].inputAnswer[i] == ansId)
            {

                toggle.isOn = true;
                if(quizSO.questions[page].AnswerType != AnsType.Multiple_Choice)
                toggle.interactable = false;
            } else
            {
                toggle.isOn = false;
                if (quizSO.questions[page].AnswerType != AnsType.Multiple_Choice)
                    toggle.interactable = true;
            }
        }
    }
    private void OnToggleValueChanged(bool isOn)
    {
        
    }
    private void DisSelect()
    {
        GameObject parentContent = gameObject.transform.parent.gameObject;

        foreach (Transform child in parentContent.transform)
        {
            Toggle toggle1 = child.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle1.isOn = false;
                toggle1.interactable = true;
            }
        }
    }
    public void checkAns()
    {
        quizSO.checkSingleAns(page);
    }
}
