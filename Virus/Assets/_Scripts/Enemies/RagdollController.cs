using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RagdollController : MonoBehaviour
{
    public float ActiveDuration = 3;

    private Animator _anim;

    private List<Rigidbody> _rbodies = new List<Rigidbody>();
    private List<Collider> _colliders = new List<Collider>();

    public bool RagdollActivated { get; private set; }

    void Start()
    {
        _anim = GetComponent<Animator>();
        _rbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        _colliders.AddRange(GetComponentsInChildren<Collider>());

        foreach (var rbody in _rbodies)
        {
            rbody.isKinematic = true;
        }

        foreach (var coll in _colliders)
        {
            coll.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ActivateRagdoll();
        }
    }

    public void ActivateRagdoll()
    {
        _anim.enabled = false;

        foreach (var rbody in _rbodies)
        {
            rbody.isKinematic = false;
        }

        foreach (var coll in _colliders)
        {
            coll.enabled = true;
        }

        RagdollActivated = true;

        Invoke("DeactivateEverything", ActiveDuration);
    }

    private void DeactivateEverything()
    {
        foreach (var rbody in _rbodies)
        {
            rbody.isKinematic = true;
        }

        foreach (var coll in _colliders)
        {
            coll.enabled = false;
        }
    }
}
