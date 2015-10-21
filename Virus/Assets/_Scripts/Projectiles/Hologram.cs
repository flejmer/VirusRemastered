using UnityEngine;
using System.Collections;

public class Hologram : MonoBehaviour
{
    public float Speed;
    public Vector3 _target;

    public float ScaleSpeed = 1;
    public Vector3 TargerScale = new Vector3(1, 1, 1);

    private bool _active = true;
    private Collider _col;

    void Start()
    {
        _col = GetComponent<CapsuleCollider>();
        _col.enabled = false;
        transform.localScale = new Vector3(.05f, .05f, .05f);
    }

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, TargerScale, Time.deltaTime*ScaleSpeed);
    }

    void FixedUpdate()
    {
        if (_active)
        {
            var direction = (_target - transform.position).normalized;

            var distance = (_target - transform.position).sqrMagnitude;
            var newPosition = transform.position + direction * Speed * Time.deltaTime;
            var newDistance = (newPosition - transform.position).sqrMagnitude;

            if (newDistance < distance)
            {
                transform.position = newPosition;
            }
            else
            {
                transform.position = _target;
                OnFinish();
            }
        }
    }

    private void OnFinish()
    {
        _active = false;
        _col.enabled = true;

        Destroy(gameObject, 5);
    }
}
