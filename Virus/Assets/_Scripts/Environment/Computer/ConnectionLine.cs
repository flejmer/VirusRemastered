using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class ConnectionLine : DelayedActivation
{
    [SerializeField]
    private int _sizeOfLineElementsPos = 5;

    private Transform _origin;
    private Transform _destination;

    private LineRenderer _lineRenderer;
    private int _lrSize;

    private IEnumerator _positionsUpdater;

    private Vector3 _originPos;
    private Vector3 _destinationPos;

    void Awake()
    {
        _origin = transform;
        _destination = transform;

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetVertexCount(_sizeOfLineElementsPos);
        _lrSize = _sizeOfLineElementsPos;
    }

    void Start()
    {
        _positionsUpdater = PositionsUpdater();
        StartCoroutine(_positionsUpdater);
    }

    void OnEnable()
    {
        StartCoroutine(PositionsCheck());

        OnActivationStarted += AnimationStart;
        OnActivationUpdate += AnimationUpdate;
        OnActivationFinished += AnimationEnd;

        OnDeactivationStarted += AnimationStart;
        OnDeactivationUpdate += AnimationUpdate;
        OnDeactivationFinished += AnimationEnd;
    }

    void OnDisable()
    {
        OnActivationStarted -= AnimationStart;
        OnActivationUpdate -= AnimationUpdate;
        OnActivationFinished -= AnimationEnd;

        OnDeactivationStarted -= AnimationStart;
        OnDeactivationUpdate -= AnimationUpdate;
        OnDeactivationFinished -= AnimationEnd;

        ResetActivator();
        _destination = transform;
    }

    public void SetDestination(Transform destination)
    {
        if (!destination.gameObject.CompareTag("Player"))
        {
            Destroy(destination.gameObject, 10);
        }

        _destination = destination;
    }

    void Update()
    {
        DrawLine();
    }

    public void AnimateLine(Enums.AnimType type)
    {
        AnimateLine(type, _defaultDuration);
    }

    public void AnimateLine(Enums.AnimType type, float duration)
    {
        if (type.Equals(Enums.AnimType.FromOriginToDestination))
        {
            StartActivation(duration);
        }
        else
        {
            StartDeactivation(duration);
        }
    }

    void AnimationStart()
    {
        StopCoroutine(_positionsUpdater);
    }

    void AnimationUpdate()
    {
        StartCoroutine(PositionsCheck());
        _destinationPos = Vector3.Lerp(_originPos, _destinationPos, GetActivationProgress());
    }

    //TODO: Check destroy calling
    void AnimationEnd()
    {
        StartCoroutine(_positionsUpdater);

        if (Math.Abs(GetActivationProgress()) < 0.05f)
        {
            var temp = _destination;
            _destination = transform;

            if(!temp.gameObject.CompareTag("Player"))
                Destroy(temp.gameObject);
        }
    }

    IEnumerator PositionsUpdater()
    {
        for (; ; )
        {
            StartCoroutine(PositionsCheck());
            yield return new WaitForEndOfFrame();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    IEnumerator PositionsCheck()
    {
        _originPos = new Vector3(_origin.position.x, _origin.position.y, _origin.position.z);
        _destinationPos = new Vector3(_destination.position.x, _destination.position.y, _destination.position.z);

        _destinationPos = Math.Abs(GetActivationProgress()) < 0.05f ? new Vector3(_originPos.x, _originPos.y, _originPos.z) : Vector3.Lerp(_originPos, _destinationPos, GetActivationProgress());

        yield return null;
    }

    void DrawLine()
    {
        if (_sizeOfLineElementsPos != _lrSize)
        {
            _lineRenderer.SetVertexCount(_sizeOfLineElementsPos);
            _lrSize = _sizeOfLineElementsPos;
        }

        if (_origin == null || _destination == null) return;

        _lineRenderer.SetPosition(0, _originPos - transform.position);

        var distance = _destinationPos - _originPos;

        var dir = distance / (_sizeOfLineElementsPos - 1);

        for (var i = 1; i < _sizeOfLineElementsPos; i++)
        {
            var tempTransform = new GameObject().transform;
            tempTransform.position = new Vector3(_originPos.x, _originPos.y, _originPos.z);

            if (dir.Equals(Vector3.zero) || _destinationPos.Equals(_originPos))
            {
                tempTransform.rotation = Quaternion.identity;
            }
            else
            {
                tempTransform.rotation = Quaternion.LookRotation(dir);
            }

            tempTransform.position += i * dir;

            if (i < _sizeOfLineElementsPos - 1)
            {
                tempTransform.Translate(Vector3.left * Random.Range(-0.25f, 0.25f));
            }

            var pointpointAlongLine = new Vector3(tempTransform.position.x, tempTransform.position.y, tempTransform.position.z);
            pointpointAlongLine = pointpointAlongLine - transform.position;
            _lineRenderer.SetPosition(i, pointpointAlongLine);

            Destroy(tempTransform.gameObject);
        }
    }
}
