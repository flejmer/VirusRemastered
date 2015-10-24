using UnityEngine;
using System.Collections;

public class SimpleMissileController : ProjectileDir
{
    public float Damage = 20;

    private ParticleSystem _pSys;
    private Component _hlo;
    private LineRenderer _lineR;
    private Light _lght;
    private Collider _cld;

    void Awake()
    {
        InitializeProjectile();

        _pSys = GetComponentInChildren<ParticleSystem>();
        _hlo = GetComponent("Halo");
        _lineR = GetComponent<LineRenderer>();
        _lght = GetComponent<Light>();
        _cld = GetComponent<BoxCollider>();
    }

    protected override void InteractionOnHit(RaycastHit hit)
    {
        var hitObj = hit.transform.gameObject;

        if (hitObj.CompareTag("Obstacle"))
        {
            if (Bouncy)
                Bouncy = !Bouncy;
        }

        if (hitObj.CompareTag("Player"))
        {
            if (GameManager.GetPlayer().ShieldActivated)
            {
                WhoFired = GameManager.GetPlayer().gameObject;
                Bouncy = true;
            }
            else
            {
                if (Bouncy)
                    Bouncy = !Bouncy;
                GameManager.DamagePlayerFromDirection(hitObj, Damage, hit.point, MoveDir, LayerMask, WhoFired);
            }

        }
        else if (hitObj.CompareTag("EnemyGuard") || hitObj.CompareTag("EnemyTech"))
        {

            GameManager.DamageEnemyFromDirection(hitObj, Damage, hit.point, MoveDir, LayerMask, WhoFired);
            if (Bouncy)
                Bouncy = !Bouncy;
        }
    }

    protected override void StopProjectile(Vector3 posiotion)
    {

        MoveDir = Vector3.zero;
        transform.position = posiotion;
        _cld.enabled = false;
        _hlo.GetType().GetProperty("enabled").SetValue(_hlo, false, null);
        _lineR.enabled = false;
        _lght.enabled = false;

        Invoke("StopParticles", 0.1f);
    }

    void StopParticles()
    {
        _pSys.emissionRate = 0;
    }
}
