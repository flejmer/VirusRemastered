using UnityEngine;
using System.Collections;

public class Stucker : MonoBehaviour
{
    public DoorDownController Door;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Door.SetLockType(Enums.DoorLockType.Locked);
        }
    }
}
