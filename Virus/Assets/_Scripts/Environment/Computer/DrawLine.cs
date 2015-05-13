using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour
{
    public Transform Origin;
    public Transform Destination;

    public float LineDrawSpeed;

    private LineRenderer _lineRenderer;
    private float _fractionOfDistance;
    private float _distance;
    private float _counter;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawToTransform();
    }

    public void SetLine(Transform origin, Transform destination, float speed)
    {
        Origin = origin;
        Destination = destination;
        LineDrawSpeed = speed;

        _lineRenderer = GetComponent<LineRenderer>();
    }

    public Transform GetDestination()
    {
        return Destination;
    }

    public void DrawToTransform()
    {
        _lineRenderer.SetPosition(0, Origin.position);

        _fractionOfDistance = 0;
        _distance = Vector3.Distance(Origin.position, Destination.position);

        if (!(_fractionOfDistance < _distance)) return;

        _counter += LineDrawSpeed * .1f * Time.deltaTime;
        _fractionOfDistance = Mathf.Lerp(0, _distance, _counter);

        var pointA = Origin.position;
        var pointB = Destination.position;

        var pointAlongLine = _fractionOfDistance * Vector3.Normalize(pointB - pointA) + pointA;

        for (var i = 1; i < 4; i++)
        {
            var pos = Vector3.Lerp(Origin.position, pointAlongLine, i / 4.0f);

            pos.x += Random.Range(-0.4f, 0.4f);
            pos.y += Random.Range(-0.4f, 0.4f);

            _lineRenderer.SetPosition(i, pos);
        }

        _lineRenderer.SetPosition(4, pointAlongLine);
    }

    public void DrawBlank()
    {
        _lineRenderer.SetPosition(0, Origin.position);
        _lineRenderer.SetPosition(1, Origin.position);
        _lineRenderer.SetPosition(2, Origin.position);
        _lineRenderer.SetPosition(3, Origin.position);
        _lineRenderer.SetPosition(4, Origin.position);
    }

}
