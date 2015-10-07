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

        StartCoroutine(LateBurst(0.05f));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyGuard") || other.CompareTag("EnemyTech"))
        {
            var enemy = other.gameObject.GetComponent<EnemySimpleAI>();
            enemy.RemoveHp(1000);
        }
    }

    IEnumerator LateBurst(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        var expPos = new Vector3(transform.position.x, 0, transform.position.z);

        var colliders = Physics.OverlapSphere(expPos, 4.85f);

        foreach (Collider hit in colliders)
        {
            var rb = hit.GetComponent<Rigidbody>();

            if (!rb.Equals(null))
            {
                rb.AddExplosionForce(1000, expPos, 4.85f);
            }

        }
    }

    public void Burst()
    {
        _collider.enabled = true;
        _particles.Play();
    }
}
