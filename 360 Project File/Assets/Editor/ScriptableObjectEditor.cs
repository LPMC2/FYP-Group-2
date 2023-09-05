using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(QuizSO))]
public class ScriptableObjectEditor : Editor
{
    //private ReorderableList columnList;

    //private void OnEnable()
    //{
    //    columnList = new ReorderableList(serializedObject, serializedObject.FindProperty("questionEng"), true, true, true, true);

    //    // Customize the display of elements
    //    columnList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
    //    {
    //        var questionElement = columnList.serializedProperty.GetArrayElementAtIndex(index);
    //        var answerProperty = serializedObject.FindProperty("answer");

    //        // Ensure the answer array has the same size as the question array
    //        if (answerProperty.arraySize != columnList.serializedProperty.arraySize)
    //        {
    //            answerProperty.arraySize = columnList.serializedProperty.arraySize;
    //        }

    //        var answerElement = answerProperty.GetArrayElementAtIndex(index);

    //        // Custom element name
    //        var elementName = "Question " + index;
    //        var answerName = "Answer";
    //        // Draw question field
    //        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), questionElement, new GUIContent(elementName));

    //        // Draw answer field with extra spacing
    //        var answerRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2, rect.width, EditorGUIUtility.singleLineHeight);
    //        EditorGUI.PropertyField(answerRect, answerElement, new GUIContent(answerName));

    //        // Comment for answer field

    //    };
    //    columnList.elementHeight = 40f;
    //    // Customize the header label
    //    columnList.drawHeaderCallback = (Rect rect) =>
    //    {
    //        EditorGUI.LabelField(rect, "Quiz - English Edition");
    //    };
    //}

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();

    //    // Display the ReorderableList
    //    columnList.DoLayoutList();

    //    serializedObject.ApplyModifiedProperties();
    //}
}
#endif