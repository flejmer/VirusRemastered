﻿using UnityEngine;
using System.Collections;


public class Enums : MonoBehaviour
{
    public enum EnemyGuardStates
    {
        Idle,
        Chase,
        Shooting,
        RunAway,
        Healing,
        Dead
    }

    public enum EnemyITStates
    {
        Idle,
        RunToComputer,
        RunForYourLife,
        Healing,
        Hack,
        Dead
    }

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


