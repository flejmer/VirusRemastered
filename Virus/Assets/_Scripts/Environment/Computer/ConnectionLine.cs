using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class ConnectionLine : DelayedActivation
{
    public Transform Origin;
    public Transform Destination;
    public int SizeOfLineElementsPos = 5;
    public float AnimationDuration = 1;

    private LineRenderer _lineRenderer;
    private int _lrSize;

    private IEnumerator _positionsUpdater;

    private Vector3 _originPos;
    private Vector3 _destinationPos;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetVertexCount(SizeOfLineElementsPos);
        _lrSize = SizeOfLineElementsPos;

        _positionsUpdater = PositionsUpdater();
        StartCoroutine(_positionsUpdater);
    }

    void OnEnable()
    {
        SetActivationDuration(AnimationDuration);

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
        if (type.Equals(Enums.AnimType.FromOriginToDestination))
        {
            StartActivation();
        }
        else
        {
            StartDeactivation();
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

    void AnimationEnd()
    {
        StartCoroutine(_positionsUpdater);
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

        _destinationPos = Math.Abs(GetActivationProgress()) < 0.05f ? new Vector3(_originPos.x, _originPos.y, _originPos.z) : Vector3.Lerp(_originPos, _destinationPos, GetActivationProgress());

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

            if (dir.Equals(Vector3.zero) || _destinationPos.Equals(_originPos))
            {
                tempTransform.rotation = Quaternion.identity;
            }
            else
            {
                tempTransform.rotation = Quaternion.LookRotation(dir);
            }


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
