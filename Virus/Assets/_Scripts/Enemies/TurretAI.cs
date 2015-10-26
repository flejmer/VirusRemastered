using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurretAI : MonoBehaviour
{
    public List<GameObject> ListOfObjectsInAwareness { get; private set; }

    private Quaternion _initialRotation;
    private Transform _gun;
    private Transform _missileSpawn;

    private bool _canFire;
    private bool _enumeratorRunning;
    private IEnumerator _fireRateEnumerator;

    public GameObject Missile;
    public float RateOfFire = 1;
    public bool PlayerControlled;



    void Start()
    {
        ListOfObjectsInAwareness = new List<GameObject>();
        
        _fireRateEnumerator = CanFireAgain(RateOfFire);
        _gun = transform.Find("Body/UpperPart");
        _missileSpawn = transform.Find("Body/UpperPart/Gun/missileSpawn");
        _initialRotation = _gun.rotation;
    }

    void Update()
    {
        for (var i = 0; i < ListOfObjectsInAwareness.Count; i++)
        {
            var go = ListOfObjectsInAwareness[i];

            if (!go.CompareTag("EnemyGuard") && !go.CompareTag("EnemyTech")) continue;

            var sc = go.GetComponent<EnemySimpleAI>();

            if (sc.HealthPoints <= 0)
            {
                ListOfObjectsInAwareness.RemoveAt(i);
            }
        }

        if (ListOfObjectsInAwareness.Count > 0)
        {
            if (!PlayerControlled)
            {
                if (GameManager.GetPlayer() == null) return;

                if (ListOfObjectsInAwareness.Contains(GameManager.GetPlayer().gameObject) && !GameManager.GetPlayer().PlayerState.Equals(Enums.PlayerStates.Dead))
                {
                    var target = GameManager.GetPlayer().gameObject;
                    RotateTowards(target.transform.position);
                    
                    Fire(target);
                }
                else
                {
                    if (_gun.rotation.Equals(_initialRotation)) return;

                    _gun.rotation = Quaternion.Lerp(_gun.rotation, _initialRotation, Time.deltaTime * 1);
                    CancelFireRateCoroutine();
                }
            }
            else
            {
                var target = ListOfObjectsInAwareness.FirstOrDefault(o => o.CompareTag("EnemyGuard") || o.CompareTag("EnemyTech"));

                if (target != null)
                {
                    RotateTowards(target.transform.position);

                    Fire(target);
                }
            }
        }
        else
        {
            if (!_gun.rotation.Equals(_initialRotation))
            {
                _gun.rotation = Quaternion.Lerp(_gun.rotation, _initialRotation, Time.deltaTime * 1);
                CancelFireRateCoroutine();
            }
        }
    }

    void Fire(GameObject target)
    {
        if (_canFire)
        {
            RaycastHit hit;
            var dir = (target.transform.position - _missileSpawn.position).normalized;

            if (!Physics.Raycast(_missileSpawn.position, dir, out hit, 15)) return;
            if (hit.transform.CompareTag("Obstacle")) return;

            var instance =
                (GameObject)Instantiate(Missile, _missileSpawn.position, _missileSpawn.rotation);
            var script = instance.GetComponent<SimpleMissileController>();
            script.WhoFired = gameObject;

            Destroy(instance, 5);

            CancelFireRateCoroutine();
        }
        else
        {
            StartFireRateCoroutine();
        }
    }

    IEnumerator CanFireAgain(float time)
    {
        _enumeratorRunning = true;
        yield return new WaitForSeconds(time);

        _canFire = true;
        _enumeratorRunning = false;
        _fireRateEnumerator = CanFireAgain(time);
    }

    void StartFireRateCoroutine()
    {
        if (!_enumeratorRunning)
            StartCoroutine(_fireRateEnumerator);
    }

    void CancelFireRateCoroutine()
    {
        _canFire = false;
        _enumeratorRunning = false;
        StopCoroutine(_fireRateEnumerator);
        _fireRateEnumerator = CanFireAgain(RateOfFire);
    }

    void RotateTowards(Vector3 target)
    {
        var targetRotation = Quaternion.LookRotation(target - _gun.position);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        _gun.rotation = Quaternion.Slerp(_gun.rotation, targetRotation, 15 * Time.deltaTime);
    }
}
