using System;
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform TargetToFollow;
    public bool CameraSmoothness = true;
    public float SmoothnessStrength = 2000;

    public bool MouseDependency = false;

    private Vector3 _offset;
    private Camera _thisCamera;

    private bool _dragMode;

    void Start()
    {
        _offset = transform.position - TargetToFollow.transform.position;
        _thisCamera = GetComponent<Camera>();

        if (!TargetToFollow.CompareTag("CyberPlayer")) return;

        _offset.x = 0;
        _offset.z = 0;
    }

    void LateUpdate()
    {
        if (TargetToFollow)
        {
            if (TargetToFollow.CompareTag("CyberPlayer"))
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    var size = _thisCamera.orthographicSize + 2;

                    if (size < 20)
                        _thisCamera.orthographicSize += 2;

                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    var size = _thisCamera.orthographicSize - 2;

                    if (size > 10)
                        _thisCamera.orthographicSize += -2;
                }

                var dragOrigin = new Vector3(Screen.width / 2, Screen.height / 2, 0);

                if (Input.GetMouseButtonDown(0))
                {
                    _dragMode = true;
                    SmoothnessStrength = 10;
                }

                if (!Input.GetMouseButton(0))
                {
                    if (_dragMode)
                        _dragMode = false;
                }


                if (_dragMode)
                {
                    var pos = this.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                    var move = new Vector3(pos.x * 1, 0, pos.y * 1);

                    transform.Translate(move, Space.World);
                }
                else
                {
                    CameraMovement();
                }
            }
            else
            {
                CameraMovement();
            }
        }
        else
        {
            Debug.Log("No target to follow chosen");
        }
    }

    void CameraMovement()
    {
        var destination = TargetToFollow.transform.position + _offset;

        if (MouseDependency)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 1;

            var cursorPosition = Camera.main.ScreenToWorldPoint(mousePos);
            destination =
                new Vector3((TargetToFollow.transform.position.x + cursorPosition.x) / 2,
                    TargetToFollow.transform.position.y,
                    (TargetToFollow.transform.position.z + cursorPosition.z) / 2) + _offset;
        }


        transform.position = CameraSmoothness ? Vector3.Lerp(transform.position, destination, SmoothnessStrength * Time.deltaTime) : destination;

        if (Math.Abs((TargetToFollow.position + _offset - transform.position).sqrMagnitude) < 0.1)
        {
            SmoothnessStrength = 20000;
        }
    }

    public void ChangeTarget(Transform newTarget)
    {
        TargetToFollow = newTarget;
        SmoothnessStrength = 10;
    }

    public void BackToPlayer()
    {
        BackToPlayer(0);
    }

    public void BackToPlayer(float delay)
    {
        StartCoroutine(ChangeToPlayerAfterDelay(delay));

    }

    IEnumerator ChangeToPlayerAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeTarget(GameManager.GetPlayer().transform);
        GameManager.GetPlayer().PlayerState = Enums.PlayerStates.RealWorld;
    }
}
