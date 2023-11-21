using UnityEngine;

[CreateAssetMenu(menuName = "Events/Camera")]
public class CameraEventChannelSO : ScriptableObject
{
    public RotationSnapAction OnRotationSnap;
}

public delegate void RotationSnapAction(Vector3 eulerAngles);
