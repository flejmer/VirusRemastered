using UnityEngine;
using System.Collections;

public class WinMenuController : MonoBehaviour
{
    public void ExitToMenu()
    {
        GUIController.WinScreenDeactivate();
        GUIController.ToMenu();
    }

    public void RestartLevel()
    {
        GUIController.WinScreenDeactivate();
        GUIController.Restart();
    }
}
