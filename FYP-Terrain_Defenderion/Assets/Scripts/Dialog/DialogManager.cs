using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogManager : MonoBehaviour
{
    [SerializeField] private DialogSO m_DialogSO;
    [SerializeField] private int m_CurrentDialogID = -1;
    [SerializeField] private string m_CurrentDialog;
    [SerializeField] private TMP_Text m_displayText;
    [SerializeField] private GameObject m_TargetUIPrefab;
    private void Start()
    {
        NextPage();
    }
    public void NextPage()
    {
        if(m_CurrentDialogID >= m_DialogSO.DialogsText.Count-1) { return; }
        m_CurrentDialogID++;
        m_CurrentDialog = m_DialogSO.DialogsText[m_CurrentDialogID];
        m_displayText.text = m_CurrentDialog;
    }
    public void StartPage()
    {
        ResetPage();
        NextPage();
    }
    public void ResetPage()
    {
        m_CurrentDialogID= -1;
        m_CurrentDialog = "";
        m_displayText.text = "";
    }
}
