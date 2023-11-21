using UnityEngine;

[CreateAssetMenu(menuName = "Events/Loading Progress")]
public class LoadingProgressEventChannelSO : ScriptableObject
{
    public FadeAction OnFade;
}

public delegate float FadeAction(bool fadeOut);
