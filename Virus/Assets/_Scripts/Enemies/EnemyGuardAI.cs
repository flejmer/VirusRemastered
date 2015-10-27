using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGuardAI : EnemySimpleAI
{
    public float RotationSpeed = 10;
    public float RateOfFire = 1;

    public GameObject Missile;
    public LayerMask SeekerMask;

    public GameObject Target;
    private Animator _anim;

    private Enums.EnemyGuardStates _enemyState = Enums.EnemyGuardStates.Idle;
    private CapsuleCollider _collider;

    private Transform _missileSpawn;
    private Transform _gun;

    private bool _canFire;
    private bool _targetClose;
    private bool _canFadeAway;

    public bool SpawnInWall { get; set; }

    private IEnumerator _fireRateEnumerator;
    private bool _enumeratorRunning;

    private RagdollController _ragdoll;

    private Vector3 _startingPos;

    void Start()
    {
        _startingPos = transform.position;

        _anim = GetComponentInChildren<Animator>();
        _collider = GetComponent<CapsuleCollider>();

        _missileSpawn = transform.Find("Body/Gun/missileSpawn");
        _gun = transform.Find("Body/Gun");

        _ragdoll = GetComponentInChildren<RagdollController>();

        _fireRateEnumerator = CanFireAgain(RateOfFire);
    }

    void Update()
    {
        AI();
        Movement();

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log(_enemyState + " " + gameObject.name + " ");
        }

        if (GameManager.GetPlayer() == null)
        {
            if (Target != null)
            {
                if (Target.CompareTag("Player"))
                {
                    Target = null;
                }
            }
        }
    }

    IEnumerator CanFireAgain(float time)
    {
        _enumeratorRunning = true;

        yield return new WaitForSeconds(time);

        _canFire = true;

        _fireRateEnumerator = CanFireAgain(time);
        _enumeratorRunning = false;
        //        if(!PlayerControlled)
        //            Debug.Log("call");
    }

    void StartFireRateCoroutine()
    {
        if (!_enumeratorRunning)
            StartCoroutine(_fireRateEnumerator);
    }

    void CancelFireRateCoroutine()
    {
        _canFire = false;
        _enumeratorRunning = false;
        StopCoroutine(_fireRateEnumerator);
        _fireRateEnumerator = CanFireAgain(RateOfFire);
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Hologram") || other.gameObject.Equals(Target)) && !_enemyState.Equals(Enums.EnemyGuardStates.Dead) && !_enemyState.Equals(Enums.EnemyGuardStates.RunAway))
        {
            if (other.CompareTag("Player") && GameManager.GetPlayer().PlayerState.Equals(Enums.PlayerStates.Dead)) return;

            if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) && other.CompareTag("Player")) return;

            if (Target == null)
                Target = other.gameObject;

            if (other.CompareTag("Hologram"))
            {
                Target = other.gameObject;
            }

            RaycastHit hit;

            var direction = (Target.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out hit, 15.0f, SeekerMask) && !_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
            {
                if (hit.transform.gameObject.Equals(Target))
                {
                    _enemyState = Enums.EnemyGuardStates.Shooting;

                    StartFireRateCoroutine();
                }
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_enemyState.Equals(Enums.EnemyGuardStates.RunAway)) return;

        if ((other.CompareTag("Player") || other.CompareTag("Hologram") || other.gameObject.Equals(Target)) &&
            !_enemyState.Equals(Enums.EnemyGuardStates.Dead) && !_enemyState.Equals(Enums.EnemyGuardStates.RunAway))
        {
            if (other.CompareTag("Player") && GameManager.GetPlayer().PlayerState.Equals(Enums.PlayerStates.Dead)) return;

            if (!(_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) && other.CompareTag("Player")))
            {

                if (Target == null)
                    Target = other.gameObject;

                if (other.CompareTag("Hologram"))
                {
                    Target = other.gameObject;
                }
            }
        }

        if (other.gameObject.Equals(Target) && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
        {
            if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) && Target.CompareTag("Player") && _enemyState.Equals(Enums.EnemyGuardStates.RunAway)) return;

            _targetClose = true;

            Vector3 position;
            Vector3 direction;

            if (_enemyState.Equals(Enums.EnemyGuardStates.Chase) || _enemyState.Equals(Enums.EnemyGuardStates.Shooting))
            {
                position = _gun.position;
                direction = (Target.transform.position - _gun.position).normalized;
            }
            else
            {
                position = transform.position;
                direction = (Target.transform.position - transform.position).normalized;
            }

            RaycastHit hit;

            if (SpawnInWall)
            {
                _enemyState = Enums.EnemyGuardStates.Chase;

                CancelFireRateCoroutine();

                return;
            }

            if (Physics.Raycast(position, direction, out hit, 15.0f, SeekerMask))
            {
                if (!_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
                {
                    if (!hit.transform.gameObject.Equals(Target)) return;

                    _enemyState = Enums.EnemyGuardStates.Shooting;

                    StartFireRateCoroutine();
                }
                else
                {
                    if (hit.transform.gameObject.Equals(Target)) return;

                    _enemyState = Enums.EnemyGuardStates.Chase;

                    CancelFireRateCoroutine();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(Target) && !_enemyState.Equals(Enums.EnemyGuardStates.Dead) && !_enemyState.Equals(Enums.EnemyGuardStates.Idle))
        {
            if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) && other.CompareTag("Player") && _enemyState.Equals(Enums.EnemyGuardStates.RunAway)) return;

            _targetClose = false;

            _enemyState = Enums.EnemyGuardStates.Chase;

            CancelFireRateCoroutine();
        }
    }

    public void StartChase()
    {
        if (Target == null)
        {
            Target = GameManager.GetPlayer().gameObject;
            _enemyState = Enums.EnemyGuardStates.Chase;
        }
        
    }

    private void AI()
    {
        if (HealthPoints <= (MaxHpPoints * 1 / 3))
        {
            if (!_enemyState.Equals(Enums.EnemyGuardStates.Dead))
            {
                if (HealthPoints <= 0)
                {
                    _enemyState = Enums.EnemyGuardStates.Dead;

                    BackToPlayer();

                }
                else if (!_enemyState.Equals(Enums.EnemyGuardStates.RunAway) &&
                         !_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled) &&
                         !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
                {
                    var randInCircle = Random.insideUnitCircle * 3;
                    var position = GameManager.GetClosestHealingCenter(gameObject).transform.position +
                                   new Vector3(randInCircle.x, 0, randInCircle.y);

                    Agent.SetDestination(position);
                    _enemyState = Enums.EnemyGuardStates.RunAway;
                }
            }
        }

        if (_enemyState.Equals(Enums.EnemyGuardStates.Idle))
        {
            _anim.SetBool("Aiming", false);
            Agent.SetDestination(_startingPos);

            if (Agent.velocity.sqrMagnitude > 0)
            {
                _anim.SetBool("Running", true);
            }
            else
            {
                _anim.SetBool("Running", false);
            }
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Chase))
        {
            if (Target != null)
                if (Target.CompareTag("Player") && RealCyberManager.GetPlayer().PlayerState.Equals(Enums.PlayerStates.Dead))
                {
                    Target = null;
                    Agent.SetDestination(_startingPos);
                }

            if (Target != null)
            {
                Agent.Resume();
                Agent.SetDestination(Target.transform.position);
                _anim.SetBool("Aiming", false);
                _anim.SetBool("Running", true);
            }
            else
            {
                _enemyState = Enums.EnemyGuardStates.Idle;
                _anim.SetBool("Aiming", false);
                _anim.SetBool("Running", false);
            }

        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting))
        {
            Agent.Stop();
            _anim.SetBool("Running", false);
            _anim.SetBool("Aiming", true);

            if (Target != null)
            {
                if (Target.CompareTag("EnemyGuard") || Target.CompareTag("EnemyTech"))
                {
                    var ai = Target.GetComponent<EnemySimpleAI>();

                    if (ai.HealthPoints <= 0)
                    {
                        Target = null;
                    }
                }
                else
                {

                    if (RealCyberManager.GetPlayer().PlayerState.Equals(Enums.PlayerStates.Dead))
                    {
                        Target = null;
                    }
                }
            }
            else
            {
                _enemyState = PlayerControlled ? Enums.EnemyGuardStates.PlayerControlled : Enums.EnemyGuardStates.Idle;

                Agent.SetDestination(transform.position);
                Agent.Resume();
                _anim.SetBool("Aiming", false);

                CancelFireRateCoroutine();
            }

            if (_canFire)
            {
                var instance = (GameObject)Instantiate(Missile, _missileSpawn.position, _missileSpawn.rotation);
                instance.GetComponent<ProjectileDir>().WhoFired = gameObject;

                Destroy(instance, 5);

                _anim.SetTrigger("FireGun");

                _canFire = false;

                StartFireRateCoroutine();
            }
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.RunAway))
        {
            Agent.Resume();

            if (!(Agent.remainingDistance <= 0))
            {
                _anim.SetBool("Aiming", false);
                _anim.SetBool("Running", true);
            }
            else
            {
                Agent.Stop();
                _anim.SetBool("Running", false);
                _enemyState = Enums.EnemyGuardStates.Healing;
            }
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.Healing))
        {
            if (HealthPoints >= MaxHpPoints)
            {
                _enemyState = Enums.EnemyGuardStates.Chase;
            }
        }
        else if (_enemyState.Equals(Enums.EnemyGuardStates.PlayerControlled))
        {
            var leftClick = Input.GetMouseButtonDown(0);
            var rightClick = Input.GetMouseButtonDown(1);

            var space = Input.GetKeyDown(KeyCode.Space);

            _anim.SetBool("Running", !(Agent.remainingDistance <= 0));

            if (leftClick || rightClick)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 30, MindControlRayMask))
                {
                    if (leftClick)
                    {

                        if ((hit.transform.CompareTag("EnemyGuard") || hit.transform.CompareTag("EnemyTech")) &&
                            !hit.transform.gameObject.Equals(gameObject))
                        {

                            Target = hit.transform.gameObject;
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

            if (space)
            {
                RemoveHp(1000);

                var instance =
                    (GameObject)Instantiate(Burst, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
                var script = instance.GetComponent<SuicideController>();

                if (!_ragdoll.RagdollActivated)
                {
                    _collider.enabled = false;
                    _ragdoll.ActivateRagdoll();
                }

                script.Burst();
                Destroy(instance, 3);
            }
        }
        else
        {
            if (_ragdoll == null)
            {
                if (Agent.enabled)
                {
                    Agent.Stop();

                }

                _anim.SetBool("Dead", true);
                _collider.enabled = false;

                Destroy(gameObject, 8);

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
            else if (_ragdoll != null)
            {
                if (!_canFadeAway)
                {
                    if (Agent.enabled)
                    {
                        Agent.Stop();
                    }

                    if (!_ragdoll.RagdollActivated)
                    {
                        _ragdoll.ActivateRagdoll();
                    }

                    _collider.enabled = false;
                    Destroy(gameObject, 8);

                    Invoke("EnableFade", _ragdoll.ActiveDuration + .1f);
                }
                else
                {
                    var newPosition = new Vector3(transform.position.x, -1, transform.position.z);

                    transform.position = Vector3.Lerp(transform.position, newPosition, .15f * Time.deltaTime);
                    Agent.enabled = false;
                }
            }
        }

    }

    void EnableFade()
    {
        _canFadeAway = true;
    }

    private void Movement()
    {
        // Rotation towards target after its reached

        if (!_enemyState.Equals(Enums.EnemyGuardStates.Idle) && Target != null && _targetClose)
        {
            if (_enemyState.Equals(Enums.EnemyGuardStates.Shooting) ||
                _enemyState.Equals(Enums.EnemyGuardStates.Chase))
            {
                RotateTowards(Target.transform.position);
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

    void BackToPlayer()
    {
        if (PlayerControlled)
        {
            var cam = Camera.main.gameObject.GetComponent<CameraFollow>();
            cam.BackToPlayer(3);
        }

    }

    public override void HitPoint(Vector3 pos, Vector3 dir, float force, LayerMask mask)
    {
        if (HealthPoints <= 0)
        {
            _enemyState = Enums.EnemyGuardStates.Dead;

            BackToPlayer();

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

    public override void GotHitBy(GameObject shooter)
    {
        if (shooter.CompareTag("EnemyGuard"))
        {
            Target = shooter;
        }
        else
        {
            if (PlayerControlled) return;
            if (shooter.CompareTag("Turret")) return;

            if (Target != null) return;

            Target = shooter;
            _enemyState = Enums.EnemyGuardStates.Chase;
        }

    }

    public override void TakeOver()
    {
        Debug.Log("guard taken");
        PlayerControlled = true;
        Target = null;

        _anim.SetBool("Aiming", false);
        Agent.Resume();
        Agent.SetDestination(transform.position);

        CancelFireRateCoroutine();

        _enemyState = Enums.EnemyGuardStates.PlayerControlled;
    }

}
