using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] MainMenu _mainMenu;

    public void OnSelect(BaseEventData eventData)
    {
        _mainMenu.OptionSelected();
    }

}
