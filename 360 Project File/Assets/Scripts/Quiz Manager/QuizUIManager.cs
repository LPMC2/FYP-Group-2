//Note: For Answer UI of Selected, you have to set the Disabled UI instead of Selected UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System;

public class QuizUIManager : MonoBehaviour
{
    [Header("Quiz Question")]
    public Language lang = Language.zh_HK;
    /*
    For lang variable:
        English - en
        Chinese Traditional - zh_HK
        Chinese Simplified - zh_CN
    */

    [Header("Main UI")]
    [SerializeField] private int page = 0;
    [SerializeField] private GameObject mainQuizCanvas;
    [SerializeField] private GameObject currentContentUI;
    [SerializeField] private GameObject StartPageUI;
    [SerializeField] private GameObject FailPageUI;
    [SerializeField] private GameObject qnaPanelMain;
    [SerializeField] private GameObject qnaPanelContent;

    [Header("End UI - Main")]
    [SerializeField] private GameObject EndPageUI;
    [SerializeField] private GameObject endQuestionPrefab;
    [SerializeField] private GameObject endQContent;
    [SerializeField] private GameObject ScoreEllipseBar;
    [SerializeField] private float lerpSpeed = 1f;
    [Header("End UI - Score")]
    [SerializeField] private TMP_Text correctCountUI;
    [SerializeField] private TMP_Text correctScoreUI;
    [SerializeField] private TMP_Text timeBonusUI;
    [SerializeField] private TMP_Text timeScoreUI;
    [SerializeField] private TMP_Text totalScoreUI;
    [SerializeField] private TMP_Text ScoreProportionUI;
    [Header("End UI - Stars")]
    [SerializeField] private GameObject starsUI;
    [SerializeField] private Sprite grayStar;
    [SerializeField] private Sprite star;
    [Header("Q&A UI")]
    [SerializeField] private GameObject questionNumUI;
    [SerializeField] private GameObject questionsUIObject;
    [SerializeField] private GameObject answerUIObject;
    public GameObject AnswerUIObject { set { answerUIObject = value; } }
    [SerializeField] private GameObject headerUI;
    [SerializeField] private Slider progressBar;
    [SerializeField] private float progressDuration;
    [SerializeField] private TMP_Text remainingTimeUI;
    [SerializeField] private GameObject explainationUI;
    private Coroutine timerCoroutine;
    private string timeUIText;
    private float targetProgress = 0.0f;
    private float currentProgress = 0.0f;
    private bool runProgress = false;
    private bool end = false;
    private int timeRemaining = 0;
    [Header("Answer Type UI")]
    [SerializeField] private GameObject textUIObject;
    [SerializeField] private GameObject pictureUIObject;
    #region UI Getter
    public GameObject TextUIObject {
        get { return textUIObject; }

    }
    public GameObject PictureUIObject
    {
        get { return pictureUIObject; }

    }

    #endregion

    [Header("Buttons")]
    [SerializeField] private GameObject nextBtn;
    [SerializeField] private GameObject backBtn;
    [SerializeField] private GameObject checkBtn;

    [Header("Animation")]
    [SerializeField] private GameObject contentPrefab;
    [SerializeField] private GameObject clonedQuizPanel;
    [SerializeField] private GameObject baseQuizPanel;
    private GameObject[] aniPanel = new GameObject[3];

    [SerializeField] private Vector2 offset;
    [Header("Data")]
    [SerializeField] private QuizSO quizSO;
    [SerializeField] private ScoreBehaviour scoreSO;
    [SerializeField] private DataStorage dataStorage;
    private string[] alphabet = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    Vector3 contentTransform;

