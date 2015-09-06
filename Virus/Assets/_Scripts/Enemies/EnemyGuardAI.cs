using UnityEngine;
using System.Collections;

public class EnemyGuardAI : EnemySimpleAI
{
    public float RotationSpeed = 10;
    public float RateOfFire = 1;

    public GameObject Missile;
    public LayerMask SeekerMask;

    private Transform _target;
    private Animator _anim;

    private Enums.EnemyGuardStates _enemyState = Enums.EnemyGuardStates.Idle;
    private CapsuleCollider _collider;

    private Transform _missileSpawn;
    private Transform _gun;

    private bool _canFire;
    private bool _targetClose;
    private bool _canFadeAway;

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
            _targetClose = true;

            if (_target == null)
                _target = other.transform;

            RaycastHit hit;

            var direction = (_target.position - _gun.position).normalized;

            if (Physics.Raycast(_gun.position, direction, out hit, 15.0f, SeekerMask))
            {
                if (hit.transform.tag.Equals("Player"))
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

        if (other.CompareTag("Player") && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
            _targetClose = true;

            RaycastHit hit;
            var direction = (_target.position - _gun.position).normalized;

            if (Physics.Raycast(_gun.position, direction, out hit, 15.0f, SeekerMask))
            {
                if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
                {
                    if (!hit.transform.tag.Equals("Player"))
                    {
                        _enemyState = Enums.EnemyGuardStates.Chase;

                        _canFire = false;
                        CancelInvoke("CanFireAgain");
                    }
                }
                else
                {
                    if (hit.transform.tag.Equals("Player"))
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
        if (other.CompareTag("Player") && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
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
            Agent.SetDestination(_target.position);
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

        if (HealthPoints <= 0)
            _enemyState = Enums.EnemyGuardStates.Dead;
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

            if(!_canFadeAway)
                Invoke("EnableFade", 3);

        }
        // Rotation towards target after its reached

        else if ((_target != null && _targetClose) || _enemyState.Equals(Enums.EnemyGuardStates.Shooting))
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*RotationSpeed);

            var targetRotation = Quaternion.LookRotation(_target.position - _gun.position);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            _gun.rotation = Quaternion.Slerp(_gun.rotation, targetRotation, 15*Time.deltaTime);
        }
    }


}
