using UnityEngine;
using System.Collections;

public class EnemySimpleAI : MonoBehaviour
{
    public Enums.EnemyType EnemyType = Enums.EnemyType.Guard;

    void Enable()
    {
        GameManager.AddEnemy(this);
    }

    // Update is called once per frame
    void Disable()
    {
        GameManager.RemoveEnemy(this);
    }
}
