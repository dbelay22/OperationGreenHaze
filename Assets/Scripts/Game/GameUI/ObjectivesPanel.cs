using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivesPanel : MonoBehaviour
{
    public readonly int KILL_ENEMIES_IDX = 0;
    public readonly int PICKUP_DATA_IDX = 1;
    public readonly int FIND_EXIT_IDX = 2;

    [SerializeField] TMP_Text _pickupCountText;

    [SerializeField] ObjectiveIcons[] _objectiveIcons;

    [Serializable]
    private class ObjectiveIcons
    {
        public GameObject _iconPending;
        public GameObject _iconCompleted;
    }

    #region Instance

    private static ObjectivesPanel _instance;


    public static ObjectivesPanel Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion
        
    void Start()
    {
        SetAllPending();
    }

    void SetAllPending()
    {
        foreach (ObjectiveIcons icons in _objectiveIcons)
        {
            icons._iconCompleted.SetActive(false);
            icons._iconPending.SetActive(true);
        }
    }

    public void SetKillemAllComplete()
    {
        SetObjectiveComplete(_objectiveIcons[KILL_ENEMIES_IDX]);
    }

    public void SetPickupDataPartialComplete(int partialCount, int total)
    {
        _pickupCountText.text = $"{partialCount}/{total}";
    }

    public void SetPickupDataComplete()
    {
        SetObjectiveComplete(_objectiveIcons[PICKUP_DATA_IDX]);
    }

    public void SetFindExitComplete()
    {
        SetObjectiveComplete(_objectiveIcons[FIND_EXIT_IDX]);
    }

    void SetObjectiveComplete(ObjectiveIcons objective)
    {
        objective._iconPending.SetActive(false);
        objective._iconCompleted.SetActive(true);
    }

}
