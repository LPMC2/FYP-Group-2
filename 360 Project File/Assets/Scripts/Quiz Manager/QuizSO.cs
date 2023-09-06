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
    public string language;
    public string GetQuestion(int index, string language)
    {
        return questions[index].question.FirstOrDefault(q => q.language == language)?.text;
    }
    public string[] GetAnswers(int page, string language)
    {
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
        [Header("Input index of options as the answer.")] public int answer;
        public LocalizableString[] question;
        public LocalizableString[] options;
        public LocalizableString[] explaination;
        public Score score;
        public int inputAnswer;
        //CorrectCount = -1 -> Answer Unselected, CorrectCount >= 0 -> Answer Selected
    }

    [System.Serializable]
    public class LocalizableString
    {
        public string language;
        public string text;
        public int id;
    }
    public void checkAns()
    {
        correctCount = 0;
        for (int i = 0; i < questions.Length;i++)
        {

            for (int j = 0; j < questions[i].options.Length; j++)
            {
                if (questions[i].options[j].language == language && questions[i].options[j].id == questions[i].inputAnswer)
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
    public string getLanText(LocalizableString[] stringArr, string lang)
    {
        for(int i =0; i<stringArr.Length; i++)
        {
            if (stringArr[i].language == lang)
            {
                return stringArr[i].text;
            }
        }
        return "";
    }
    public bool checkSingleAns(int page)
    {
        for (int j = 0; j < questions[page].options.Length; j++)
        {
            if (questions[page].options[j].language == language && questions[page].options[j].id == questions[page].inputAnswer)
            {
                if (questions[page].answer == questions[page].inputAnswer)
                {
                    correctCount++;
                    return true;
                } else
                {
                    return false;
                }
                
            }

        }
        return false;
    }
    public void GetAns(int page,string lang)
    {
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
