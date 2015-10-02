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
        AI();

    }

    void OnTriggerEnter(Collider other)
    {
        if (_targetComputer == null) return;
        if (!other.gameObject.Equals(_targetComputer.gameObject)) return;

        RotateTowards(_targetComputer.transform.position);
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

                }
                else if (!_enemyState.Equals(Enums.EnemyTechStates.RunForYourLife) &&
                         !_enemyState.Equals(Enums.EnemyTechStates.PlayerControlled))
                {
                    var randInCircle = Random.insideUnitCircle * 3;
                    var position = GameManager.GetClosestHealingCenter(gameObject).transform.position +
                                   new Vector3(randInCircle.x, 0, randInCircle.y);

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
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.RunToComputer))
        {
            Agent.Resume();
            Agent.SetDestination(_targetComputer.GetHackPosition());

            _anim.SetBool("Running", true);

            if (Agent.remainingDistance <= 0)
            {
//                Debug.Log("dsad");
                _enemyState = Enums.EnemyTechStates.Hack;
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.Hack))
        {
            _anim.SetBool("Running", false);

            _targetComputer.StopHacking();
            _targetComputer.StartDehacking();
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.RunForYourLife))
        {
            _anim.SetBool("Running", true);

            if (Agent.remainingDistance <= 0)
            {
                _enemyState = Enums.EnemyTechStates.Healing;
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.Healing))
        {
            _anim.SetBool("Running", false);

            if (HealthPoints >= MaxHpPoints)
            {
                _enemyState = Enums.EnemyTechStates.Idle;
            }
        }
        else if (_enemyState.Equals(Enums.EnemyTechStates.PlayerControlled))
        {

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

        if (_ragdoll)
        {
            _collider.enabled = false;
            _ragdoll.ActivateRagdoll();

            RaycastHit hit;

            if (Physics.Raycast(pos - dir, dir, out hit, 3, mask))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(dir * 2500);
            }

        }
    }

    public override void TakeOver()
    {
        throw new System.NotImplementedException();
    }
}
