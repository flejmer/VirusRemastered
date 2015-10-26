using UnityEngine;
using System.Collections;

public class DeadMenuController : MonoBehaviour
{
    public void ExitToMenu()
    {
        GUIController.DeadScreenDeactivate();
        GUIController.ToMenu();
    }

    public void RestartLevel()
    {
        GUIController.DeadScreenDeactivate();
        GUIController.Restart();
    }
}
