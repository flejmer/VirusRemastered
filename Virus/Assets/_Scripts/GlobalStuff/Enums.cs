using UnityEngine;
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
        Dead,
        PlayerControlled
    }

    public enum EnemyTechStates
    {
        Idle,
        RunToComputer,
        RunForYourLife,
        Healing,
        Hack,
        Dead,
        PlayerControlled
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

    public enum Abilities
    {
        Laser,
        MindControl,
        Blast,
        Hologram,
    }

    public enum InteractionNodes
    {
        Door,
        Extinguisher,
        Turret,
        Data
    }
}


