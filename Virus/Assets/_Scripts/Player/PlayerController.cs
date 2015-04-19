using UnityEngine;
using System.Collections;

[System.Serializable]
public class Movement
{
    public float movementSpeed = 15;
    public float rotationSpeed = 10;
}

[System.Serializable]
public class Shooting
{
    public BasicAttack basicAttack;
}

[System.Serializable]
public class BasicAttack
{
    public GameObject missile;
    public float missileLifetime = 10;
}

public class PlayerController : MonoBehaviour
{
    public Movement movement;
    public Shooting shooting;

    private Rigidbody rbody;
    private GameObject missileSpawn;

    private Collider missileSpawnCollider;
    private Collider wallBounds;
    private bool spawnInTheWall = false;




    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        missileSpawn = GameObject.Find("/PlayerV0/Body/Gun/missileSpawn");
        missileSpawnCollider = missileSpawn.GetComponent<Collider>();

        StartCoroutine("missileSpawnBoundsCheck");
    }

    void Update()
    {
        Shooting();
    }

    void OnTriggerStay(Collider other)
    {
        if (missileSpawnCollider.bounds.Intersects(other.bounds))
        {
            wallBounds = other;
        }
    }

    void FixedUpdate()
    {
        MouseRotation();
        Movement();
    }

    void MouseRotation()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            rbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, movement.rotationSpeed * Time.deltaTime);
        }
    }

    void Movement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical != 0 || horizontal != 0)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical);
            rbody.MovePosition(transform.position + movement * this.movement.movementSpeed * Time.deltaTime);
        }
    }

    void Shooting()
    {
        bool fireInput = Input.GetButtonDown("Fire1");

        if (fireInput)
        {
            if (!spawnInTheWall)
            {
                GameObject instance = GameObject.Instantiate(shooting.basicAttack.missile, missileSpawn.transform.position, missileSpawn.transform.rotation) as GameObject;
                Destroy(instance, shooting.basicAttack.missileLifetime);
            }
        }
    }

    IEnumerator missileSpawnBoundsCheck()
    {
        for (; ; )
        {
            if (wallBounds)
            {
                if (missileSpawnCollider.bounds.Intersects(wallBounds.bounds))
                {
                    spawnInTheWall = true;
                }
                else
                {
                    spawnInTheWall = false;
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}
