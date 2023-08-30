using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    [SerializeField] GameObject[] _missionPickups;

    Dictionary<string, bool> _missionsCompleted;

    #region Instance

    private static MissionSystem _instance;

    public static MissionSystem Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        MissionsCompletedStart();
    }

    void MissionsCompletedStart()
    {
        _missionsCompleted = new Dictionary<string, bool>();
        foreach (GameObject pickup in _missionPickups)
        {
            _missionsCompleted.Add(pickup.name, false);
        }
    }
    
    public void OnMissionItemPickup(GameObject pickup)
    {
        Debug.Log($"[MissionSystem] (OnMissionItemPickup) pickup: {pickup.name}");

        bool completedStatus = false;

        bool pickupFound = _missionsCompleted.TryGetValue(pickup.name, out completedStatus);

        if (pickupFound == true)
        {
            if (completedStatus == true)
            {
                Debug.LogError("[MissionSystem] (OnMissionItemPickup) MISSION PICKUP ALREADY COMPLETED ?????");
                return;
            }
            else
            {
                // mark complete
                _missionsCompleted[pickup.name] = true;

                if (IsAllMissionsCompleted())
                {
                    Game.Instance.ReportAllMissionPickupsCompleted();
                }
            }
        }
    }

    bool IsAllMissionsCompleted()
    {
        bool allCompleted = true;
        
        foreach (KeyValuePair<string, bool> mission in _missionsCompleted)
        {
            if (mission.Value == false) {
                allCompleted = false;
                break;
            }
        }
        
        return allCompleted;
    }

}
