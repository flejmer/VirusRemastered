using UnityEngine;
using System.Collections;

public class EnemyTechAI : EnemySimpleAI
{
    private bool test;
    private Animator _anim;

    private Enums.EnemyTechStates _enemyState = Enums.EnemyTechStates.Idle;

    private Transform _targetComputer;

    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        AI();
        Movement();
    }

    public void SetTargetComputer(Transform computer)
    {
        _targetComputer = computer;
    }

    void AI()
    {
        if (_enemyState.Equals(Enums.EnemyTechStates.Idle))
        {
            
        }
        else if(_enemyState.Equals(Enums.EnemyTechStates.RunToComputer))
        {
            _anim.SetBool("Running", true);
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.Hack))
        {
            _anim.SetBool("Running", false);
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.RunForYourLife))
        {

        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.Healing))
        {

        }
        else
        {
            
        }
    }

    void Movement()
    {
        if (_enemyState.Equals(Enums.EnemyTechStates.Hack))
        {
            Vector3 direction = (_targetComputer.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }
    }


}
