using UnityEngine;

public class MapManagerDebug : MonoBehaviour
{
    [SerializeField]
    private Map m_Map;
    [SerializeField]
    private MapLandmark m_StartPosition;

    private void Start()
    {
        NavigationManager navManager = NavigationManager.Instance;
        navManager.LoadMap(m_Map);
        navManager.TeleportTowards(navManager.GetSphericalHelper(m_StartPosition));
        Destroy(this);
    }
}
