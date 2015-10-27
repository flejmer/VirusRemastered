using UnityEngine;
using System.Collections;

public class MainFrameGuard : MonoBehaviour
{
    public MainFrameEnemiesAround MainFrame;

    void OnEnable()
    {
        MainFrame.AddEnemy(gameObject);
    }

    void OnDisable()
    {
        MainFrame.RemoveEnemy(gameObject);
    }
}
