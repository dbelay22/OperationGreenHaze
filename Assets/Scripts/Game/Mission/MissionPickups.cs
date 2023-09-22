using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPickups : MonoBehaviour
{
    [SerializeField] GameObject[] _pickups;

    Dictionary<string, bool> _objectivesCompleted;

    int _objectivesCompletedCount;

    #region Instance

    private static MissionPickups _instance;

    public static MissionPickups Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        ObjectivesStart();
    }

    void ObjectivesStart()
    {
        _objectivesCompleted = new Dictionary<string, bool>();
        
        foreach (GameObject pickup in _pickups)
        {
            _objectivesCompleted.Add(pickup.name, false);
        }
        
        _objectivesCompletedCount = 0;
    }
    
    public void OnMissionItemPickup(GameObject pickup)
    {
        //Debug.Log($"[MissionPickups] (OnMissionItemPickup) pickup: {pickup.name}");

        bool pickupFound = _objectivesCompleted.TryGetValue(pickup.name, out bool completedStatus);

        if (pickupFound == true)
        {
            if (completedStatus == true)
            {
                Debug.LogError("[MissionPickups] (OnMissionItemPickup) MISSION PICKUP ALREADY COMPLETED ?????");
                return;
            }
            else
            {
                // mark complete
                _objectivesCompleted[pickup.name] = true;

                if (IsMissionDone())
                {
                    Game.Instance.ReportAllMissionPickupsCollected();
                }
                else
                {
                    _objectivesCompletedCount++;

                    GameUI.Instance.ShowInGameMessage($"Item {_objectivesCompletedCount}/{_objectivesCompleted.Count} is safe, find the next one.", 4f);
                }
            }
        }
    }

    bool IsMissionDone()
    {
        bool allCompleted = true;
        
        foreach (KeyValuePair<string, bool> mission in _objectivesCompleted)
        {
            if (mission.Value == false) {
                allCompleted = false;
                break;
            }
        }
        
        return allCompleted;
    }

}
