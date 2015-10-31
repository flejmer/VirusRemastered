using UnityEngine;
using System.Collections;

public class CyberPlayer : MonoBehaviour
{
    public float Speed = 3f;
    private bool _reachedDestination = true;

    [SerializeField]
    private Node _currentNode;
    private Node _targetNode;
    private Node _nextNode;

    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }

    public bool NotThisTime = false;

    void Update()
    {
        PlayerInteraction();
    }

    void OnEnable()
    {
        GameManager.SetCyberPlayer(this);

        _targetNode = null;
        _nextNode = null;
    }

    void OnDisable()
    {
        GameManager.SetCyberPlayer(null);
    }

    void FixedUpdate()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");

        if (_reachedDestination)
        {
            if (_nextNode == null)
            {
                if (vertical > 0 || vertical < 0)
                {
                    _targetNode = vertical > 0 ? _currentNode.UpNode : _currentNode.DownNode;
                }
                else if (horizontal > 0 || horizontal < 0)
                {
                    _targetNode = horizontal > 0 ? _currentNode.RightNode : _currentNode.LeftNode;
                }
            }
            else
            {
                _targetNode = _nextNode;
            }

            if (_targetNode != null && !_targetNode.Equals(_currentNode))
                _reachedDestination = false;
        }
        else
        {
            Move();

            var direction = _targetNode.transform.position - transform.position;
            var distance = direction.magnitude;

            if (distance < 0.5f)
            {
                if (vertical > 0 || vertical < 0)
                {
                    _nextNode = vertical > 0 ? _targetNode.UpNode : _targetNode.DownNode;
                }
                else if (horizontal > 0 || horizontal < 0)
                {
                    _nextNode = horizontal > 0 ? _targetNode.RightNode : _targetNode.LeftNode;
                }
            }
        }
    }

    void PlayerInteraction()
    {
        if(GUIController.IsPopupActivated()) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_reachedDestination) return;

            if (_currentNode.CompareTag("InteractionNode"))
            {
                var node = _currentNode.GetComponent<InteractionNode>();
                node.UnlockNode();
            }
            else if (_currentNode.CompareTag("ComputerNode"))
            {
                var node = _currentNode.GetComponent<ComputerNode>();

                RealCyberManager.GoToRealWorld(node);
                //                node.Overload = true;
            }
            else if (_currentNode.CompareTag("AbilityNode"))
            {
                var node = _currentNode.GetComponent<AbilityNode>();

                if (!node.Unlocked)
                    node.Unlocked = true;
                else
                {
                    if(!NotThisTime)
                        node.UnlockAbility();
                }
            }
        }

        NotThisTime = false;
    }

    void OnTriggerEnter(Collider c)
    {
        if (!c.CompareTag("Firewall")) return;

        _targetNode = _currentNode;
        _reachedDestination = false;
        _nextNode = null;
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

        _currentNode = _targetNode;

        if (_nextNode != null)
        {
            if (_currentNode.Equals(_nextNode))
            {
                _reachedDestination = true;
                _nextNode = null;
            }
            else
                _targetNode = _nextNode;

            return;
        }

        _reachedDestination = true;
    }


}
