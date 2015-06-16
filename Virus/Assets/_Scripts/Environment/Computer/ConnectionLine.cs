using UnityEngine;
using System.Collections;

public class ConnectionLine : DelayedActivation
{
    public Transform Origin;
    public Transform Destination;
    public int SizeOfLineElementsPos = 5;
    public float AnimationDuration = 5;

    private LineRenderer _lineRenderer;
    private int _lrSize;

    private IEnumerator _positionsUpdater;
    private IEnumerator _animationCoroutine;

    private Vector3 _originPos;
    private Vector3 _destinationPos;

    private bool _animationInProgress;
    private float _animProgressTime;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetVertexCount(SizeOfLineElementsPos);
        _lrSize = SizeOfLineElementsPos;

        _positionsUpdater = PositionsUpdater();
        StartCoroutine(_positionsUpdater);
    }

    void Update()
    {
        DrawLine();

        if (Input.GetKeyDown(KeyCode.F))
        {
            AnimateLine(Enums.AnimType.FromOriginToDestination);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            AnimateLine(Enums.AnimType.FromDestinationToOrigin);
        }
    }

    void AnimateLine(Enums.AnimType type)
    {
        StartCoroutine(type.Equals(Enums.AnimType.FromOriginToDestination) ? AnimateFromOriginToDestination() : AnimateFromDestinationToOrigin());
    }

    void AnimationStart()
    {
        StopCoroutine(_positionsUpdater);
        _animationInProgress = true;
    }

    void AnimationEnd()
    {
        StartCoroutine(_positionsUpdater);
        _animationInProgress = false;
    }

    IEnumerator AnimateFromOriginToDestination()
    {
        if (!_animationInProgress)
        {
            _animationCoroutine = AnimateFromOriginToDestination(0);
        }
        else
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = AnimateFromOriginToDestination(_animProgressTime);
        }

        StartCoroutine(_animationCoroutine);

        yield return null;
    }

    IEnumerator AnimateFromDestinationToOrigin()
    {
        if (!_animationInProgress)
        {
            _animationCoroutine = AnimateFromDestinationToOrigin(1);
        }
        else
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = AnimateFromDestinationToOrigin(_animProgressTime);
        }

        StartCoroutine(_animationCoroutine);
        yield return null;
    }

    IEnumerator AnimateFromOriginToDestination(float animProgress)
    {
        AnimationStart();

        _animProgressTime = animProgress;

        while (_animProgressTime < 1)
        {
            StartCoroutine(PositionsCheck());
            _destinationPos = Vector3.Lerp(_originPos, _destinationPos, _animProgressTime);

            _animProgressTime += Time.deltaTime / AnimationDuration;
            yield return new WaitForEndOfFrame();
        }

        AnimationEnd();
    }

    IEnumerator AnimateFromDestinationToOrigin(float animProgress)
    {
        AnimationStart();

        _animProgressTime = animProgress;

        while (_animProgressTime > 0)
        {
            StartCoroutine(PositionsCheck());
            _destinationPos = Vector3.Lerp(_originPos, _destinationPos, _animProgressTime);

            _animProgressTime -= Time.deltaTime / AnimationDuration;
            yield return new WaitForEndOfFrame();
        }

        AnimationEnd();
    }

    IEnumerator PositionsUpdater()
    {
        for (; ; )
        {
            StartCoroutine(PositionsCheck());
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PositionsCheck()
    {
        _originPos = new Vector3(Origin.position.x, Origin.position.y, Origin.position.z);
        _destinationPos = new Vector3(Destination.position.x, Destination.position.y, Destination.position.z);
        yield return null;
    }

    void DrawLine()
    {
        if (SizeOfLineElementsPos != _lrSize)
        {
            _lineRenderer.SetVertexCount(SizeOfLineElementsPos);
            _lrSize = SizeOfLineElementsPos;
        }

        if (Origin == null || Destination == null) return;

        _lineRenderer.SetPosition(0, _originPos);

        var distance = _destinationPos - _originPos;
        var dir = distance / (SizeOfLineElementsPos - 1);

        for (var i = 1; i < SizeOfLineElementsPos; i++)
        {
            var tempTransform = new GameObject().transform;
            tempTransform.position = new Vector3(_originPos.x, _originPos.y, _originPos.z);
            tempTransform.rotation = dir.Equals(Vector3.zero) ? Quaternion.identity : Quaternion.LookRotation(dir);
            tempTransform.position += i * dir;

            if (i < SizeOfLineElementsPos - 1)
            {
                tempTransform.Translate(Vector3.left * Random.Range(-0.4f, 0.4f));
            }

            var pointpointAlongLine = new Vector3(tempTransform.position.x, tempTransform.position.y, tempTransform.position.z);
            _lineRenderer.SetPosition(i, pointpointAlongLine);

            Destroy(tempTransform.gameObject);
        }
    }
}
