﻿using UnityEngine;
using System.Collections;

public class EnemyGuardAI : EnemySimpleAI
{
    public float RotationSpeed = 10;
    public float RateOfFire = 1;

    public GameObject Missile;
    public LayerMask SeekerMask;

    public GameObject Burst;
    public LayerMask RayMask;

    private GameObject _target;
    private Animator _anim;

    private Enums.EnemyGuardStates _enemyState = Enums.EnemyGuardStates.Idle;
    private CapsuleCollider _collider;

    private Transform _missileSpawn;
    private Transform _gun;

    private bool _canFire;
    private bool _targetClose;
    private bool _canFadeAway;
    private bool _playerControlled;

    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _collider = GetComponent<CapsuleCollider>();

        _missileSpawn = transform.Find("Body/Gun/missileSpawn");
        _gun = transform.Find("Body/Gun");
    }

    void Update()
    {
        AI();
        Movement();

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log(_enemyState);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.gameObject.Equals(_target)) && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
            if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) && other.CompareTag("Player")) return;

            if (_target == null)
                _target = other.gameObject;

            Debug.Log("dasd");

            RaycastHit hit;

            var direction = (_target.transform.position - _gun.position).normalized;

            if (Physics.Raycast(_gun.position, direction, out hit, 15.0f, SeekerMask) && !_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
            {
                if (hit.transform.gameObject.Equals(_target))
                {
                    _enemyState = Enums.EnemyGuardStates.Shooting;
                    Invoke("CanFireAgain", RateOfFire / 2);
                }
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_enemyState.Equals(Enums.EnemyGuardStates.RunAway)) return;

        if (other.gameObject.Equals(_target) && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
            if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) && _target.CompareTag("Player")) return;

            _targetClose = true;

            RaycastHit hit;
            var direction = (_target.transform.position - _gun.position).normalized;

            if (Physics.Raycast(_gun.position, direction, out hit, 15.0f, SeekerMask))
            {
                if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
                {
                    if (!hit.transform.gameObject.Equals(_target))
                    {
                        _enemyState = Enums.EnemyGuardStates.Chase;

                        _canFire = false;
                        CancelInvoke("CanFireAgain");
                    }
                }
                else
                {
                    if (hit.transform.gameObject.Equals(_target) && !_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
                    {
                        _enemyState = Enums.EnemyGuardStates.Shooting;
                        Invoke("CanFireAgain", RateOfFire / 2);

                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(_target) && !_enemyState.Equals(Enums.EnemyGuardStates.Dead) && !_enemyState.Equals(Enums.EnemyGuardStates.Idle))
        {
            if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) && other.CompareTag("Player")) return;

            _targetClose = false;

            _enemyState = Enums.EnemyGuardStates.Chase;

            _canFire = false;
            CancelInvoke("CanFireAgain");
        }
    }

    private void CanFireAgain()
    {
        _canFire = true;
    }

    void AI()
    {
        if (_enemyState.Equals(Enums.EnemyGuardStates.Idle))
        {
            _anim.SetBool("Aiming", false);
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Chase))
        {
            Agent.Resume();
            Agent.SetDestination(_target.transform.position);
            _anim.SetBool("Aiming", false);
            _anim.SetBool("Running", true);
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
        {
            Agent.Stop();
            _anim.SetBool("Running", false);
            _anim.SetBool("Aiming", true);

            if (_canFire)
            {
                var instance = (GameObject)Instantiate(Missile, _missileSpawn.position, _missileSpawn.rotation);

                Destroy(instance, 5);

                _anim.SetTrigger("FireGun");

                _canFire = false;
                Invoke("CanFireAgain", RateOfFire);
            }
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.RunAway))
        {

        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Healing))
        {

        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled))
        {
            var leftClick = Input.GetMouseButtonDown(0);
            var rightClick = Input.GetMouseButtonDown(1);

            _anim.SetBool("Running", !(Agent.remainingDistance <= 0));

            if (leftClick || rightClick)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 20, RayMask))
                {
                    if (leftClick)
                    {

                        if ((hit.transform.CompareTag("EnemyGuard") || hit.transform.CompareTag("EnemyTech")) &&
                            !hit.transform.gameObject.Equals(gameObject))
                        {
                            
                            _target = hit.transform.gameObject;
                            _enemyState = Enums.EnemyGuardStates.Chase;
                        }

                    }

                    if (rightClick)
                    {
                        Agent.Resume();
                        Agent.SetDestination(hit.point);
                    }
                }
            }
        }
        else
        {
            if (Agent.enabled)
            {
                Agent.Stop();

            }

            _anim.SetBool("Dead", true);
            _collider.enabled = false;

            Destroy(gameObject, 8);
        }

        if (HealthPoints <= 0 && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
            _enemyState = Enums.EnemyGuardStates.Dead;

            var instance = (GameObject)Instantiate(Burst, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
            var script = instance.GetComponent<SuicideController>();
            script.Burst();

            Destroy(instance, 3);
        }
    }

    void EnableFade()
    {
        _canFadeAway = true;
    }

    private void Movement()
    {
        if (_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
            Agent.enabled = false;

            Vector3 newPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
            float fadeTime;

            if (_canFadeAway)
            {
                newPosition = new Vector3(transform.position.x, -1f, transform.position.z);
                fadeTime = .15f;
            }
            else
            {
                newPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
                fadeTime = 5;
            }
            transform.position = Vector3.Lerp(transform.position, newPosition, fadeTime * Time.deltaTime);

            if (!_canFadeAway)
                Invoke("EnableFade", 3);

        }
        // Rotation towards target after its reached

        else if (!_enemyState.Equals(Enums.EnemyGuardStates.Idle) && _target != null && _targetClose)
        {
            if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting) ||
                _enemyState.Equals(Enums.EnemyGuardStates.Chase))
            {
                RotateTowards(_target.transform.position);
            }
        }
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);

        var targetRotation = Quaternion.LookRotation(target - _gun.position);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        _gun.rotation = Quaternion.Slerp(_gun.rotation, targetRotation, 15 * Time.deltaTime);
    }

    public override void TakeOver()
    {
        Debug.Log("taken");
        _playerControlled = true;
        _enemyState = Enums.EnemyGuardStates.PlayerControlled;
    }

}
