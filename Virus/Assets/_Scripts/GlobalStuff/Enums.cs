﻿using UnityEngine;
using System.Collections;


public class Enums : MonoBehaviour
{
    public enum Popusp
    {
        Text,
        HackingInfo,
        LaserUnlocked,
        MindControlUnlocked,
        HologramUnlocked,
        SlowMotionUnlocked,
        ShieldUnlocked
    }

    public enum PlayerStates
    {
        RealWorld,
        MindControlling,
        Dead
    }

    public enum GameStates
    {
        MainMenu,
        GamePlay
    }

    public enum InGameStates
    {
        Normal,
        InitTutorial,
        Pause,
    }

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
        Hologram,
        Shield,
        SlowMotion
    }

    public enum InteractionNodes
    {
        Door,
        Extinguisher,
        Turret,
        Data
    }
}


