using UnityEngine;
using System.Collections;

public class MainFrameGuard : MonoBehaviour
{
    public MainFrameEnemiesAround MainFrame;
    private EnemyGuardAI _thisEnemy;

    void OnEnable()
    {
        MainFrame.AddEnemy(gameObject);
    }

    void Start()
    {
        _thisEnemy = GetComponent<EnemyGuardAI>();
    }

    void Update()
    {
        if (_thisEnemy.HealthPoints <= 0)
        {
            MainFrame.RemoveEnemy(gameObject);
        }
    }
}
