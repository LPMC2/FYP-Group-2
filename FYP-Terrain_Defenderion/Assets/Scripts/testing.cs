using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class testing : MonoBehaviour
{
    [SerializeField] 
    [Header("Quiz Question")]
    private QuizSO quizScriptableObject;
    [SerializeField] TMP_Text Target;
    [SerializeField] string Type;
    void Start()
    {
        for (int i = 0; i < quizScriptableObject.questions.Length; i++)
        {
            Target.text += quizScriptableObject.GetQuestion(i, Type);
            Target.text += "\n";
        }
    }
}
