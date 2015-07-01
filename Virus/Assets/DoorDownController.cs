using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorDownController : MonoBehaviour
{
    private bool open;
    public Enums.DoorLockType lockType = Enums.DoorLockType.EnemyLock;

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetBool("DoorsOpen", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetBool("DoorsOpen", false);
        }
    }
}
