using UnityEngine;
using System.Collections;
using System;

public class RayhitObj : IComparable<RayhitObj>
{
    private readonly GameObject _gameObject;
    public GameObject GObject { get { return _gameObject; } }

    private readonly Vector3 _point;
    public Vector3 HitPoint { get { return _point; } }

    private readonly float _dist;

    public RayhitObj(GameObject obj, float distance, Vector3 hitPoint)
    {
        _point = hitPoint;
        _gameObject = obj;
        _dist = distance;
    }

    public int CompareTo(RayhitObj x)
    {
        return _dist.CompareTo(x._dist);
    }
}
