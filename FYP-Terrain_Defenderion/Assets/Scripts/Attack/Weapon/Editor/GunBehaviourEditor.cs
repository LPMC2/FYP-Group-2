using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GunBehaviour))]
public class GunBehaviourEditor : WeaponBehaviourEditor
{
    // Custom editor code specific to GunBehaviour
}
#endif