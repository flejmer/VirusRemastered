using UnityEngine;
using System.Collections;

public class EnemySimpleAI : MonoBehaviour
{
    public Enums.EnemyType EnemyType = Enums.EnemyType.Guard;
    public Transform Target;
    public Camera cam;

    protected NavMeshAgent Agent;

    private bool _stopped;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    void Enable()
    {
        GameManager.AddEnemy(this);
    }

    void Disable()
    {
        GameManager.RemoveEnemy(this);
    }
}
