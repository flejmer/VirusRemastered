using System;
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
                    new Vector3((TargetToFollow.transform.position.x + cursorPosition.x) / 2,
                        TargetToFollow.transform.position.y,
                        (TargetToFollow.transform.position.z + cursorPosition.z) / 2) + _offset;
            }


            transform.position = CameraSmoothness ? Vector3.Lerp(transform.position, destination, SmoothnessStrength * Time.deltaTime) : destination;

            if (Math.Abs((TargetToFollow.position + _offset - transform.position).sqrMagnitude) < 0.1)
            {
                SmoothnessStrength = 200;
            }
        }
        else
        {
            Debug.Log("No target to follow chosen");
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
