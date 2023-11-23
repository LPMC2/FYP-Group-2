using UnityEngine;

[CreateAssetMenu(menuName = "Events/Navigation Point UI")]
public class NavigationPointUIEventChannelSO : ScriptableObject
{
    public DisplayUnlockJourneyEntryAction OnDisplayUnlockJourneyEntry;

    public FadeUIAction OnFadeUI;
    public FadeOverlayAction OnFadeOverlay;
}

public delegate void DisplayUnlockJourneyEntryAction(JourneyEntrySO entry);

public delegate float FadeUIAction(bool fadeIn);
public delegate float FadeOverlayAction(bool fadeIn);
