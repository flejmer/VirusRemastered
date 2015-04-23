using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ProjectileDir : MonoBehaviour
{
    public float MissileSpeed = 20;
    public LayerMask LayerMask = ~(1 << 8 | 1 << 10);

    public bool Bouncy = false;
    public int MaxBounces = 3;
    private int _bouncesCount;

    private Vector3 _moveDir = Vector3.forward;

    private Rigidbody _rBody;
    private Collider _collid;

    private Vector3 _previousPosition;

    protected Vector3 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value; }
    }

    void Awake()
    {
        InitializeProjectile();
    }

    void FixedUpdate()
    {
        ProjectilePhysicsStep();
    }

    protected void InitializeProjectile()
    {
        _rBody = GetComponent<Rigidbody>();
        _previousPosition = _rBody.position;
        _collid = GetComponent<Collider>();
    }

    protected void ProjectilePhysicsStep()
    {
        transform.Translate(_moveDir * MissileSpeed * Time.deltaTime, transform);
        var movementThisStep = _rBody.position - _previousPosition;
        SendCollisionRay(movementThisStep);
        _previousPosition = _rBody.position;
    }

    void SendCollisionRay(Vector3 movement)
    {
        RaycastHit hit;

        if (!Physics.Raycast(transform.position, movement, out hit, movement.magnitude, LayerMask)) return;
        Debug.DrawRay(transform.position, movement * movement.magnitude, Color.cyan, 2);

        if (Bouncy && !(_bouncesCount >= MaxBounces))
        {
            BounceOff(hit);
        }
        else
        {
            StopProjectile();
        }
    }

    void BounceOff(RaycastHit hit)
    {
        var reflected = Vector3.Reflect((hit.point - transform.position).normalized, hit.normal);
        transform.position = hit.point;

        var lookAt = Quaternion.LookRotation(reflected);
        transform.rotation = lookAt;

        if (_bouncesCount == 0)
            LayerMask = ~(1 << LayerMask.NameToLayer("Projectiles"));

        _bouncesCount++;
    }

    protected virtual void StopProjectile()
    {
        MoveDir = Vector3.zero;
        _collid.enabled = false;
    }
}
