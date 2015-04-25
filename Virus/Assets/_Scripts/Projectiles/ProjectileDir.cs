﻿using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine.EventSystems;

public class ProjectileDir : MonoBehaviour
{
    public float MissileSpeed = 20;
    public LayerMask LayerMask;

    public bool Bouncy = false;
    public int MaxBounces = 3;
    private int _bouncesCount;

    private Vector3 _moveDir = Vector3.forward;

    private Rigidbody _rBody;
    private Collider _collid;
    private float _minimumExtent;
    private float _partialExtent;

    private Vector3 _previousPosition;

    protected Vector3 MoveDir
    {
        get { return transform.TransformDirection(_moveDir); }
        set { _moveDir = value; }
    }

    void Awake()
    {
        InitializeProjectile();
    }

    void Start()
    {
        SendCollisionRay(MoveDir);
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
        _minimumExtent = Mathf.Min(Mathf.Min(_collid.bounds.extents.x, _collid.bounds.extents.y), _collid.bounds.extents.z);
        _partialExtent = _minimumExtent * (1.0f - 0.1f);
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

        Debug.DrawRay(transform.position, movement * movement.magnitude, Color.cyan, 4);
        if (!Physics.Raycast(transform.position, movement, out hit, movement.magnitude, LayerMask)) return;

        DebugDraw.DrawSphere(hit.point, 2, Color.black);

        if (Bouncy && !(_bouncesCount >= MaxBounces))
        {
            BounceOff(hit);
        }
        else
        {
            _rBody.position = hit.point - (movement / movement.magnitude) * _partialExtent;
            StopProjectile(hit);
        }
    }

    void BounceOff(RaycastHit hit)
    {
        var reflected = Vector3.Reflect((hit.point - transform.position).normalized, hit.normal);

        Quaternion lookAt = !reflected.Equals(Vector3.zero) ? Quaternion.LookRotation(reflected) : Quaternion.Inverse(transform.rotation);
        
        transform.position = hit.point;
        transform.rotation = lookAt;
        

        if (_bouncesCount == 0)
            LayerMask = 1 << LayerMask.NameToLayer("Player")
                | 1 << LayerMask.NameToLayer("Enemies")
                | 1 << LayerMask.NameToLayer("Obstacles");

        _bouncesCount++;
    }

    protected virtual void StopProjectile(RaycastHit hit)
    {
        transform.position = hit.point;
        MoveDir = Vector3.zero;
        _collid.enabled = false;
    }
}
