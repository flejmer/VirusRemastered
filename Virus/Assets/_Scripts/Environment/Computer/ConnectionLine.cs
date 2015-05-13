using UnityEngine;
using System.Collections;

public class ConnectionLine : MonoBehaviour
{
    public Transform Origin;
    public Transform Destination;
    public int SizeOfLineElementsPos = 5;

    private LineRenderer _lineRenderer;
    private int _lrSize;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetVertexCount(SizeOfLineElementsPos);
        _lrSize = SizeOfLineElementsPos;
    }

    void Update()
    {
        DrawLine();
    }


    void DrawLine()
    {
        if (SizeOfLineElementsPos != _lrSize)
        {
            _lineRenderer.SetVertexCount(SizeOfLineElementsPos);
            _lrSize = SizeOfLineElementsPos;
        }

        if (Origin == null || Destination == null) return;

        _lineRenderer.SetPosition(0, Origin.position);

        var distance = Destination.position - Origin.position;
        var dir = distance / (SizeOfLineElementsPos - 1);

        for (var i = 1; i < SizeOfLineElementsPos; i++)
        {
            var tempTransform = new GameObject().transform;
            tempTransform.position = new Vector3(Origin.position.x, Origin.position.y, Origin.position.z);
            tempTransform.rotation = Quaternion.LookRotation(dir);
            tempTransform.position += i * dir;

            if (i < SizeOfLineElementsPos-1)
            {
                tempTransform.Translate(Vector3.left * Random.Range(-0.4f, 0.4f));
            }

            var pointpointAlongLine = new Vector3(tempTransform.position.x, tempTransform.position.y, tempTransform.position.z);
            _lineRenderer.SetPosition(i, pointpointAlongLine);

            Destroy(tempTransform.gameObject);
        }
    }
}
