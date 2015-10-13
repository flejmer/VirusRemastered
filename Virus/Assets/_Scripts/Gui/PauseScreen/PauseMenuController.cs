using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    public void ResumeGame()
    {
        GUIController.PauseScreenDeactivate();
    }

    public void ExitToMenu()
    {
        GUIController.PauseScreenDeactivate();
        GUIController.ToMenu();
    }
}
