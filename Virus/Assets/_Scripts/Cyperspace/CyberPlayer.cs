using UnityEngine;
using System.Collections;

public class CyberPlayer : MonoBehaviour
{ 
    public float Speed = 3f;
    private bool _reachedDestination = true;

    [SerializeField]
    private Node _currentNode;
    private Node _targetNode;

    void FixedUpdate()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");

        if (_reachedDestination)
        {
            if (vertical > 0 || vertical < 0)
            {
                _targetNode = vertical > 0 ? _currentNode.UpNode : _currentNode.DownNode;
            }
            else if (horizontal > 0 || horizontal < 0)
            {
                _targetNode = horizontal > 0 ? _currentNode.RightNode : _currentNode.LeftNode;
            }

            if (_targetNode != null && !_targetNode.Equals(_currentNode))
                _reachedDestination = false;
        }
        else
        {
            Move();
        }

    }

    void OnTriggerEnter(Collider c)
    {
        if (!c.CompareTag("Firewall")) return;

        _targetNode = _currentNode;
        _reachedDestination = false;
    }

    private void Move()
    {
        if (!_targetNode.Active || !_currentNode.Active) return;

        var direction = _targetNode.transform.position - transform.position;
        var distance = direction.magnitude;
        direction = direction.normalized;

        var moveStep = Speed * Time.deltaTime;

        if (moveStep > distance)
            moveStep = distance;

        transform.Translate(direction * moveStep);

        if (_targetNode.transform.position != transform.position) return;

        _reachedDestination = true;
        _currentNode = _targetNode;
    }


}
