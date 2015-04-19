using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform targetToFollow;
    public bool cameraSmoothness = true;
    public float smoothnessStrength = 20;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - targetToFollow.transform.position;
    }

    void LateUpdate()
    {
        if (targetToFollow)
        {
            Vector3 destination = targetToFollow.position + offset;

            if (cameraSmoothness)
            {
                transform.position = Vector3.Lerp(transform.position, destination, smoothnessStrength * Time.deltaTime);
            }
            else
            {
                transform.position = destination;
            }
        }
        else
        {
            Debug.Log("No target to follow chosen");
        }
    }
}
