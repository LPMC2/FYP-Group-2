using UnityEngine;

[CreateAssetMenu(menuName = "Events/Journey")]
public class JourneyEventChannelSO : ScriptableObject
{
    public EntryCompletedAction OnEntryCompleted;

    public LoadEntriesAction OnLoadEntries;
}

public delegate void EntryCompletedAction(JourneyEntrySO entry);

public delegate void LoadEntriesAction(JourneyEntrySO[] entries);
