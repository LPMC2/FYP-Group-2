using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Color normalColor;
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color correctColor;
    [SerializeField] private Color incorrectColor;
    [SerializeField] private bool isPressed = false;
    private GameObject ansIndicatorObject;
    private GameObject backGround;
    #region Color Modifier Settings
    [Header("Color Transition Settings")]
    private Color startColor;
    private Color endColor;
    private Image targetImage;
    [SerializeField] private float duration = 1f;
    private bool isActive = false;
    private float timer = 0f;
    public CorrectState correctState { get; private set; }
    public void StartColorTransition(Image target, Color StartColor, Color EndColor)
    {
        startColor = StartColor;
        endColor = EndColor;
        isActive = true;
        targetImage = target;
        timer = 0f;
        
    }
    private void ChangeColor()
    {
        if (!isActive) return;
        timer += Time.deltaTime;

        // Calculate the current color based on the timer and duration
        float t = Mathf.Clamp01(timer / duration);
        Color currentColor = Color.Lerp(startColor, endColor, t);

        // Assign the current color to the sprite renderer
        targetImage.color = currentColor;

        // Check if the color change is complete
        if (t >= 1f)
        {
            timer = 0f;
            isActive = false;
        }
    }
    #endregion
    public Toggle toggle;
    private Image toggleTargetImage;
    private Button btn;
    private void Start()
    {
        backGround = GameObjectFinder.GetGameObjectWithTagFromChilds(gameObject, "Bg");
        ansIndicatorObject = GameObjectFinder.GetGameObjectWithTagFromChilds(gameObject, "Indicator");
        GetComponent<Animator>().enabled = false;
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
    public void SetCorrectStateUI(CorrectState correctState)
    {
        Image ansIndicatorImage = ansIndicatorObject.GetComponent<Image>();
        TMP_Text ansIndicatorText = ansIndicatorObject.transform.GetChild(0).GetComponent<TMP_Text>();
        Outline outline = backGround.GetComponent<Outline>();
        if (ansIndicatorText == null || ansIndicatorImage == null) return;
        ansIndicatorObject.SetActive(true);
        this.correctState = correctState;
        switch(correctState)
        {
            case CorrectState.Correct:
                ansIndicatorText.text = LocalizableString.GetLocalizableString(QuizUIManager.Singleton.lang, QuizUIManager.QuizSOSingleton.correctText);
                StartColorTransition(ansIndicatorImage, ansIndicatorImage.color, correctColor);
                outline.effectColor = correctColor;
                break;
            case CorrectState.Incorrect:
                ansIndicatorText.text = LocalizableString.GetLocalizableString(QuizUIManager.Singleton.lang, QuizUIManager.QuizSOSingleton.incorrectText);
                StartColorTransition(ansIndicatorImage, ansIndicatorImage.color, incorrectColor);
                outline.effectColor = incorrectColor;
                break;
            case CorrectState.CorrectAnswer:
                ansIndicatorText.text = LocalizableString.GetLocalizableString(QuizUIManager.Singleton.lang, QuizUIManager.QuizSOSingleton.correctAnsText);
                StartColorTransition(ansIndicatorImage, ansIndicatorImage.color, correctColor);
                outline.effectColor = correctColor;
                break;
            case CorrectState.Default:
                ansIndicatorObject.SetActive(false);
                break;
        }
    }
    private void Update()
    {
        //if (quizSO.questions[page].AnswerType == AnsType.Multiple_Choice)
        //{
        //    if (isPressed == true)
        //    {
        //        toggleTargetImage.col = selectedSprite;
        //    } else if(isPressed == false)
        //    {
        //        toggleTargetImage.sprite = normalSprite;
        //    }

        //}
        ChangeColor();
    }
    public void SetPressedState(bool value)
    {
        isPressed = value;
        switch(isPressed) 
        {
            case true:
                StartColorTransition(toggleTargetImage ,toggleTargetImage.color, SelectedColor);
                break;
            case false:
                StartColorTransition(toggleTargetImage, toggleTargetImage.color, normalColor);
                break;
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
            SetPressedState(!isPressed);
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
                
                //Debug.Log("Toggle untoggled!");
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
public enum CorrectState
{
    Default,
    Correct,
    Incorrect,
    CorrectAnswer
}