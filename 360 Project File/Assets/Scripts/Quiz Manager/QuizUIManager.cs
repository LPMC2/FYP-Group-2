//Note: For Answer UI of Selected, you have to set the Disabled UI instead of Selected UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuizUIManager : MonoBehaviour
{
    [SerializeField] 
    [Header("Quiz Question")]
    private QuizSO quizScriptableObject;
    /*
    For lang variable:
        English - en
        Chinese Traditional - zh_HK
        Chinese Simplified - zh_CN
    */
    public string lang = "";
    [SerializeField] GameObject contentQuestions;
    [SerializeField] GameObject questionsUIObject;
    [SerializeField] GameObject answerUIObject;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject backBtn;
    [SerializeField] GameObject checkBtn;
    [SerializeField] QuizSO quizSO;
    [SerializeField] ScoreBehaviour scoreSO;
    [SerializeField] DataStorage dataStorage;
    Vector3 contentTransform;
    [SerializeField] private int page = 0;
    int pagelimit = 1;
    void Start()
    {
        if (scoreSO.getScore() != 0)
            scoreSO.setScore(0);
        for (int i = 0; i < quizSO.questions.Length; i++)
        {
            quizSO.questions[i].inputAnswer = -1;
            quizSO.GetAns(i, lang);
        }
        quizSO.correctCount = -1;
        dataStorage.questions = arrayBehaviour.ResetArray<string>(dataStorage.questions);
        dataStorage.optionsNum = arrayBehaviour.ResetArray<int>(dataStorage.optionsNum);
        dataStorage.options = arrayBehaviour.ResetArray<string>(dataStorage.options);
        dataStorage.userInput = arrayBehaviour.ResetArray<string>(dataStorage.userInput);
        dataStorage.answers = arrayBehaviour.ResetArray<string>(dataStorage.answers);
        //float textHeight = 0;
        //float contentHeight = 0;
        contentTransform = contentQuestions.transform.position;
        ////if (questionsText.GetComponent<RectTransform>())
        ////{
        ////    textHeight = questionsText.GetComponent<RectTransform>().rect.height;
        ////}
        ////if (contentQuestions.GetComponent<RectTransform>())
        ////{
        ////    contentHeight = contentQuestions.GetComponent<RectTransform>().rect.height;
        //}
        pagelimit = quizScriptableObject.questions.Length - 1;

        setQuestion();
        getAnswers();
        backBtn.SetActive(false);
        checkBtn.SetActive(false);
       
    }
    public void setLanguage(string type)
    {
        lang = type;
        reloadPage();
    }
    public void nextPage()
    {
        if (page < pagelimit)
        {
            page++;
            //Remove All Child Objects inside the content(As the previous page content still exists)
            foreach (Transform child in contentQuestions.transform)
            {
                Destroy(child.gameObject);
            }
            setQuestion();
            getAnswers();
            //Set "Next" Button and "Back" button to be visible or not depending on the current page
            if (page == pagelimit)
            {
                checkBtn.SetActive(true);
                nextBtn.SetActive(false);
            } else
            {
               
                checkBtn.SetActive(false);
                nextBtn.SetActive(true);
            }
            backBtn.SetActive(true);
        }
    }
    public void backPage()
    {
        if (page > 0)
        {
            page--;
            foreach (Transform child in contentQuestions.transform)
            {
                Destroy(child.gameObject);
            }
            setQuestion();
            getAnswers();
            if (page == 0)
            {
                backBtn.SetActive(false);
                nextBtn.SetActive(true);
            } else
            {

                nextBtn.SetActive(true);
            }
            checkBtn.SetActive(false);
        }
    }
    public void reloadPage()
    {
        foreach (Transform child in contentQuestions.transform)
        {
            Destroy(child.gameObject);
        }
        setQuestion();
        getAnswers();
    }
    public void setQuestion()
    {
        //Add UI Object to the content
        GameObject target = Instantiate(questionsUIObject, contentTransform, Quaternion.identity);
        target.transform.SetParent(contentQuestions.transform);
        TMP_Text targettext;
        if (target.GetComponent<TMP_Text>() != null)
        {
            targettext = target.GetComponent<TMP_Text>();
        } else
        {
            targettext = target.transform.GetChild(0).GetComponent<TMP_Text>();
        }
        //Add text to the UI from the question
        targettext.text = quizScriptableObject.GetQuestion(page, lang);
    }
    string[] answers;
    public void setAnswers()
    {
        //Add Answer UI
        GameObject target = Instantiate(answerUIObject, contentTransform, Quaternion.identity);
    }
    private void getAnswers()
    {
        //Put Answer Text into the Answers UI
        string[] ans;
        ans = quizScriptableObject.GetAnswers(page,lang);
        for(int i=0; i<ans.Length; i++)
        {
            GameObject target = Instantiate(answerUIObject, contentTransform, Quaternion.identity);
            target.transform.SetParent(contentQuestions.transform);
            QuizAnsBehaviour targetComponent;
            if (target.GetComponent<QuizAnsBehaviour>() != null)
            {
                targetComponent = target.GetComponent<QuizAnsBehaviour>();
            } else
            targetComponent = target.AddComponent<QuizAnsBehaviour>();
            //Set Value to QuizAnsBehaviour
            targetComponent.setAns(i, ans[i], page);
            TMP_Text targettext;
            if (target.transform.GetChild(0).GetComponent<TMP_Text>() != null)
            {
                targettext = target.transform.GetChild(0).GetComponent<TMP_Text>();
                targettext.text = ans[i];
            } else if(target.transform.GetChild(1).GetComponent<TMP_Text>() != null)
            {
                targettext = target.transform.GetChild(1).GetComponent<TMP_Text>();
                targettext.text = ans[i];
            }
            targetComponent.checkSelect();
        }
    }
}