    int pagelimit = 1;
    void Start()
    {
        SetupVertical(StartPageUI, 2f);
        timeRemaining = 0;
        quizSO.correctCount = -1;
        dataStorage.questions = arrayBehaviour.ResetArray<string>(dataStorage.questions);
        dataStorage.optionsNum = arrayBehaviour.ResetArray<int>(dataStorage.optionsNum);
        dataStorage.options = arrayBehaviour.ResetArray<string>(dataStorage.options);
        dataStorage.userInput = arrayBehaviour.ResetArray<string>(dataStorage.userInput);
        dataStorage.answers = arrayBehaviour.ResetArray<string>(dataStorage.answers);
        //float textHeight = 0;
        //float contentHeight = 0;
        contentTransform = currentContentUI.transform.position;
        ////if (questionsText.GetComponent<RectTransform>())
        ////{
        ////    textHeight = questionsText.GetComponent<RectTransform>().rect.height;
        ////}
        ////if (contentQuestions.GetComponent<RectTransform>())
        ////{
        ////    contentHeight = contentQuestions.GetComponent<RectTransform>().rect.height;
        //}
        pagelimit = quizSO.questions.Length - 1;

        setTimeUI();
        remainingTimeUI.gameObject.SetActive(false);
        if (page >= 0)
        {
            if (scoreSO.getScore() != 0)
                scoreSO.setScore(0);
            for (int i = 0; i < quizSO.questions.Length; i++)
            {
                quizSO.questions[i].inputAnswer = -1;
                quizSO.GetAns(i, lang);
            }
            if(explainationUI != null)
            explainationUI.SetActive(false);
            GetQuestionNumber();
            startTime();

            getAnswers();
            backBtn.SetActive(false);
            checkBtn.SetActive(true);
            SetQuestion();
        } else
        {
            backBtn.SetActive(false);
            checkBtn.SetActive(false);
        }



    }
    public void getLanguage()
    {
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
        {
            lang = Language.en;
        }
        else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1])
        {
            lang = Language.zh_HK;
        }
        else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[2])
        {
            lang = Language.zh_CN;
        }
        
    }
    private Dictionary<GameObject, bool> scaledObjects = new Dictionary<GameObject, bool>();
    private void SetupVertical(GameObject ui, float scale)
    {
        if (scaledObjects.ContainsKey(ui) && scaledObjects[ui])
            return;
        // Check the current screen orientation
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        // Determine if it's a vertical screen
        bool isVerticalScreen = screenHeight > screenWidth;

        // Adjust the scale constraint based on the screen orientation
        if (isVerticalScreen)
        {
            SetVerticalUI(ui, scale);
            scaledObjects[ui] = true;
        }
    }
    private void SetVerticalUI(GameObject ui, float scale)
    {

        RectTransform rectTransform = ui.GetComponent<RectTransform>();
     
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y * scale);
        }
    }

    public void setTimeUI()
    {
        timeUIText = remainingTimeUI.text;
    }
    public void startTime()
    {

        timerCoroutine = StartCoroutine(StartTimer());
    }
    public void setPage(int index)
    {
        page = index;
    }
    private void Update()
    {

    }
    public void setLanguage(string type)
    {
        switch (type)
        {
            case "en":
                lang = Language.en;
            break;
            case "zh_HK":
                lang = Language.zh_HK;
                break;
            case "zh_CN":
                lang = Language.zh_CN;
                break;
        }

        reloadPage();
    }
    private void setExplainationUI()
    {
        if (explainationUI != null)
        {
            TMP_Text explUI = explainationUI.transform.GetChild(0).GetComponent<TMP_Text>();
            explainationUI.SetActive(true);
            explUI.text = quizSO.getLanText(quizSO.questions[page].explaination, lang);
        }
    }
    public void checkAnswer()
    {
        Debug.Log("Answer Number: " + (quizSO.questions[page].answer + 1) + "\n" + "Input Number: " + (quizSO.questions[page].inputAnswer + 1));
        GameObject targetUI;
        GameObject ansUI;
        if (currentContentUI.transform.childCount >= 1 && quizSO.questions[page].inputAnswer != -1)
        {
            if (!currentContentUI.transform.GetChild(0).CompareTag("Answer"))
            {
                targetUI = currentContentUI.transform.GetChild(quizSO.questions[page].inputAnswer + 1).gameObject;
                ansUI = currentContentUI.transform.GetChild(quizSO.questions[page].answer + 1).gameObject;
            } else
            {
                targetUI = currentContentUI.transform.GetChild(quizSO.questions[page].inputAnswer).gameObject;
                ansUI = currentContentUI.transform.GetChild(quizSO.questions[page].answer).gameObject;
            }
        } else
        {
            return;
        }
        Animator animatorTarget = targetUI.GetComponent<Animator>();
        Animator animatorAns = ansUI.GetComponent<Animator>();
        if (quizSO.questions[page].inputAnswer != -1)
        {
            switch (quizSO.checkSingleAns(page))
            {
                case true:
                    animatorTarget.Play("correct");

                    break;
                case false:
                    animatorTarget.Play("incorrect");
                    animatorAns.Play("correct");
                    setExplainationUI();
                    break;
            }
            nextBtn.SetActive(true);
            checkBtn.SetActive(false);
            currentContentUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
            startProgress();
            resetOptions();
        } else
        {
            checkBtn.GetComponent<Animator>().Play("unavailable");
        }
    }

    private void removeChild(GameObject parentObject, string specificTag)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.transform.CompareTag(specificTag))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void StartPageTransition()
    {
        Vector2 currentPosition;
        RectTransform rectTransform = clonedQuizPanel.GetComponent<RectTransform>();
        currentPosition = rectTransform.anchoredPosition;
        Debug.Log("Position: " + currentPosition);
        GameObject clonedContent = Instantiate(clonedQuizPanel, transform);
        clonedContent.transform.SetParent(mainQuizCanvas.transform);
        clonedContent.transform.SetAsLastSibling();
        RectTransform clonedContentRect = clonedContent.GetComponent<RectTransform>();
        clonedContentRect.anchoredPosition = currentPosition;
        nextPage();
        
        GameObject nextUI = Instantiate(baseQuizPanel, transform);
        nextUI.name = "TEXTCLONED";
        nextUI.transform.SetParent(mainQuizCanvas.transform);
        nextUI.transform.SetAsLastSibling();
        RectTransform clonedNextRect = nextUI.GetComponent<RectTransform>();
        clonedNextRect.anchoredPosition = currentPosition + offset;
        clonedQuizPanel.SetActive(false);

        
    }
    private void resetOptions()
    {
        foreach(Transform child in currentContentUI.transform)
        {
            if(child.CompareTag("Answer")) {
                child.gameObject.GetComponent<Toggle>().interactable = true;
            }
        }
    }
    public void nextPage()
    {
        if(page == -1)
        {
            startTime();
            qnaPanelMain.SetActive(true);

        }
        if (page < pagelimit)
        {
            page++;
            QuizAnsUIManager quizAnsUIManager = currentContentUI.GetComponent<QuizAnsUIManager>();
            if (quizAnsUIManager != null)
            {
                quizAnsUIManager.AnswerType = quizSO.questions[page].AnswerType;
            }
            SetupVertical(qnaPanelMain, 2f);
            SetupVertical(qnaPanelContent, 2f);
            //Set Quiz Data(Language Id)
            quizSO.GetAns(page, lang);
            //Reset Input Answer
            quizSO.questions[page].inputAnswer = -1;
            //Remove All Child Objects inside the content(As the previous page content still exists)
            removeChild(currentContentUI, "Answer");

            SetQuestion();
            getAnswers();
            if(explainationUI != null)
            explainationUI.SetActive(false);
            //Set "Next" Button and "Back" button to be visible or not depending on the current page

                checkBtn.SetActive(true);
                nextBtn.SetActive(false);
            GetQuestionNumber();
            SetupVertical(currentContentUI, 2f);
            ChangeText changeText = gameObject.GetComponent<ChangeText>();
            if (changeText != null)
            {
                changeText.InitialFont();
            }
        } else if(page == pagelimit)
        {
            QuizEnd();
        }
        currentContentUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    private void QuizEnd()
    {
        end = true;
        page++;
        quizSO.checkAns();
        StopTimer();
        remainingTimeUI.gameObject.SetActive(false);
        EndPageUI.gameObject.SetActive(true);
        qnaPanelMain.gameObject.SetActive(false);
        getResult();
        SetupVertical(EndPageUI, 2f);
    }
    public void backPage()
    {
        if (page > 0)
        {
            page--;
            removeChild(currentContentUI, "Answer");
            SetQuestion();
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
    private void GetQuestionNumber()
    {
        GameObject target = questionNumUI.transform.GetChild(1).gameObject;
        TMP_Text targetTMP = target.GetComponent<TMP_Text>();
        targetTMP.text = (page+1) + "/" + quizSO.questions.Length;
    }
    public void reloadPage()
    {

        if (page >= 0 && page <= pagelimit)
        {
            SetQuestion();

            if (explainationUI != null)
            {
                setExplainationUI();
                if (currentContentUI.GetComponent<CanvasGroup>().blocksRaycasts != false)
                {
                    explainationUI.SetActive(false);
                }
            }
            GetQuestionNumber();
            reloadAns();
        }
    }
    public void SetQuestion()
    {
        //Modify UI Text Object to the content
        GameObject target = GameObjectFinder.GetGameObjectWithTagFromChilds(questionsUIObject, "QuestionText");
        TMP_Text targettext = target.GetComponent<TMP_Text>();
        if (targettext != null)
        {
            
            targettext.text = quizSO.GetQuestion(page, lang);
        } 
        //Add text to the UI from the question
        
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
        Sprite[] ansImg = default;
        ans = quizSO.GetAnswers(page,lang);
        if (quizSO.questions[page].AnswerType == AnsType.Picture)
        {
            ansImg = quizSO.GetImgAnswers(page, lang);
        }
        for (int i=0; i<ans.Length; i++)
        {
            GameObject target = Instantiate(answerUIObject, contentTransform, Quaternion.identity);
            target.transform.SetParent(currentContentUI.transform);
            QuizAnsBehaviour targetComponent;
            if (target.GetComponent<QuizAnsBehaviour>() != null)
            {
                targetComponent = target.GetComponent<QuizAnsBehaviour>();
            } else
            targetComponent = target.AddComponent<QuizAnsBehaviour>();

            //Set Value to QuizAnsBehaviour
            targetComponent.setAns(i, ans[i], page, quizSO);
            GameObject ansObj = GameObjectFinder.GetGameObjectWithTagFromChilds(target, "AnsText");
            if (ansObj != null)
            {
                TMP_Text targettext = ansObj.GetComponent<TMP_Text>();
                if (targettext != null)
                {
                    targettext.text = ans[i];
                }
            }
            GameObject alphabetObj = GameObjectFinder.GetGameObjectWithTagFromChilds(target, "Alphabet");
            if (alphabetObj != null)
            {
                TMP_Text alphaText = alphabetObj.GetComponent<TMP_Text>();
                if (alphaText != null)
                {
                    alphaText.text = alphabet[i];
                }
            }
            if(quizSO.questions[page].AnswerType == AnsType.Picture)
            {
                GameObject targetChild = GameObjectFinder.GetGameObjectWithTagFromChilds(target, "AnsImg");
                Image targetImg;
                targetImg = targetChild.GetComponent<Image>();
                if(targetImg != null)
                {
                    targetImg.sprite = ansImg[i];
                }
            }
        }
    }
    public void reloadAns()
    {
        string[] ans;
        ans = quizSO.GetAnswers(page, lang);
        Sprite[] ansImg = default;
        if (quizSO.questions[page].AnswerType == AnsType.Picture)
        {
            ansImg = quizSO.GetImgAnswers(page, lang);
        }
        for (int i = 0; i < currentContentUI.transform.childCount; i++)
        {
            if(!currentContentUI.transform.GetChild(0).CompareTag("Answer"))
            {
                break;
            }
            Debug.Log(i);
            GameObject target = currentContentUI.transform.GetChild(i).gameObject;
            QuizAnsBehaviour targetComponent = target.GetComponent<QuizAnsBehaviour>();
            if (ans.Length >= i)
            {
                Debug.Log(ans.Length + "/" + i);
                if (!currentContentUI.transform.GetChild(0).CompareTag("Answer"))
                    targetComponent.setAns(i - 1, ans[i - 1], page, quizSO);
                else
                    targetComponent.setAns(i, ans[i], page, quizSO);
            } else
            {
                targetComponent.setAns(i - 1, "Null", page, quizSO);
            }
            TMP_Text targettext;
            GameObject targetChild = GameObjectFinder.GetGameObjectWithTagFromChilds(target, "AnsText");
            if (targetChild.GetComponent<TMP_Text>() != null)
            {
                targettext = targetChild.GetComponent<TMP_Text>();
                if (ans.Length >= i)
                {
                    if (!currentContentUI.transform.GetChild(0).CompareTag("Answer"))
                        targettext.text = ans[i - 1];
                    else
                        targettext.text = ans[i];
                } else
                {
                    targettext.text = "";
                }
            }
            if (quizSO.questions[page].AnswerType == AnsType.Picture)
            {
                GameObject targetChildObj = GameObjectFinder.GetGameObjectWithTagFromChilds(target, "AnsImg");
                Image targetImg;
                targetImg = targetChildObj.GetComponent<Image>();
                if (targetImg != null)
                {
                    targetImg.sprite = ansImg[i];
                }
            }
        }
    }
    private void startProgress()
    {
        
        targetProgress = ((page + 1f) / quizSO.questions.Length) * 100f;
       
        currentProgress = progressBar.value;
        StartCoroutine(runningProgress());
    }
    IEnumerator runningProgress()
    {

        while (currentProgress != targetProgress)
        {
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime / progressDuration);
            progressBar.value = currentProgress;
            yield return null;
        }

        // Ensure the progress value reaches the exact target value
        currentProgress = targetProgress;
        progressBar.value = currentProgress;

    }
    private IEnumerator StartTimer()
    {
        float elapsedTime = quizSO.timeLimit;

        while (elapsedTime > 0)
        {
            UpdateTimerText(elapsedTime);
            elapsedTime -= Time.deltaTime;
            timeRemaining = Mathf.RoundToInt(elapsedTime);
            yield return null; // Pause the coroutine and resume from here in the next frame
        }

        // Timer has reached the time limit
        if (end == false)
        {
            page = -1;
            remainingTimeUI.gameObject.SetActive(false);
            EndPageUI.gameObject.SetActive(false);
            qnaPanelMain.gameObject.SetActive(false);
            FailPageUI.gameObject.SetActive(true);
            end = true;
            ChangeText changeText = gameObject.GetComponent<ChangeText>();
            if (changeText != null)
            {
                changeText.InitialFont();
            }
        }

        StopTimer();
    }
    private void UpdateTimerText(float elapsedTime)
    {
        string text = timeUIText + TimeUnit.getTimeUnit(elapsedTime);
        remainingTimeUI.text = text;
    }
    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            Debug.Log("Stopped");
        }
    }
    public void TryAgain()
    {
        timeRemaining = 0;
        scoreSO.reset();
        removeChild(endQContent, "result");
        end = false;
        qnaPanelMain.gameObject.SetActive(false);
        nextPage();
        EndPageUI.gameObject.SetActive(false);
        FailPageUI.gameObject.SetActive(false);
        StartPageUI.gameObject.SetActive(true);
        StopTimer();
        page = -1;
    }
    public void StartQuiz()
    {
        end = false;
        qnaPanelMain.gameObject.SetActive(true);
        nextPage();
        StartPageUI.gameObject.SetActive(false);
        startTime();
        remainingTimeUI.gameObject.SetActive(true);
    }

    public void getResult()
    {
        targetProgress = 0;
        StopAllCoroutines();
        progressBar.value = 0;
        removeChild(endQContent, "result");
        for (int i = 0; i < quizSO.questions.Length; i++)
        {
            GameObject resultObj = Instantiate(endQuestionPrefab, EndPageUI.transform.position, Quaternion.identity);
            resultObj.transform.SetParent(endQContent.transform);
            SelectorBehaviour targetComponent;
            if (resultObj.GetComponent<SelectorBehaviour>() != null)
            {
                targetComponent = resultObj.GetComponent<SelectorBehaviour>();
                targetComponent.setImg(quizSO.questions[i].answer == quizSO.questions[i].inputAnswer);
                targetComponent.setQNumber(i + 1);
            }
        }
        getScore();
        ChangeText changeText = gameObject.GetComponent<ChangeText>();
        if (changeText != null)
        {
            changeText.InitialFont();
        }
    }
    private void getScore()
    {
        scoreSO.calculateScore(quizSO.correctCount, timeRemaining, quizSO.timeLimit);
        correctCountUI.text = quizSO.correctCount.ToString();
        correctScoreUI.text = scoreSO.getCorrectScore().ToString();
        timeBonusUI.text = timeRemaining.ToString();
        timeScoreUI.text = "x " + scoreSO.getTimeBonusScore().ToString();
        totalScoreUI.text = scoreSO.getScore().ToString();
        getStars();
        LerpEllipseValue();
        GetProportionUI();
    }
    private void getStars()
    {
        int starCount = 0;
        float scorePercentage = 0;
        scorePercentage = ((float)scoreSO.getScore() / quizSO.fullStarsScoreReq) * 100f;
        if(scorePercentage < 75 && scorePercentage > 0)
        {
            starCount = 1;
        } else if(scorePercentage >= 75 && scorePercentage < 100)
        {
            starCount = 2;
        } else if(scorePercentage >= 100)
        {
            starCount = 3;
        } else
        {
            starCount = 0;
        }
        for(int i=0; i<starsUI.transform.childCount; i++)
        {
            Image imgComponent;
            imgComponent = starsUI.transform.GetChild(i).transform.GetComponent<Image>();
            if (i < starCount)
            {
                imgComponent.sprite = star;
            } else
            {
                imgComponent.sprite = grayStar;
            }
        }
        
    }
    public void LerpEllipseValue()
    {
        Image image = ScoreEllipseBar.GetComponent<Image>();
        float scorePercentage = 0;
        scorePercentage = (float)scoreSO.getScore() / quizSO.fullStarsScoreReq;
        LerpUtility.SmoothLerp(image, 0, scorePercentage, lerpSpeed);
    }
    private void GetProportionUI()
    {
        ScoreProportionUI.text = (int)scoreSO.getScore() + "/" + (int)quizSO.fullStarsScoreReq;
    }
}
