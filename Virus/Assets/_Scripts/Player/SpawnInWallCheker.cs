using UnityEngine;
using System.Collections;

public class SpawnInWallCheker : MonoBehaviour
{
    private PlayerController _pc;

    void Start()
    {
        _pc = GetComponentInParent<PlayerController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Computer") || other.CompareTag("Obstacle"))
        {
            if (IsInvoking("UpdateSpawnInWall"))
                CancelInvoke("UpdateSpawnInWall");

            _pc.SpawnInWall = true;

            Invoke("UpdateSpawnInWall", Time.deltaTime + .05f);
        }
    }

    void UpdateSpawnInWall()
    {
        _pc.SpawnInWall = false;
    }
}
