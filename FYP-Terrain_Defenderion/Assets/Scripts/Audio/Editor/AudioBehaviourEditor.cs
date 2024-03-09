using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/*
#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(AudioBehaviour))]
public class AudioBehaviourDrawer : PropertyDrawer
{
    float offsetX = -120f;
    float rectWidthMultiplyer = 1.5f;
    bool isOpened = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate the height for each property
        float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        Rect audioSORect = new Rect(position.x+offsetX, position.y+20, position.width*rectWidthMultiplyer, lineHeight);
        Rect audioListRect = new Rect(position.x+offsetX, position.y+25 + lineHeight, position.width*rectWidthMultiplyer, (position.height - lineHeight)*2);

        // Get the serialized properties
        SerializedProperty audioSOProperty = property.FindPropertyRelative("m_audioSO");
        SerializedProperty audioListProperty = property.FindPropertyRelative("audioList");

        // Draw the AudioSO field
        EditorGUI.PropertyField(audioSORect, audioSOProperty);

        // Draw the audioList field
        EditorGUI.PropertyField(audioListRect, audioListProperty, true);
        position.y += lineHeight * 2;

        // Draw buttons
        Rect buttonRect = new Rect(position.x*1.425f+offsetX, position.y-20, position.width/3f  , lineHeight);
        string txt = "";
        if(isOpened)
        {
            txt = "< Data";
        } else
        {
            txt = "> Data";
        }
        

        if (GUI.Button(buttonRect, txt))
        {
            Debug.Log("Test:" + audioSOProperty.objectReferenceValue.GetType());
            if (isOpened && audioSOProperty.objectReferenceValue != null)
            {
               
                AudioSO audioSO = (AudioSO)audioSOProperty.objectReferenceValue;
                GUIStyle boxStyle = GUI.skin.box;
                Rect boxRect = new Rect(position.x + offsetX, position.y, position.width * rectWidthMultiplyer, lineHeight * audioSO.Audios.Count);
                GUI.Box(boxRect, GUIContent.none, boxStyle);

                for (int i = 0; i < audioSO.Audios.Count; i++)
                {
                    Rect labelRect = new Rect(boxRect.x, boxRect.y + lineHeight * i, boxRect.width, lineHeight);
                    GUI.Label(labelRect, $"{i + 1} - {audioSO.Audios[i].Name}");
                }
            }
            isOpened = !isOpened;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty audioListProperty = property.FindPropertyRelative("audioList");
        float audioListHeight = EditorGUI.GetPropertyHeight(audioListProperty, true);
        return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + audioListHeight;
    }
}
#endif
*/