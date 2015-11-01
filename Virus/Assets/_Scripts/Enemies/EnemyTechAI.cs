using UnityEngine;
using System.Collections;

public class EnemyTechAI : EnemySimpleAI
{
    public float RotationSpeed = 10;
    public bool Active;

    private Animator _anim;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private Enums.EnemyTechStates _enemyState = Enums.EnemyTechStates.Idle;

    private CompController _targetComputer;
    private CapsuleCollider _collider;

    private RagdollController _ragdoll;

    private bool _canFade;

    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        _anim = GetComponentInChildren<Animator>();
        _ragdoll = GetComponentInChildren<RagdollController>();

        _collider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if (Active)
            AI();

    }

    void OnTriggerStay(Collider other)
    {
        if (_targetComputer == null) return;
        if (!other.CompareTag("ComputerInteraction")) return;

        if (!_enemyState.Equals(Enums.EnemyTechStates.RunForYourLife) && !_enemyState.Equals(Enums.EnemyTechStates.RunForYourLife))
            RotateTowards(new Vector3(_targetComputer.transform.position.x, transform.position.y, _targetComputer.transform.position.z));
    }

    void BackToPlayer()
    {
        if (PlayerControlled)
        {
            var cam = Camera.main.gameObject.GetComponent<CameraFollow>();
            cam.BackToPlayer(3);
        }

    }

    private void AI()
    {
        if (HealthPoints <= (MaxHpPoints * .2f))
        {
            if (!_enemyState.Equals(Enums.EnemyTechStates.Dead))
            {
                if (HealthPoints <= 0)
                {
                    _enemyState = Enums.EnemyTechStates.Dead;

                    BackToPlayer();

                    if (_targetComputer != null && _targetComputer.IsDehackInProgress)
                    {
                        _targetComputer.StopDehacking();
                        _targetComputer.StartHacking(GameManager.GetPlayer());
                    }

                }
                else if (!_enemyState.Equals(Enums.EnemyTechStates.RunForYourLife) && !_enemyState.Equals(Enums.EnemyTechStates.PlayerControlled) && !_enemyState.Equals(Enums.EnemyGuardStates.Dead))
                {
                    var randInCircle = Random.insideUnitCircle * 2;
                    var position = GameManager.GetClosestHealingCenter(gameObject).transform.position + new Vector3(randInCircle.x, 0, randInCircle.y);

                    Agent.SetDestination(position);
                    _enemyState = Enums.EnemyTechStates.RunForYourLife;
                }
            }
        }

        if (_enemyState.Equals(Enums.EnemyTechStates.Idle))
        {
            if (_targetComputer != null)
            {
                _enemyState = Enums.EnemyTechStates.RunToComputer;
            }
            else if (GameManager.GetLastHackedComputer() != null)
            {
                _targetComputer = GameManager.GetLastHackedComputer();
                _enemyState = Enums.EnemyTechStates.RunToComputer;
            }
            else
            {
                if (!(Agent.remainingDistance <= 0))
                {
                    Agent.Resume();
                    Agent.SetDestination(_startPosition);
                    _anim.SetBool("Running", true);
                }
                else
                {
                    Agent.Stop();
                    _anim.SetBool("Running", false);

                    if (!transform.rotation.Equals(_startRotation))
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, _startRotation, RotationSpeed * Time.deltaTime);
                    }
                }
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.RunToComputer))
        {
            Agent.Resume();
            Agent.SetDestination(_targetComputer.GetHackPosition());

            _anim.SetBool("Running", true);

            if (Agent.remainingDistance <= 0 && !float.IsPositiveInfinity(Agent.remainingDistance))
            {
                _enemyState = Enums.EnemyTechStates.Hack;
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.Hack))
        {
            if (float.IsPositiveInfinity(Agent.remainingDistance) || Agent.remainingDistance > 0)
            {
                _enemyState = Enums.EnemyTechStates.RunToComputer;
                return;
            }

            //            transform.position = Agent.destination + new Vector3(0, 1f, 0);

            _anim.SetBool("Running", false);
            Agent.Stop();

            if (!_targetComputer.IsDehackInProgress && _targetComputer.IsHacked)
            {
                _targetComputer.StopHacking();
                _targetComputer.StartDehacking();
            }

            if (!_targetComputer.IsHacked)
            {
                _enemyState = Enums.EnemyTechStates.Idle;
                _targetComputer = null;
                Agent.SetDestination(_startPosition);
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.RunForYourLife))
        {
            Agent.Resume();
            _anim.SetBool("Running", true);
            //            Debug.Log(Agent.remainingDistance);

            if (Agent.remainingDistance <= 0)
            {
                _enemyState = Enums.EnemyTechStates.Healing;
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.Healing))
        {
            Agent.Stop();
            _anim.SetBool("Running", false);

            if (HealthPoints >= MaxHpPoints)
            {
                _enemyState = Enums.EnemyTechStates.Idle;
                Agent.Resume();
                Agent.SetDestination(_startPosition);
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.PlayerControlled))
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

                var instance = (GameObject)Instantiate(Burst, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
                var script = instance.GetComponent<SuicideController>();

                SoundManager.PlayEnemyBurstSound(GetAudioSource());

                script.Burst();
                Destroy(instance, 3);
            }
        }
        else
        {
            if (!_canFade)
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

    void EnableFade()
    {
        _canFade = true;
    }

    void RotateTowards(Vector3 target)
    {
        var direction = (target - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }


    public override void HitPoint(Vector3 pos, Vector3 dir, float force, LayerMask mask)
    {
        if (!(HealthPoints <= 0)) return;

        _enemyState = Enums.EnemyTechStates.Dead;

        BackToPlayer();

        if (_targetComputer != null && _targetComputer.IsDehackInProgress)
        {
            _targetComputer.StopDehacking();
            _targetComputer.StartHacking(GameManager.GetPlayer());
        }

        if (_ragdoll)
        {
            _collider.enabled = false;
            _ragdoll.ActivateRagdoll();

            RaycastHit hit;

            //todo: better
            if (Physics.Raycast(pos - dir, dir, out hit, 1, mask))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(dir * 2500);
            }

        }
    }

    public override void TakeOver()
    {
        _targetComputer = null;
        PlayerControlled = true;
        Active = true;

        _anim.SetBool("Running", false);
        Agent.Resume();
        Agent.SetDestination(transform.position);

        _enemyState = Enums.EnemyTechStates.PlayerControlled;
    }

    public override void GotHitBy(GameObject shooter)
    {
        //nothing
    }
}
