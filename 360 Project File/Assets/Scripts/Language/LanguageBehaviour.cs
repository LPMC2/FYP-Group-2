using UnityEngine;
using UnityEngine.UI;

public class LanguageBehaviour : MonoBehaviour
{

    private Button myButton;
    [SerializeField] private Language changeLanguageType;
    [SerializeField] private GameObject targetObject;
    private void Awake()
    {
        myButton = gameObject.GetComponent<Button>();
    }
    private void Start()
    {
        myButton.onClick.AddListener(() => OnButtonClick(changeLanguageType));
    }

    private void OnButtonClick(Language enumValue)
    {
        if(targetObject != null)
        {
            QuizUIManager quizUIManager = targetObject.GetComponent<QuizUIManager>();
            if(quizUIManager != null)
            {
                quizUIManager.setLanguage(enumValue.ToString());
            }
        }
    }
}

public enum Language
{
    en,
    zh_HK,
    zh_CN
}