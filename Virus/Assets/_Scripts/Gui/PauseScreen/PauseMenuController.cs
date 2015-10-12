using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    public void ResumeGame()
    {
        GUIController.PauseScreenDeactivate();
        Time.timeScale = 1;
    }

    public void ExitToMenu()
    {
        GUIController.PauseScreenDeactivate();
        Time.timeScale = 1;

        Application.LoadLevel("Menu");
    }
}
