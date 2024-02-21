using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ObjectAnimationCoroutineBehaviour<>), true)]
public class ObjectAnimationCoroutineEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var behaviour = target as IObjectAnimationCoroutineBehaviour;

        if (GUILayout.Button("Test Animation"))
        {
            behaviour.StartAnimation();
        }
    }
}
#endif
