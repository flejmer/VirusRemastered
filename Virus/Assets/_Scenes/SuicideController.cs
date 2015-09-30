using UnityEngine;
using System.Collections;

public class SuicideController : MonoBehaviour
{
    private ParticleSystem _particles;
    private SphereCollider _collider;

    void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        _collider = GetComponent<SphereCollider>();
        
        _collider.enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            Burst();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
    }

    public void Burst()
    {
        _collider.enabled = true;
        _particles.Play();
    }
}
