using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(BodyManager))]
public class BodyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        BodyManager bodyManager = (BodyManager)target;

        if (GUILayout.Button("Auto Assign Body Parts"))
        {
            bodyManager.AssignBody();
        }
        DrawDefaultInspector();

    }
}

#endif
public class BodyManager : MonoBehaviour
{
    public body bodyParts;
    [System.Serializable]
    public class body
    {
        [Header("-< Head >-")]
        public head head;
        [Header("-< Body >-")]
        public GameObject spineMain;
        public GameObject spineLowerChest;
        public GameObject spineUpperChest;
        public GameObject hips;
        [Header("-< Arms >-")]
        public arm leftArm;
        public arm rightArm;
        [Header("-< Legs >-")]
        public leg leftLeg;
        public leg rightLeg;
    }
    [System.Serializable]
    public class head
    {
        public GameObject neckObj;
        public GameObject headObj;
        public GameObject topHeadObj;
    }
    [System.Serializable]
    public class arm
    {
        public GameObject shoulderObj;
        public GameObject armObj;
        public GameObject foreArmObj;
        public hand hand;
    }
    [System.Serializable]
    public class hand
    {
        public GameObject handObj;
        public finger[] fingers = new finger[5];

        [System.Serializable]
        public class finger
        {
            public GameObject[] fingerjoints = new GameObject[3];
        }
    }
    [System.Serializable]
    public class leg
    {
        public GameObject upLegObj;
        public GameObject legObj;
        public GameObject footObj;
        public GameObject toeObj;
        public GameObject toeEndObj;
    }
    public void AssignBody()
    {
        AssignObject("hips", ref bodyParts.hips);
        AssignObject("Neck", ref bodyParts.head.neckObj);
        AssignObject("head", ref bodyParts.head.headObj);
        AssignObject("headtop", ref bodyParts.head.topHeadObj);
        AssignObject("Spine", ref bodyParts.spineMain);
        AssignObject("Spine1", ref bodyParts.spineLowerChest);
        AssignObject("Spine2", ref bodyParts.spineUpperChest);
        AssignObject("LeftShoulder", ref bodyParts.leftArm.shoulderObj);
        AssignObject("LeftArm", ref bodyParts.leftArm.armObj);
        AssignObject("LeftForeArm", ref bodyParts.leftArm.foreArmObj);
        AssignObject("LeftHand", ref bodyParts.leftArm.hand.handObj);
        AssignFingerObject("LeftHandIndex", ref bodyParts.leftArm.hand.fingers[0].fingerjoints);
        AssignFingerObject("LeftHandMiddle", ref bodyParts.leftArm.hand.fingers[1].fingerjoints);
        AssignFingerObject("LeftHandPinky", ref bodyParts.leftArm.hand.fingers[2].fingerjoints);
        AssignFingerObject("LeftHandRing", ref bodyParts.leftArm.hand.fingers[3].fingerjoints);
        AssignFingerObject("LeftHandThumb", ref bodyParts.leftArm.hand.fingers[4].fingerjoints);
        AssignObject("RightShoulder", ref bodyParts.rightArm.shoulderObj);
        AssignObject("RightArm", ref bodyParts.rightArm.armObj);
        AssignObject("RightForeArm", ref bodyParts.rightArm.foreArmObj);
        AssignObject("RightHand", ref bodyParts.rightArm.hand.handObj);
        AssignFingerObject("RightHandIndex", ref bodyParts.rightArm.hand.fingers[0].fingerjoints);
        AssignFingerObject("RightHandMiddle", ref bodyParts.rightArm.hand.fingers[1].fingerjoints);
        AssignFingerObject("RightHandPinky", ref bodyParts.rightArm.hand.fingers[2].fingerjoints);
        AssignFingerObject("RightHandRing", ref bodyParts.rightArm.hand.fingers[3].fingerjoints);
        AssignFingerObject("RightHandThumb", ref bodyParts.rightArm.hand.fingers[4].fingerjoints);
        AssignObject("LeftUpLeg", ref bodyParts.leftLeg.upLegObj);
        AssignObject("LeftLeg", ref bodyParts.leftLeg.legObj);
        AssignObject("LeftFoot", ref bodyParts.leftLeg.footObj);
        AssignObject("LeftToeBase", ref bodyParts.leftLeg.toeObj);
        AssignObject("LeftToe_End", ref bodyParts.leftLeg.toeEndObj);
        AssignObject("RightUpLeg", ref bodyParts.rightLeg.upLegObj);
        AssignObject("RightLeg", ref bodyParts.rightLeg.legObj);
        AssignObject("RightFoot", ref bodyParts.rightLeg.footObj);
        AssignObject("RightToeBase", ref bodyParts.rightLeg.toeObj);
        AssignObject("RightToe_End", ref bodyParts.rightLeg.toeEndObj);
        Debug.Log("Assign Successful!");
    }

    private void AssignObject(string searchName, ref GameObject targetObject)
    {
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);

        foreach (Transform childTransform in allTransforms)
        {
            if (childTransform.gameObject.name.ToLower().Contains(searchName.ToLower()))
            {
                targetObject = childTransform.gameObject;
                return;
            }
        }

        targetObject = null;
    }
    private void AssignFingerObject(string searchName, ref GameObject[] targetObject)
    {
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);

        foreach (Transform childTransform in allTransforms)
        {
            if (childTransform.gameObject.name.ToLower().Contains(searchName.ToLower()))
            {
                targetObject[0] = childTransform.GetChild(0).gameObject;
                targetObject[1] = childTransform.GetChild(0).GetChild(0).gameObject;
                targetObject[2] = childTransform.GetChild(0).GetChild(0).GetChild(0).gameObject;

                return;
            }
        }

        targetObject = null;
    }
}
