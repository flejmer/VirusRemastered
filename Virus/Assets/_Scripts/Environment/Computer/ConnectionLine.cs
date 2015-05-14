using UnityEngine;
using System.Collections;

public class ConnectionLine : MonoBehaviour
{
    public Transform Origin;
    public Transform Destination;
    public int SizeOfLineElementsPos = 5;
    public float AnimationTime = 5;

    private LineRenderer _lineRenderer;
    private int _lrSize;

    private Vector3 _originPos;
    private Vector3 _destinationPos;

    private float _animStartTime;
    private float _animFinishTime;
    private bool _animInProgress;
    private bool _connAnim;

    private IEnumerator _corutime;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetVertexCount(SizeOfLineElementsPos);
        _lrSize = SizeOfLineElementsPos;

        StartCoroutine(PositionsCheck());
    }

    void Update()
    {

        DrawLine();

        if (Input.GetKeyDown(KeyCode.F) && !_animInProgress)
        {
            _corutime = ConnectAnimation(AnimationTime, true);
            StartCoroutine(_corutime);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (_animInProgress)
            {
                _animFinishTime = _animStartTime + ((Time.time - _animStartTime) / AnimationTime) * 2 * AnimationTime;
//                _animStartTime = -1 + _animStartTime;
                _connAnim = false;
            }
        }
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



    IEnumerator PositionsUpdate()
    {

        for (; ; )
        {
            Debug.Log("unkillable");

            StartCoroutine(PositionsCheck());
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator PositionsCheck()
    {
        _originPos = new Vector3(Origin.position.x, Origin.position.y, Origin.position.z);
        _destinationPos = new Vector3(Destination.position.x, Destination.position.y, Destination.position.z);
        yield return null;
    }

    IEnumerator ConnectAnimation(float animTime, bool connAnim)
    {
        _connAnim = connAnim;

        _animStartTime = Time.time;

        if (!_animInProgress)
        {
            _animInProgress = true;
            _animFinishTime = Time.time + animTime;
        }

        while (_animFinishTime >= Time.time)
        {


            var animationProgress = _connAnim ? (Time.time - _animStartTime) / animTime : 1 - (Time.time - _animStartTime) / animTime;

            StartCoroutine(PositionsCheck());
            _destinationPos = Vector3.Lerp(_originPos, Destination.position, animationProgress);

            yield return new WaitForFixedUpdate();
        }

        _animInProgress = false;
    }
}
