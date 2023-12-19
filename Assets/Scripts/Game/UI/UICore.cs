using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICore : MonoBehaviour
{  

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

}
