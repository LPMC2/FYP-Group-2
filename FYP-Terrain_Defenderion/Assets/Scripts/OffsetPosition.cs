//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor.UIElements;
//using UnityEngine.UIElements;
//#if UNITY_EDITOR
//using UnityEditor;
//[CustomPropertyDrawer(typeof(OffsetPosition), true)]
//[CanEditMultipleObjects]
//public class OffsetPositionEditor : PropertyDrawer
//{
//    public override VisualElement CreatePropertyGUI(SerializedProperty property)
//    {
//        // Create property container element.
//        var container = new VisualElement();

//        return container;
//    }
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);

//        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        // Indent the content
//        var contentPosition = EditorGUI.IndentedRect(position);

//        // Create a SerializedObject from the target object
//        var serializedObject = new SerializedObject(property.serializedObject.targetObject);

//        // Get the serialized properties
//        var initialPosProp = serializedObject.FindProperty("m_initialPos");
//        var offsetPosProp = serializedObject.FindProperty("m_offsetPos");

//        // Display the m_initialPos and m_offsetPos fields
//        EditorGUI.PropertyField(contentPosition, initialPosProp);
//        contentPosition.y += EditorGUIUtility.singleLineHeight;
//        EditorGUI.PropertyField(contentPosition, offsetPosProp);
//        contentPosition.y += EditorGUIUtility.singleLineHeight;

//        // Apply modifications to the serialized object
//        serializedObject.ApplyModifiedProperties();

//        // Add a button to store the current position as the offset position
//        if (GUI.Button(contentPosition, "Store Current Position as Offset"))
//        {
//            GameObject selectedObject = Selection.activeGameObject;
//            if (selectedObject != null)
//            {
//                var offsetPosition = property.serializedObject.targetObject as OffsetManager;
//                offsetPosition.SetOffsetPosition(selectedObject);
//            }
//        }
//        contentPosition.y += EditorGUIUtility.singleLineHeight;

//        // Add a button to reset the position to the initial position
//        if (GUI.Button(contentPosition, "Reset Offset Position"))
//        {
//            GameObject selectedObject = Selection.activeGameObject;
//            if (selectedObject != null)
//            {
//                var offsetPosition = property.serializedObject.targetObject as OffsetManager;
//                offsetPosition.ResetValue(selectedObject);
//            }
//        }
//        contentPosition.y += EditorGUIUtility.singleLineHeight;

//        if (GUI.Button(contentPosition, "Reset GameObject Position"))
//        {
//            GameObject selectedObject = Selection.activeGameObject;
//            if (selectedObject != null)
//            {
//                var offsetPosition = property.serializedObject.targetObject as OffsetManager;
//                offsetPosition.ResetPosition(selectedObject);
//            }
//        }

//        EditorGUI.EndProperty();
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        // Calculate the height based on the number of lines + button heights
//        int lines = 2; // Number of serialized fields
//        int buttons = 3; // Number of buttons
//        float lineHeight = EditorGUIUtility.singleLineHeight;
//        float buttonHeight = lineHeight + EditorGUIUtility.standardVerticalSpacing;
//        return lines * lineHeight + buttons * buttonHeight;
//    }
//}
//#endif

//[System.Serializable]
//public class OffsetPosition
//{
//    [SerializeField]
//    private Vector3 m_initialPos = Vector3.zero;
//    [SerializeField]
//    private Vector3 m_offsetPos;
//    public Vector3 InitialPos { get { return m_initialPos; } set { m_initialPos = value; } }
//    public Vector3 OffsetPos { get { return m_offsetPos; } set { m_offsetPos = value; } }
//}
