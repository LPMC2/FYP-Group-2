using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizMenu", menuName ="ScriptableObjects/QuizSO")]
public class QuizSO : ScriptableObject
{
    [Header("Setup")]
    public int id;
    public int fullStarsScoreReq = 100;
    [Header("Unit: Second")]
    public float timeLimit = 1f;
    [Header("Note: for Language:\n- en -> English\n- zh_HK -> Traditional Chinese\n- zh_CN -> Simplified Chinese")]
    public int correctCount = -1;
    public Question[] questions;
    public Language language = Language.zh_HK;
    public void setLanguage(string languageType)
    {
        switch (languageType)
        {
            case "en":
                language = Language.en;
                break;
            case "zh_HK":
                language = Language.zh_HK;
                break;
            case "zh_CN":
                language = Language.zh_CN;
                break;
            default:
                Debug.LogWarning("Not Found!");
                break;
        }
    }
    public string GetQuestion(int index, Language language)
    {
        return questions[index].question.FirstOrDefault(q => q.language == language)?.text;
    }
    public string[] GetAnswers(int page, Language language)
    {
        //Get the list of answers from specific language
        string[] items = new string[1];
        int i = 0;
        int num = 0;
        foreach(Question question in questions)
        {
            if (page == num)
            {
                LocalizableString[] options = question.options;
                foreach (LocalizableString option in options)
                {
                    if (option.language == language)
                    {
                        if (i > 0)
                        {
                            items = addArray(items, option.text);
                        }
                        else
                        {
                            items[i] = option.text;
                        }
                        i++;
                    }
                }
            }
            num++;
        }
        return items;
    }
    public Sprite[] GetImgAnswers(int page, Language language)
    {
        //Get the list of answers from specific language
        Sprite[] items = new Sprite[1];
        int i = 0;
        int num = 0;
        foreach (Question question in questions)
        {
            if (page == num)
            {
                LocalizableString[] options = question.options;
                foreach (LocalizableString option in options)
                {
                    if (option.language == language)
                    {
                        if (i > 0)
                        {
                            items = arrayBehaviour.addArray<Sprite>(items);
                            items[i] = option.Image;
                        }
                        else
                        {
                            items[i] = option.Image;
                        }
                        i++;
                    }
                }
            }
            num++;
        }
        return items;
    }
    private string[] addArray(string[] arr, string newValue)
    {
        int originalsize = arr.Length;
        string[] newArray = new string[originalsize + 1];
        for(int i=0; i<originalsize+1; i++)
        {
            if(i < originalsize)
            {
                
                newArray[i] = arr[i];
            } else
            {
                newArray[i] = newValue;
            }
           
        }
        return newArray;
    }
    [System.Serializable]
    public class Question
    {
        [Header("Input index of options as the answer.")] public int[] answer;
        public LocalizableString[] question;
        public AnsType AnswerType = AnsType.Text;
        public LocalizableString[] options;
        public LocalizableString[] explaination;
        public Score score;
        public int[] inputAnswer;
        //CorrectCount = -1 -> Answer Unselected, CorrectCount >= 0 -> Answer Selected
    }

    [System.Serializable]
    public class LocalizableString
    {
        public Language language = Language.zh_HK;
        [TextArea]
        public string text;
        public int id;
        [Header("Only for Quiz UI - Answer Type is Picture")]
        public Sprite Image;
    }
    public void checkAns()
    {
        //Get the number of correct answers and store the value to correctCount variable
        correctCount = 0;
        for (int i = 0; i < questions.Length;i++)
        {

            for (int j = 0; j < questions[i].options.Length; j++)
            {
                if (questions[i].options[j].language == language && questions[i].options[j].id == questions[i].inputAnswer[0])
                {
                    if (questions[i].answer == questions[i].inputAnswer)
                    {
                        correctCount++;
                    }
                }

            }
        }
        Debug.Log("Correct Amount: " + correctCount);
    }
    public string getLanText(LocalizableString[] stringArr, Language lang)
    {
        //Get text from language
        for(int i =0; i<stringArr.Length; i++)
        {
            if (stringArr[i].language == lang)
            {
                return stringArr[i].text;
            }
        }
        return "";
    }
    public bool checkSingleAns(int page, bool isAddScore = true)
    {
        
        float correctAnswer = 0;
        //Find all options that have the corresponding language
        for (int j = 0; j < questions[page].options.Length; j++)
        {
            //Foreach input answer to check
            for (int i = 0; i < questions[page].inputAnswer.Length; i++)
            {
                if (questions[page].options[j].language == language && questions[page].options[j].id == questions[page].inputAnswer[i])
                {
                    bool answerCorrect = false;
                    for (int a = 0; a < questions[page].answer.Length; a++)
                    {
                        if (questions[page].answer[a] == questions[page].inputAnswer[i])
                        {

                            answerCorrect = true;
                            correctAnswer++;

                            break;
                        }
                       
                    }
                    if(!answerCorrect)
                    {
                        correctAnswer--;
                       
                    }

                }
            }

        }
        //Prevent correct Answer Count from having negative number
        correctAnswer = Mathf.Clamp(correctAnswer, 0, questions[page].inputAnswer.Length);
        //Detect if correctAnswer equals the answer count to count towards score
        if (correctAnswer == questions[page].answer.Length)
        {
            if (isAddScore)
            {
                if(correctCount == -1)
                {
                    correctCount = 0;
                }
                correctCount++;
            }
            return true;
        } else
        return false;
    }
    public void GetAns(int page, Language lang)
    {
        //Set the answer id to sort from language
        int index = 0;
        language = lang;
        for(int i=0; i< questions[page].options.Length; i++)
        {
            if(questions[page].options[i].language == lang)
            {
                
                questions[page].options[i].id = index;
                index++;
            }
        }
    }
}
