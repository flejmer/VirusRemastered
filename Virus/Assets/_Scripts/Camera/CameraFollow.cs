using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform TargetToFollow;
    public bool CameraSmoothness = true;
    public float SmoothnessStrength = 200;

    public bool MouseDependency = false;

    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - TargetToFollow.transform.position;

        if (!TargetToFollow.CompareTag("CyberPlayer")) return;

        _offset.x = 0;
        _offset.z = 0;
    }

    void LateUpdate()
    {
        if (TargetToFollow)
        {
            var destination = TargetToFollow.transform.position + _offset;

            if (MouseDependency)
            {
                    var mousePos = Input.mousePosition;
                    mousePos.z = 1;

                    var cursorPosition = Camera.main.ScreenToWorldPoint(mousePos);
                    destination =
                        new Vector3((TargetToFollow.transform.position.x + cursorPosition.x)/2,
                            TargetToFollow.transform.position.y,
                            (TargetToFollow.transform.position.z + cursorPosition.z)/2) + _offset;
            }


            transform.position = CameraSmoothness ? Vector3.Lerp(transform.position, destination, SmoothnessStrength * Time.deltaTime) : destination;

        }
        else
        {
            Debug.Log("No target to follow chosen");
        }
    }
}
