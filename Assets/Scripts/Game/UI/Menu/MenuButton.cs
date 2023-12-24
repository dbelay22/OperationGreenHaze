using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] MenuBase _menu;

    public void OnSelect(BaseEventData eventData)
    {
        _menu.OptionSelected();
    }

}
