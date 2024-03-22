using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TrackerResultManager : MonoBehaviour
{
    [SerializeField] TMP_Text m_playerStatsInfo;
    [SerializeField] TMP_Text m_mobStatsInfo;
    [SerializeField] TMP_Text m_structureStatsInfo;
    private BehaviourTracker behaviourTracker;
   public void ShowInfo()
    {
        behaviourTracker = BehaviourTracker.Singleton;
        m_playerStatsInfo.text = showPlayerStats();
        m_mobStatsInfo.text = showMobStats();
        m_structureStatsInfo.text = showStructureStats();
    }

    //Hardcode time lmfao XD
    private string showPlayerStats()
    {
        //Gather Player Data
        int playerDeaths = behaviourTracker.TrackerData.GetTrackedDeathAmount(LayerMask.NameToLayer("Player"));
        string info = "";
        info += "Player:\n" +
                "Damage Dealt: " + behaviourTracker.TrackerData.GetTotalTrackedDamage() + "\n" +
                "Deaths: " + playerDeaths;

        return info;
    }
    private string showMobStats()
    {
        string info = "";
        info += "Mobs: \n" +
            "Total Killed Count: " + behaviourTracker.TrackerData.GetTrackedDeathAmount(LayerMask.NameToLayer("Enemy"));
        return info;
    }
    private string showStructureStats()
    {
        string info = "";
        info += "Structures: \n" +
            "Built: " + behaviourTracker.TrackerData.GetTrackedPlacedStructure() +"\n" +
            "Destroyed: " + behaviourTracker.TrackerData.GetTrackedDeathAmount(LayerMask.NameToLayer("Defense"));
        return info;
    }
}
