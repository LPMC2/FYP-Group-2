using UnityEngine;

[CreateAssetMenu(menuName = "Events/Navigation")]
public class NavigationEventChannelSO : ScriptableObject
{
    public NavigationStartedAction OnNavigationStarted;
    public NavigationFinishedAction OnNavigationFinished;
    public MapFloorAddedAction OnMapFloorAdded;
    public MapFloorChangedAction OnMapFloorChanged;
    public SphericalChangedAction OnSphericalChanged;

    public LoadMapAction OnLoadMap;
    public NavigateAction OnNavigate;
}

public delegate void NavigationStartedAction(NavigationManager.NavigationMode mode);
public delegate void NavigationFinishedAction(NavigationManager.NavigationMode mode);
public delegate void MapFloorAddedAction(MapFloor mapFloor);
public delegate void MapFloorChangedAction(MapFloor from, MapFloor to, NavigationManager.NavigationMode mode);
public delegate void SphericalChangedAction(SphericalHelper from, SphericalHelper to, NavigationManager.NavigationMode mode);

public delegate void LoadMapAction(Map map, MapLandmark startPoint, Vector3 startRotation);
public delegate void NavigateAction(SphericalHelper destination, NavigationManager.NavigationMode mode);
