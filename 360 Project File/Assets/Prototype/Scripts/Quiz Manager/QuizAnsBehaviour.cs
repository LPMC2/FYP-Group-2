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
    [SerializeField] private bool isPressed = false;
    public Toggle toggle;
    private Image toggleTargetImage;
    private Button btn;
    private void Start()
    {
        if (quizSO.questions[page].AnswerType != AnsType.Multiple_Choice)
        {
            quizSO.questions[page].inputAnswer = new int[1];
            quizSO.questions[page].inputAnswer[quizSO.questions[page].inputAnswer.Length - 1] = -1;
        } else
        {
            quizSO.questions[page].inputAnswer = new int[0];
        }
        // Get the Toggle component attached to this object
        toggle = GetComponent<Toggle>();
        btn = GetComponent<Button>();
        if(btn != null)
        {
            toggleTargetImage = btn.targetGraphic.GetComponent<Image>();
        }
        if (toggle == null)
            return;
        toggleTargetImage = toggle.targetGraphic.GetComponent<Image>();
        normalSprite = toggleTargetImage.sprite;
        // Subscribe to the onValueChanged event
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

    }
    private void Update()
    {
        if (quizSO.questions[page].AnswerType == AnsType.Multiple_Choice)
        {
            if (isPressed == true)
            {
                toggleTargetImage.sprite = selectedSprite;
            } else if(isPressed == false)
            {
                toggleTargetImage.sprite = normalSprite;
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
            isPressed = !isPressed;
            Button btn = gameObject.GetComponent<Button>();
            if (btn == null) return;
            if (btn.interactable == true && isPressed == false)
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

        if (toggle != null || isPressed == true)
        {
            if(quizSO.questions[page].AnswerType == AnsType.Multiple_Choice)

            quizSO.questions[page].inputAnswer = arrayBehaviour.addArray(quizSO.questions[page].inputAnswer);
            quizSO.questions[page].inputAnswer[quizSO.questions[page].inputAnswer.Length - 1] = ansId;
        }
        if (quizSO.questions[page].AnswerType != AnsType.Multiple_Choice)
        {
            
            toggle.interactable = false;
            
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
        if (isOn == true)
        {
            isOn = false;
        }
    }
    private void DisSelect()
    {
        GameObject parentContent = gameObject.transform.parent.gameObject;

        foreach (Transform child in parentContent.transform)
        {
            Toggle toggle1 = child.GetComponent<Toggle>();
            if (toggle1 != null)
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
