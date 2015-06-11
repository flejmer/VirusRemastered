using UnityEngine;
using System.Collections;

public class EnemySimpleAI : MonoBehaviour
{
    public Enums.EnemyType EnemyType = Enums.EnemyType.Guard;

    void Enable()
    {
        GameManager.AddEnemy(this);
    }

    void Disable()
    {
        GameManager.RemoveEnemy(this);
    }
}
