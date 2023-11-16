using System.Collections;
using UnityEngine;

public class MapManagerDebug : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private Map m_Map;
    [SerializeField]
    private MapLandmark m_StartPosition;

    private IEnumerator Start()
    {
        MapManager mapManager = null;
        NavigationManager navManager = null;
        yield return new WaitUntil(() =>
        {
            mapManager = MapManager.Instance;
            navManager = NavigationManager.Instance;
            return MapManager.Instance != null && navManager != null;
        });

        for (int i = 0; i < m_Map.Floors.Length; i++)
            mapManager.SpawnFloor(m_Map.Floors[i], i * 2.5f);
        navManager.LoadMap(m_Map);

        var startPoint = mapManager[m_StartPosition];
        navManager.TeleportTowards(startPoint);

        Destroy(this);
    }
#endif
}
