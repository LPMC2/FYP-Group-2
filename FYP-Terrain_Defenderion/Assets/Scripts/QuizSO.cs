using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizMenu", menuName ="ScriptableObjects/QuizSO")]
public class QuizSO : ScriptableObject
{
    public int id;
    public Question[] questions;

    public string GetQuestion(int index, string language)
    {
        return questions[index].question.FirstOrDefault(q => q.language == language)?.text;
    }

    [System.Serializable]
    public class Question
    {
        public LocalizableString[] question;
        public string[] answers;
    }

    [System.Serializable]
    public class LocalizableString
    {
        public string language;
        public string text;
    }
}
