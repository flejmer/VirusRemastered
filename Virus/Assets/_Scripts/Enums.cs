using UnityEngine;
using System.Collections;


public class Enums : MonoBehaviour
{
    public enum BuffType
    {
        None,
        Damage,
        Energy,
        Speed
    }

    public enum AnimType
    {
        FromOriginToDestination,
        FromDestinationToOrigin
    }

    public enum EnemyType
    {
        Guard,
        Tech
    }

    public enum DoorLockType
    {
        Locked,
        EnemyLock,
        Unlocked,
        OpenForever
    }
}


