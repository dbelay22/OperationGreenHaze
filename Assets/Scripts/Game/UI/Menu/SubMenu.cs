using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu : MenuBase
{
    [SerializeField] GameObject _mainMenu;

    void Update()
    {
        // [Esc] Back to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            _mainMenu.SetActive(true);
        }
    }
}
