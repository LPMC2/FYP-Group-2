using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Void")]
public class VoidEventChannelSO : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        if (OnEventRaised == null)
            return;

        OnEventRaised.Invoke();
    }
}
