using UnityEngine;
using System.Collections;

public class TurretAwareness : MonoBehaviour
{
    private TurretAI _turret;
    // Use this for initialization
    void Start()
    {
        _turret = GetComponentInParent<TurretAI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyTech") && !other.CompareTag("EnemyGuard") && !other.CompareTag("Player")) return;

        if (!_turret.ListOfObjectsInAwareness.Contains(other.gameObject))
        {
            _turret.ListOfObjectsInAwareness.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("EnemyTech") && !other.CompareTag("EnemyGuard") && !other.CompareTag("Player")) return;

        if (_turret.ListOfObjectsInAwareness.Contains(other.gameObject))
        {
            _turret.ListOfObjectsInAwareness.Remove(other.gameObject);
        }
    }
}
