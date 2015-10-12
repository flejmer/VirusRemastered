using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public Animator Anim { get; private set; }

    // Use this for initialization
    void Start()
    {
        Anim = GetComponent<Animator>();
    }

    public void StartGame()
    {
        Application.LoadLevel("Game01");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }
}
