using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 15;
    public float rotationSpeed = 10;

    public GameObject missile;
    public float missileSpeed = 1500;
    public float missileLifetime = 10;

    private Rigidbody rbody;
    private GameObject missileSpawn;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        missileSpawn = GameObject.Find("/PlayerV0/Body/Gun/missileSpawn");
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

    void MouseRotation()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            rbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Movement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical != 0 || horizontal != 0)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical);
            rbody.MovePosition(transform.position + movement * movementSpeed * Time.deltaTime);
        }
    }

    void Shooting()
    {
        bool fireInput = Input.GetButtonDown("Fire1");

        if (fireInput)
        {
            GameObject instance = GameObject.Instantiate(missile, missileSpawn.transform.position, missileSpawn.transform.rotation) as GameObject;
            instance.GetComponent<Rigidbody>().AddForce(missileSpawn.transform.forward * missileSpeed);
            Destroy(instance, missileLifetime);
        }
    }
}
