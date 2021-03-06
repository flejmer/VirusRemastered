﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MovementProperties
{
    public float MovementSpeed = 15;
    public float RotationSpeed = 10;
}

[System.Serializable]
public class ProjectilesProperties
{
    public BasicAttack BasicAttack;
    public LaserAttack LaserAttack;
    public MindControl MindControl;
    public Holog Hologram;
    public SlowMotion SlowMotion;
    public Shield Shield;
}

[System.Serializable]
public class BasicAttack
{
    public int Damage = 20;
    public GameObject Missile;
    public float MissileLifetime = 10;
    public GameObject Follower;
    public float EnergyCost = 0;
}

[System.Serializable]
public class LaserAttack
{
    public int Damage = 100;
    public GameObject Missile;
    public float MissileLifetime = 1;
    public float EnergyCost = 30;
}

[System.Serializable]
public class MindControl
{
    public LayerMask RayMask;
    public float RayLength = 20;
    public GameObject Missile;
    public float EnergyCost = 80;
}

[System.Serializable]
public class Holog
{
    public GameObject Missile;
    public float EnergyCost = 40;
}

[System.Serializable]
public class SlowMotion
{
    public float Duration = 3;
    public float EnergyCost = 40;
}

[System.Serializable]
public class Shield
{
    public float EnergyCost = 0;
}

public class PlayerController : MonoBehaviour
{
    public MovementProperties MovementProperties;
    public ProjectilesProperties ProjectilesProperties;

    public bool SpawnInWall { get; set; }

    public bool LaserUnlocked { get; private set; }
    public bool MindControlUnlocked { get; private set; }
    public bool ShieldUnlocked { get; private set; }
    public bool SlowMotionUnlocked { get; private set; }
    public bool HologramUnlocked { get; private set; }

    public bool ShieldActivated { get { return _shield.ShieldActivated; } }
    public bool IsMoving;

    private Rigidbody _rbody;

    private Transform _gun;
    private Transform _missileSpawn;

    private Animator _anim;
    private ShieldController _shield;

    private bool _hacking;

    private HealthEnergyManager _heManager;

    public Enums.PlayerStates PlayerState = Enums.PlayerStates.RealWorld;

    public bool GodMode;


    private float _prevMoveHori;
    private float _prevMoveVert;

