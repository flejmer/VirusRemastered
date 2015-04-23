using UnityEngine;
using System.Collections;

[System.Serializable]
public class MovementProperties
{
    public float MovementSpeed = 15;
    public float RotationSpeed = 10;
}

[System.Serializable]
public class ProjectilesProperties
{
    public BasicAttack BasicAttack;
}

[System.Serializable]
public class BasicAttack
{
    public GameObject Missile;
    public float MissileLifetime = 10;
}

public class PlayerController : MonoBehaviour
{
    public MovementProperties MovementProperties;
    public ProjectilesProperties ProjectilesProperties;

    private Rigidbody _rbody;

    private GameObject _missileSpawn;
    private Collider _missileSpawnCollider;

    private Collider _wallBounds;
    private bool _spawnInWall = false;

    //debug
    private const float FireRate = 0.2f;
    private float _timeCapture;

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _missileSpawn = GameObject.Find("/PlayerV0/Body/Gun/missileSpawn");
        _missileSpawnCollider = _missileSpawn.GetComponent<Collider>();
    }

    void Start()
    {
        StartCoroutine("MissileSpawnBoundsCheck");
    }

    void Update()
    {
        Shooting();
    }

    void FixedUpdate()
    {
        MouseRotation();
        Movement();
    }

    void OnTriggerStay(Collider other)
    {
        if (_missileSpawnCollider.bounds.Intersects(other.bounds))
            _wallBounds = other;
    }

    void MouseRotation()
    {
        var playerPlane = new Plane(Vector3.up, transform.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            _rbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, MovementProperties.RotationSpeed * Time.deltaTime);
        }
    }

    void Movement()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        if (vertical != 0 || horizontal != 0)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical);
            _rbody.MovePosition(transform.position + movement*this.MovementProperties.MovementSpeed*Time.deltaTime);
        }
    }

    void Shooting()
    {
        bool fireInput = Input.GetButtonDown("Fire1");

        if (!_spawnInWall)
        {
            if (fireInput)
            {
                GameObject instance =
                    Instantiate(ProjectilesProperties.BasicAttack.Missile, _missileSpawn.transform.position,
                        _missileSpawn.transform.rotation) as GameObject;

                Destroy(instance, ProjectilesProperties.BasicAttack.MissileLifetime);
            }
        }
    }

    IEnumerator MissileSpawnBoundsCheck()
    {
        for (; ; )
        {
            if (_wallBounds)
                _spawnInWall = _missileSpawnCollider.bounds.Intersects(_wallBounds.bounds);

            yield return new WaitForFixedUpdate();
        }
    }

    void DebugShooting()
    {
        if (_timeCapture == 0)
            _timeCapture = Time.time;

        if (!_spawnInWall)
        {
            if (Time.time - _timeCapture >= FireRate)
            {
                GameObject instance = Instantiate(ProjectilesProperties.BasicAttack.Missile, _missileSpawn.transform.position, _missileSpawn.transform.rotation) as GameObject;
                Destroy(instance, 1);
                _timeCapture = Time.time;
            }
        }
    }
}
