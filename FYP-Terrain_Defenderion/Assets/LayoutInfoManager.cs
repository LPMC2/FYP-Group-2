using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LayoutInfoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text m_title;
    [SerializeField] private Image m_infoImage;
    [SerializeField] private TMP_Text m_infoText;
    public TMP_Text Title { get { return m_title; } set { m_title = value; } }
    public Image InfoImg { get { return m_infoImage; } set { m_infoImage = value; } }
    public TMP_Text Description { get { return m_infoText; } set { m_infoText = value; } }
}
