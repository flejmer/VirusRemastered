using System;
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
}

[System.Serializable]
public class BasicAttack
{
    public int Damage = 20;
    public GameObject Missile;
    public float MissileLifetime = 10;
    public GameObject Follower;
}

[System.Serializable]
public class LaserAttack
{
    public int Damage = 100;
    public GameObject Missile;
    public float MissileLifetime = 1;
}

[System.Serializable]
public class MindControl
{
    public LayerMask RayMask;
    public float RayLength = 20;
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

    public bool ShieldActivated { get { return _shield.ShieldActivated; }  }
    public bool IsMoving;

    private Rigidbody _rbody;

    private Transform _gun;
    private Transform _missileSpawn;

    private Animator _anim;
    private ShieldController _shield;

    private bool _hacking;

    private HealthEnergyManager _heManager;

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _gun = transform.Find("Body/Gun");
        _missileSpawn = transform.Find("Body/Gun/missileSpawn");
        _anim = GetComponentInChildren<Animator>();
        _shield = GetComponentInChildren<ShieldController>();
        _heManager = GetComponent<HealthEnergyManager>();
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
        Shooting();
        Interaction();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
//            _rbody.velocity = _rbody.velocity + Vector3.up * 10;
        }

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
        _heManager.RemoveHp(amount);
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
            _rbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, MovementProperties.RotationSpeed * Time.deltaTime);

            //            var point = new Vector3(targetPoint.x, _missileSpawn.transform.position.y, targetPoint.z);
            //            _gun.transform.LookAt(point);



            targetRotation = Quaternion.LookRotation(targetPoint - _gun.position);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            var gunRotCopy = _gun.rotation;
            _gun.rotation = Quaternion.Slerp(_gun.rotation, targetRotation, MovementProperties.RotationSpeed * Time.deltaTime);

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

            transform.Translate(movement * MovementProperties.MovementSpeed * Time.deltaTime, Space.World);
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
        EnemySimpleAI enemy;

        if (Physics.Raycast(ray, out hit, 20, ProjectilesProperties.MindControl.RayMask))
        {
            if (hit.transform.CompareTag("EnemyGuard") || hit.transform.CompareTag("EnemyTech"))
            {
                enemy = hit.transform.gameObject.GetComponent<EnemySimpleAI>();
                enemy.Highlight();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    enemy.TakeOver();
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
                if (IsInvoking("Aiming"))
                    CancelInvoke("Aiming");

                _anim.SetBool("Aiming", true);
                _anim.SetTrigger("FireGun");

                Invoke("Aiming", 1.5f);
            }

            if (fireInput1)
            {

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
            }

            if (fireInput2)
            {
                var instance = (GameObject) Instantiate(ProjectilesProperties.LaserAttack.Missile, _missileSpawn.position, _missileSpawn.rotation);

                var script = instance.GetComponent<Laser>();
                script.MissleSpawnPoint = _missileSpawn;

                script.LifeTime = ProjectilesProperties.LaserAttack.MissileLifetime;
                script.FireLaser();

                Destroy(instance, ProjectilesProperties.LaserAttack.MissileLifetime);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_shield.ShieldActivated)
            {
                _shield.DeactivateShield();
            }
            else
            {
                _shield.ActivateShield();
            }
            
        }

    }

    void Aiming()
    {
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
}
