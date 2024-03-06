using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName ="Dialog Data", menuName = "Dialog/Create new Dialog")]
public class DialogSO :ScriptableObject
{
    [SerializeField] private TMP_FontAsset m_Font;
    [TextArea]
    [SerializeField] private List<string> m_dialogText = new List<string>();
    public TMP_FontAsset Font { get { return m_Font; } }
    public List<string> DialogsText { get { return m_dialogText; } }
}
