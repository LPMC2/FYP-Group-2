using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuizAnsUIManager : MonoBehaviour
{
    [SerializeField]
    private Vector2 PictureSpacing = new Vector2(279.2f, 251.23f);
    [SerializeField] private int pictureConstraintCount = 2;
    [SerializeField]
    private Vector2 TextSpacing = new Vector2(279.2f, 16.0f);
    [SerializeField] private int textConstraintCount = 1;
    private GridLayoutGroup gridLayout;
    private QuizUIManager uiManager;
    private void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        if(gridLayout == null)
        {
            gridLayout = gameObject.AddComponent<GridLayoutGroup>();
        }
        GameObject ManagerObject = GameObject.FindGameObjectWithTag("Manager");
        uiManager = ManagerObject.GetComponent<QuizUIManager>();
    }


    [SerializeField]
    private AnsType answerType = AnsType.Text;
    public AnsType AnswerType 
    { 
        set 
        {
            answerType = value;
            OnAnswerTypeChanged();
        }
    }

    private AnsType preAnswerType = AnsType.Text;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (answerType != preAnswerType)
        {
            preAnswerType = answerType;
            OnAnswerTypeChanged();
        }
    }
#endif
    private void OnAnswerTypeChanged()
    {
        if(gridLayout == null) { return; }
        switch(answerType)
        {
            case AnsType.Text:
                gridLayout.spacing = TextSpacing;
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = textConstraintCount;
                if(uiManager != null)
                {
                    uiManager.AnswerUIObject = uiManager.TextUIObject;
                }
                break;
            case AnsType.Picture:
                gridLayout.spacing = PictureSpacing;
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                gridLayout.constraintCount = pictureConstraintCount;
                if (uiManager != null)
                {
                    uiManager.AnswerUIObject = uiManager.PictureUIObject;
                }
                break;

            default:
                Debug.LogWarning("Type: " + answerType + "Not Yet Developed / Not Exist");
                break;
        }
    }
}
public enum AnsType
{
    Text,
    Picture,
    Interactive
}