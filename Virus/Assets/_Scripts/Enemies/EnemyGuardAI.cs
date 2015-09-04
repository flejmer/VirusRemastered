using UnityEngine;
using System.Collections;

public class EnemyGuardAI : EnemySimpleAI
{
    public float RotationSpeed = 10;

    private Transform _target;
    private Animator _anim;

    private Enums.EnemyGuardStates _enemyState = Enums.EnemyGuardStates.Idle;

    void Start()
    {
        EnemyType = Enums.EnemyType.Guard;
        _anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        AI();
        Movement();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_target == null)
                _target = other.transform;

            RaycastHit hit;

            var direction = (_target.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out hit, 100.0f))
            {
                Debug.Log(hit.transform.tag);

                _enemyState = Enums.EnemyGuardStates.Shooting;
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting) || _enemyState.Equals(Enums.EnemyGuardStates.RunAway)) return;

        if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            var direction = (_target.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out hit, 100.0f))
            {
                Debug.Log(hit.transform.tag);

                _enemyState = Enums.EnemyGuardStates.Shooting;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemyState = Enums.EnemyGuardStates.Chase;
        }
    }

    void AI()
    {
        if (_enemyState.Equals(Enums.EnemyGuardStates.Idle))
        {

        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Chase))
        {
            Agent.Resume();
            Agent.SetDestination(_target.position);
            _anim.SetBool("Running", true);
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
        {
            Agent.Stop();
            _anim.SetBool("Running", false);
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.RunAway))
        {

        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Healing))
        {

        }
        else
        {

        }
    }

    void Movement()
    {
        // Rotation towards target after its reached
        if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
        }
    }
}