    private bool _deadEvent;
    private RagdollController _ragdoll;
    private CapsuleCollider _collider;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rbody = GetComponent<Rigidbody>();
        _gun = transform.Find("Body/Gun");
        _missileSpawn = transform.Find("Body/Gun/missileSpawn");
        _anim = GetComponentInChildren<Animator>();
        _shield = GetComponentInChildren<ShieldController>();
        _heManager = GetComponent<HealthEnergyManager>();
        _ragdoll = GetComponentInChildren<RagdollController>();
        _collider = GetComponent<CapsuleCollider>();
    }

    public void StopAnimations()
    {
        _anim.speed = 0.0001f;
    }

    public void ResumeAnimations()
    {
        _anim.speed = 1;
    }

    public void HitPoint(Vector3 pos, Vector3 dir, float force, LayerMask mask)
    {
        if (_heManager.GetHealth() <= 0)
        {
            PlayerState = Enums.PlayerStates.Dead;
            _rbody.isKinematic = true;

            if (_ragdoll)
            {
                _collider.enabled = false;
                _ragdoll.ActivateRagdoll();

                RaycastHit hit;

                if (Physics.Raycast(pos - dir, dir, out hit, 3, mask))
                {
                    var rig = hit.transform.gameObject.GetComponent<Rigidbody>();

                    if (rig != null)
                        hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(dir * 5000);
                }

            }
        }
    }

    void OnEnable()
    {
        GameManager.SetPlayer(this);
    }

    void OnDisable()
    {
        GameManager.SetPlayer(null);
    }

    void Update()
    {
        if (!GameManager.Instance.InGameState.Equals(Enums.InGameStates.Normal)) return;
        if (GUIController.IsPopupActivated()) return;
        if (PlayerState.Equals(Enums.PlayerStates.MindControlling)) return;
        if (PlayerState.Equals(Enums.PlayerStates.Dead)) return;

        Shooting();
        Interaction();

        if (RealCyberManager.Instance.InCyberspace) return;

//        if (Input.GetKeyDown(KeyCode.N))
//        {
//            UnlockLaser();
//            UnlockHologram();
//            UnlockMindControl();
//            UnlockShield();
//            UnlockSlowMotion();
//
//            GodMode = true;
//        }

        if (!_hacking) return;

        if (GameManager.GetComputersInPlayerInterRange(this).Count > 0)
        {
            var count = GameManager.GetComputersInPlayerInterRange(this).Count(computer => computer.IsHackInProgress);

            if (count == 0)
                _anim.SetBool("Hacking", false);
        }
        else
        {
            _anim.SetBool("Hacking", false);
        }


    }

    void FixedUpdate()
    {
        if (PlayerState.Equals(Enums.PlayerStates.MindControlling)) return;
        if (PlayerState.Equals(Enums.PlayerStates.Dead)) return;

        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        if (vertical > 0 || horizontal > 0 || vertical < 0 || horizontal < 0)
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
        }

        MouseRotation();

        var dirLocal = Movement(vertical, horizontal);

        horizontal = dirLocal.x;
        vertical = dirLocal.z;

        if (_anim != null)
            Animations(vertical, horizontal);

    }

    public void UnlockLaser()
    {
        LaserUnlocked = true;
    }

    public void UnlockMindControl()
    {
        MindControlUnlocked = true;
    }

    public void UnlockShield()
    {
        ShieldUnlocked = true;
    }

    public void UnlockSlowMotion()
    {
        SlowMotionUnlocked = true;
    }

    public void UnlockHologram()
    {
        HologramUnlocked = true;
    }

    public void RemoveHealth(float amount)
    {
        if (!GodMode)
        {
            _heManager.RemoveHp(amount);

            if (!(_heManager.GetHealth() <= 0) || _deadEvent) return;

            _deadEvent = true;
            SoundManager.PlayChuckDeathSound(_audioSource);
            GameManager.Instance.PlayerDied();
            PlayerState = Enums.PlayerStates.Dead;
        }
    }

    public void RemoveEnergy(float amount)
    {
        if (!GodMode)
            _heManager.RemoveEnergy(amount);
    }

    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameManager.GetComputersInPlayerInterRange(this).Count > 0)
        {

            foreach (var computer in GameManager.GetComputersInPlayerInterRange(this).Where(computer => !computer.IsHacked))
            {
                if (computer.IsDehackInProgress)
                {
                    continue;
                }

                computer.StartHacking(this);
                _hacking = true;
                _anim.SetBool("Hacking", true);
            }

            foreach (var computer in GameManager.GetComputersInPlayerInterRange(this).Where(computer => computer.IsHacked))
            {
                RealCyberManager.GoToCyberspace(computer);
            }
        }

        if (Input.GetKeyDown(KeyCode.J) && GameManager.GetComputersInPlayerInterRange(this).Count > 0)
        {
            foreach (var computer in GameManager.GetComputersInPlayerInterRange(this))
            {
                computer.StopHacking();
                computer.StartDehacking();
                _hacking = true;
                _anim.SetBool("Hacking", true);
            }
        }
    }

    void MouseRotation()
    {
        var playerPlane = new Plane(Vector3.up, transform.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            var targetPoint = ray.GetPoint(hitdist);
            var targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            _rbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, MovementProperties.RotationSpeed * Time.deltaTime / Time.timeScale);

            //            var point = new Vector3(targetPoint.x, _missileSpawn.transform.position.y, targetPoint.z);
            //            _gun.transform.LookAt(point);



            targetRotation = Quaternion.LookRotation(targetPoint - _gun.position);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            var gunRotCopy = _gun.rotation;
            _gun.rotation = Quaternion.Slerp(_gun.rotation, targetRotation, MovementProperties.RotationSpeed * Time.deltaTime / Time.timeScale);

            var dot = Vector3.Dot(transform.forward, _gun.forward);

            if (dot < .95f)
                _gun.rotation = gunRotCopy;
        }
    }

    Vector3 Movement(float vertical, float horizontal)
    {
        if (Math.Abs(vertical) > 0.05f || Math.Abs(horizontal) > 0.05f)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical);


            transform.Translate(movement * MovementProperties.MovementSpeed * Time.deltaTime / Time.timeScale, Space.World);
            //            _rbody.MovePosition(transform.position + movement * MovementProperties.MovementSpeed * Time.deltaTime);

            Vector3 movRelative = transform.InverseTransformDirection(movement);

            return movRelative;
        }

        return Vector3.zero;
    }

    void Animations(float vertical, float horizontal)
    {
        //Math.Abs(vertical) < 0.05f && Math.Abs(horizontal) < 0.05f
        if (Math.Abs(vertical) < 0.05f && Math.Abs(horizontal) < 0.05f)
        {
            _anim.SetBool("NotMoving", true);
        }
        else
        {
            _anim.SetBool("NotMoving", false);
            _anim.SetFloat("Vertical", vertical);
            _anim.SetFloat("Horizontal", horizontal);
        }
    }

    void Shooting()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20, ProjectilesProperties.MindControl.RayMask))
        {

            if (Input.GetKeyDown(KeyCode.G) && !hit.transform.CompareTag("Obstacle") && HologramUnlocked && _heManager.GetEnergy() >= ProjectilesProperties.Hologram.EnergyCost)
            {
                var instance = (GameObject)Instantiate(ProjectilesProperties.Hologram.Missile, _missileSpawn.position, _missileSpawn.rotation);

                var script = instance.GetComponent<Hologram>();
                script.SetTarget(new Vector3(hit.point.x, 1, hit.point.z));

                RemoveEnergy(ProjectilesProperties.Hologram.EnergyCost);
            }

            if (hit.transform.CompareTag("EnemyGuard") || hit.transform.CompareTag("EnemyTech"))
            {
                var enemy = hit.transform.gameObject.GetComponent<EnemySimpleAI>();
                enemy.Highlight();

                if (Input.GetKeyDown(KeyCode.E) && MindControlUnlocked && _heManager.GetEnergy() >= ProjectilesProperties.MindControl.EnergyCost)
                {
                    SoundManager.PlayPlayerMindControlSound(_audioSource);

                    PlayerState = Enums.PlayerStates.MindControlling;

                    var instance = (GameObject)Instantiate(ProjectilesProperties.MindControl.Missile, _missileSpawn.position,
                        _missileSpawn.rotation);

                    var script = instance.GetComponent<MindControlProjectile>();
                    script.SetTarget(enemy);

                    var cam = Camera.main.gameObject.GetComponent<CameraFollow>();
                    cam.ChangeTarget(instance.transform);

                    RemoveEnergy(ProjectilesProperties.MindControl.EnergyCost);
                }
            }
        }


        var fireInput1 = Input.GetButtonDown("Fire1");
        var fireInput2 = Input.GetButtonDown("Fire2");

        if (SpawnInWall) return;

        if (fireInput1 || fireInput2)
        {
            if (_anim != null)
            {
                if ((fireInput2 && LaserUnlocked && _heManager.GetEnergy() >= ProjectilesProperties.LaserAttack.EnergyCost) || fireInput1 && _heManager.GetEnergy() >= ProjectilesProperties.BasicAttack.EnergyCost)
                {
                    if (IsInvoking("Aiming"))
                        CancelInvoke("Aiming");

                    _anim.SetBool("Aiming", true);
                    _anim.SetTrigger("FireGun");

                    Invoke("Aiming", 1.5f);
                }
            }

            if (fireInput1 && _heManager.GetEnergy() >= ProjectilesProperties.BasicAttack.EnergyCost)
            {
                SoundManager.PlayPlayerAttackSound(_audioSource);

                var instance = (GameObject)
                    Instantiate(ProjectilesProperties.BasicAttack.Missile, _missileSpawn.position,
                        _missileSpawn.rotation);

                instance.GetComponent<ProjectileDir>().WhoFired = gameObject;


                //                var follower =
                //                    (GameObject)
                //                        Instantiate(ProjectilesProperties.BasicAttack.Follower, _missileSpawn.transform.position,
                //                            _missileSpawn.transform.rotation);
                //
                //                follower.GetComponent<FollowPath>().TargetToFollow = instance;



                Destroy(instance, ProjectilesProperties.BasicAttack.MissileLifetime);
                RemoveEnergy(ProjectilesProperties.BasicAttack.EnergyCost);
            }

            if (fireInput2 && LaserUnlocked && _heManager.GetEnergy() >= ProjectilesProperties.LaserAttack.EnergyCost)
            {
                SoundManager.PlayPlayerAttack2Sound(_audioSource);

                var instance = (GameObject)Instantiate(ProjectilesProperties.LaserAttack.Missile, _missileSpawn.position, _missileSpawn.rotation);

                var script = instance.GetComponent<Laser>();
                script.MissleSpawnPoint = _missileSpawn;

                script.LifeTime = ProjectilesProperties.LaserAttack.MissileLifetime;
                script.FireLaser();

                Destroy(instance, ProjectilesProperties.LaserAttack.MissileLifetime);

                RemoveEnergy(ProjectilesProperties.LaserAttack.EnergyCost);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && ShieldUnlocked && _heManager.GetEnergy() >= ProjectilesProperties.Shield.EnergyCost)
        {
            if (!_shield.ShieldActivated)
            {
                _shield.ActivateShield();
                RemoveEnergy(ProjectilesProperties.Shield.EnergyCost);
            }
            else
            {
                _shield.DeactivateShield();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && SlowMotionUnlocked && _heManager.GetEnergy() >= ProjectilesProperties.SlowMotion.EnergyCost)
        {
            GameManager.Instance.ActivateSlowMotion(ProjectilesProperties.SlowMotion.Duration);
            RemoveEnergy(ProjectilesProperties.SlowMotion.EnergyCost);
        }
    }

    void Aiming()
    {
        if (_anim)
            _anim.SetBool("Aiming", false);
    }

    public float GetHealth()
    {
        return _heManager.GetHealth();
    }

    public float GetEnergy()
    {
        return _heManager.GetEnergy();
    }

    public void AddHealth(float amount)
    {
        _heManager.AddHp(amount);
    }

    public void AddEnergy(float amount)
    {
        _heManager.AddEnergy(amount);
    }
}
