using UnityEngine;
using System.Collections;

public class SpawnInWallCheker : MonoBehaviour
{
    private PlayerController _pc;
    private EnemyGuardAI _guard;

    void Start()
    {
        _pc = GetComponentInParent<PlayerController>();
        _guard = GetComponentInParent<EnemyGuardAI>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Computer") || other.CompareTag("Obstacle"))
        {
            if (IsInvoking("UpdateSpawnInWall"))
                CancelInvoke("UpdateSpawnInWall");

            if (_pc)
                _pc.SpawnInWall = true;
            else
                _guard.SpawnInWall = true;

            Invoke("UpdateSpawnInWall", Time.deltaTime + .05f);
        }
    }

    void UpdateSpawnInWall()
    {
        if (_pc)
            _pc.SpawnInWall = false;
        else
            _guard.SpawnInWall = false;
    }
}
