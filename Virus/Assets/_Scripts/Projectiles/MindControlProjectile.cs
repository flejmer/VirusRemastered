using UnityEngine;
using System.Collections;

public class MindControlProjectile : MonoBehaviour
{
    public float Speed;
    private EnemySimpleAI _target;

    private ParticleSystem[] _pss;

    private bool _active = true;

    void Awake()
    {
        _pss = GetComponentsInChildren<ParticleSystem>();
    }

    public void SetTarget(EnemySimpleAI target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_active)
        {
            var direction = (_target.transform.position - transform.position).normalized;

            var distance = (_target.transform.position - transform.position).sqrMagnitude;
            var newPosition = transform.position + direction * Speed * Time.deltaTime;
            var newDistance = (newPosition - transform.position).sqrMagnitude;

            if (newDistance < distance)
            {
                transform.position = newPosition;
            }
            else
            {
                transform.position = _target.transform.position;
                ParticlesOnFinish();
            }
        }
    }

    void ParticlesOnFinish()
    {
        _pss[0].Stop();
        _pss[1].Play();
        _target.TakeOver();
        _active = false;

        var cam = Camera.main.gameObject.GetComponent<CameraFollow>();
        cam.ChangeTarget(_target.transform);

        SoundManager.PlayInfectionSound(_target.GetAudioSource());

        Destroy(gameObject, 1);
    }
}
