using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorDownController : MonoBehaviour
{
    [SerializeField]
    private Enums.DoorLockType _lockType = Enums.DoorLockType.Unlocked;
    private Animator _animator;
    private bool _unlocked;
    private NavMeshObstacle _navObstacle;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _navObstacle = GetComponent<NavMeshObstacle>();
    }

    void Update()
    {
        if (_lockType.Equals(Enums.DoorLockType.Unlocked) || _lockType.Equals(Enums.DoorLockType.EnemyLock))
        {
            _navObstacle.enabled = false;

            _animator.SetBool("DoorsOpen", _unlocked);
            return;
        }

        if (_lockType.Equals(Enums.DoorLockType.Locked))
        {
            _animator.SetBool("DoorsOpen", false);
            _navObstacle.enabled = true;
        }
        else
        {
            _animator.SetBool("DoorsOpen", true);
            _navObstacle.enabled = false;
        }
        
    }

    public void SetLockType(Enums.DoorLockType type)
    {
        _lockType = type;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_lockType.Equals(Enums.DoorLockType.Unlocked))
        {
            if (other.CompareTag("Player") || other.CompareTag("EnemyGuard") || other.CompareTag("EnemyTech"))
                _unlocked = true;
        }
        else if (_lockType.Equals(Enums.DoorLockType.EnemyLock))
        {
            if (other.CompareTag("EnemyGuard") || other.CompareTag("EnemyTech"))
                _unlocked = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (_lockType.Equals(Enums.DoorLockType.Unlocked))
        {
            if (other.CompareTag("Player") || other.CompareTag("EnemyGuard") || other.CompareTag("EnemyTech"))
                _unlocked = false;
        }
        else if (_lockType.Equals(Enums.DoorLockType.EnemyLock))
        {
            if (other.CompareTag("EnemyGuard") || other.CompareTag("EnemyTech"))
                _unlocked = false;
        }
    }
}
