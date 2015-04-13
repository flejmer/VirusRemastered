using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 15;
    public float rotationSpeed = 10;

    private Rigidbody rbody;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
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

        if(vertical != 0 || horizontal != 0)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical);
            rbody.MovePosition(transform.position + movement * movementSpeed * Time.deltaTime);
        }
    }
}
