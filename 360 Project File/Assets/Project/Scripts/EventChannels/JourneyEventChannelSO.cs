using UnityEngine;

[CreateAssetMenu(menuName = "Events/Journey")]
public class JourneyEventChannelSO : ScriptableObject
{
    public EntryCompletedAction OnEntryCompleted;

    public LoadEntriesAction OnLoadEntries;
    public DestinationReached OnDestinationReached;
}

public delegate void EntryCompletedAction(JourneyEntrySO entry);

public delegate void LoadEntriesAction(JourneyEntrySO[] entries);
public delegate void DestinationReached(MapLandmark landmark);
