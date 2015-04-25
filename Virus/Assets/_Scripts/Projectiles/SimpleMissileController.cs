using UnityEngine;
using System.Collections;

public class SimpleMissileController : ProjectileDir
{
    private ParticleSystem _pSys;
    private Component _hlo;
    private LineRenderer _lineR;
    private Light _lght;
    private Collider _cld;

    void Awake()
    {
        DebugDraw.DrawSphere(transform.position, .0f, Color.cyan);
        InitializeProjectile();

        _pSys = GetComponentInChildren<ParticleSystem>();
        _hlo = GetComponent("Halo");
        _lineR = GetComponent<LineRenderer>();
        _lght = GetComponent<Light>();
        _cld = GetComponent<BoxCollider>();
    }

    protected override void StopProjectile(Vector3 posiotion)
    {

        MoveDir = Vector3.zero;
        transform.position = posiotion;
        _cld.enabled = false;
        _hlo.GetType().GetProperty("enabled").SetValue(_hlo, false, null);
        _lineR.enabled = false;
        _lght.enabled = false;

        Invoke("StopParticles", 0.3f);
    }

    void StopParticles()
    {
        _pSys.emissionRate = 0;
    }
}
