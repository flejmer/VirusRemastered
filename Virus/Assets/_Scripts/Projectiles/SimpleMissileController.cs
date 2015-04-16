using UnityEngine;
using System.Collections;

public class SimpleMissileController : MonoBehaviour
{
    public float missileSpeed = 20;

    private float rayLenght = 0.75f;

    void Start()
    {
    }

    void Update()
    {
        FireRay(true, transform.TransformDirection(Vector3.forward));
        FireRay(true, transform.TransformDirection(Vector3.back));
        FireRay(true, transform.TransformDirection(Vector3.left));
        FireRay(true, transform.TransformDirection(Vector3.right));
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * missileSpeed * Time.deltaTime, this.transform);
    }

    void OnTriggerEnter(Collider other)
    {
    }

    void FireRay(bool debug, Vector3 dir)
    {
        if (debug)
            Debug.DrawRay(transform.position, dir * rayLenght, Color.red);

        if (Physics.Raycast(transform.position, dir, rayLenght))
            print("Ray hit something");
    }
}
