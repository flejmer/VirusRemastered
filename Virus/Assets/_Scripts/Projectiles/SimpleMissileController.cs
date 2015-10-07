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

//        Debug.Log(hitObj.tag);

        if (hitObj.CompareTag("Player"))
        {
            if (GameManager.GetPlayer().ShieldActivated)
            {
                WhoFired = GameManager.GetPlayer().gameObject;
            }
            else
            {
                Bouncy = !Bouncy;
                GameManager.DamagePlayerFromDirection(hitObj, 20, hit.point, MoveDir, LayerMask, WhoFired);
            }
            
        }
        else if (hitObj.CompareTag("EnemyGuard") || hitObj.CompareTag("EnemyTech"))
        {

            GameManager.DamageEnemyFromDirection(hitObj, 20, hit.point, MoveDir, LayerMask, WhoFired);
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
