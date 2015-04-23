using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform TargetToFollow;
    public bool CameraSmoothness = true;
    public float SmoothnessStrength = 20;

    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - TargetToFollow.transform.position;
    }

    void LateUpdate()
    {
        if (TargetToFollow)
        {
            var destination = TargetToFollow.position + _offset;
            transform.position = CameraSmoothness ? Vector3.Lerp(transform.position, destination, SmoothnessStrength * Time.deltaTime) : destination;
        }
        else
        {
            Debug.Log("No target to follow chosen");
        }
    }
}
