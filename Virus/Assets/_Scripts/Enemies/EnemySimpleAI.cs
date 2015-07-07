using UnityEngine;
using System.Collections;

public class EnemySimpleAI : MonoBehaviour
{
    public Enums.EnemyType EnemyType = Enums.EnemyType.Guard;
    public Transform Target;
    public Camera cam;

    private NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        _agent.SetDestination(Target.position);

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Target.position = hit.point;
            }
        }
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
