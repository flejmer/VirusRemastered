using UnityEngine;
using System.Collections;

public class PlaceToVisit
{
    public readonly Vector3 Position;
    public readonly Quaternion EndRotation;

    public PlaceToVisit(Vector3 position, Quaternion endRotation)
    {
        Position = position;
        EndRotation = endRotation;
    }
}

public class FollowPath : MonoBehaviour
{
    public GameObject TargetToFollow;

    private readonly Queue _placesToVisit = new Queue();

    private bool _inQueueMotion;
    private PlaceToVisit _currentPlaceToVisit;

    //TODO: using ques to visit places where projectile have been
    void FixedUpdate()
    {
        if (TargetToFollow)
        {
            if (_inQueueMotion)
            {
                QueueMotion();
            }
            else
            {
                transform.position = transform.position;
                transform.rotation = TargetToFollow.transform.rotation;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void QueueMotion()
    {
        if (!_currentPlaceToVisit.Equals(null))
        {
            if (!transform.position.Equals(_currentPlaceToVisit.Position))
            {
                transform.position = Vector3.Lerp(transform.position, TargetToFollow.transform.position,
                    Time.deltaTime);
            }
            else
            {
                transform.rotation = _currentPlaceToVisit.EndRotation;
                _currentPlaceToVisit = null;
            }
        }
        else
        {
            if (!_placesToVisit.Count.Equals(0))
            {
                _currentPlaceToVisit = (PlaceToVisit) _placesToVisit.Dequeue();
            }
            else
            {
                _currentPlaceToVisit = null;
                _inQueueMotion = false;
            }
        }
    }

    public void AddPlaceToVisit(PlaceToVisit place)
    {
        _placesToVisit.Enqueue(place);

        if (!_inQueueMotion)
            _inQueueMotion = true;
    }
}
