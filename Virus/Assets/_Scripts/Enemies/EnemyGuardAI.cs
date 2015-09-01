using UnityEngine;
using System.Collections;

public class EnemyGuardAI : EnemySimpleAI
{
    private Animator _anim;

    private bool _inMotion;

    void Start()
    {
        EnemyType = Enums.EnemyType.Guard;
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Movement();
        Animations();
    }

    void Movement()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Target.position = hit.point;
                Agent.SetDestination(Target.position);
            }
        }
    }

    void Animations()
    {
        if (Agent.velocity.magnitude > 0)
        {
            _anim.SetBool("Running", true);
        }
        else
        {
            _anim.SetBool("Running", false);
        }

    }
}
