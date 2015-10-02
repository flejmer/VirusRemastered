using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Laser : MonoBehaviour
{
    public Transform MissleSpawnPoint;
    public LayerMask Mask;

    private LineRenderer _lineRend;
    private float _lineRendWidth = 1;

    public float DrawSpeed = 10;
    public float LifeTime = 3;

    private float _rayRange = 20;

    public Vector3 MoveDir { get; private set; }

    void Awake()
    {
        _lineRend = GetComponent<LineRenderer>();
        Destroy(gameObject, LifeTime);
    }

    public void FireLaser()
    {
        var direction = MissleSpawnPoint.forward.normalized;

        MoveDir = direction;

        direction = new Vector3(direction.x, direction.y, direction.z);

        var pos1 = MissleSpawnPoint.position;
        var pos2 = MissleSpawnPoint.TransformPoint(2, 0, 0);
        var pos3 = MissleSpawnPoint.TransformPoint(-2, 0, 0);


        Debug.DrawRay(pos1, MissleSpawnPoint.forward * _rayRange, Color.black, 1);
        Debug.DrawRay(pos2, MissleSpawnPoint.forward * _rayRange, Color.black, 1);
        Debug.DrawRay(pos3, MissleSpawnPoint.forward * _rayRange, Color.black, 1);

        RaycastHit[] hits1 = Physics.RaycastAll(pos1, MissleSpawnPoint.forward, _rayRange, Mask);
        RaycastHit[] hits2 = Physics.RaycastAll(pos2, MissleSpawnPoint.forward, _rayRange, Mask);
        RaycastHit[] hits3 = Physics.RaycastAll(pos3, MissleSpawnPoint.forward, _rayRange, Mask);


        Vector3 endPoint = new Vector3();

        var hitList = new List<RayhitObj>();

        if (hits1.Length > 0 || hits2.Length > 0 || hits3.Length > 0)
        {
            hitList.AddRange(hits1.Select(hit => new RayhitObj(hit.transform.gameObject, (hit.point - pos1).sqrMagnitude, hit.point)));

            foreach (var hit in hits2)
            {
                if (hit.transform.gameObject.CompareTag("EnemyGuard") || hit.transform.gameObject.CompareTag("EnemyTech"))
                {
                    hitList.Add(new RayhitObj(hit.transform.gameObject, (hit.point - pos2).sqrMagnitude, hit.point));
                }
            }

            foreach (var hit in hits3)
            {
                if (hit.transform.gameObject.CompareTag("EnemyGuard") || hit.transform.gameObject.CompareTag("EnemyTech"))
                {
                    hitList.Add(new RayhitObj(hit.transform.gameObject, (hit.point - pos3).sqrMagnitude, hit.point));
                }
            }

            hitList.TrimExcess();
            hitList.Sort();

            var obstacleCalled = false;

            for (var i = 0; i < hitList.Count; i++)
            {
                var obj = hitList[i].GObject;

                if (obj.CompareTag("EnemyGuard"))
                {
                    var enemy = obj.GetComponent<EnemySimpleAI>();

                    enemy.RemoveHp(100);
                    enemy.HitPoint(hitList[i].GObject.transform.position, MoveDir, 100, Mask);
                }
                else if (obj.CompareTag("EnemyTech"))
                {
                    var enemy = obj.GetComponent<EnemySimpleAI>();
                    enemy.RemoveHp(100);
                    enemy.HitPoint(hitList[i].GObject.transform.position, MoveDir, 100, Mask);
                }
                else if (obj.CompareTag("Obstacle") || obj.CompareTag("Untagged") || obj.CompareTag("Computer"))
                {
                    obstacleCalled = true;
                    endPoint = hitList[i].HitPoint;
                    break;
                }
            }

            if (!obstacleCalled)
            {
                endPoint = pos1 + MissleSpawnPoint.forward * _rayRange;
            }
        }
        else
        {
            endPoint = pos1 + MissleSpawnPoint.forward * _rayRange;
        }


        hitList.Clear();

        StartCoroutine(DrawAnimation(endPoint, direction));
    }

    IEnumerator DrawAnimation(Vector3 endPoint, Vector3 direction)
    {

        var startPosition = new Vector3(MissleSpawnPoint.position.x, MissleSpawnPoint.position.y, MissleSpawnPoint.position.z);
        var currentPosition = MissleSpawnPoint.position;

        var previousSqrMagnitude = (currentPosition - endPoint).sqrMagnitude;

        //        var halfWayThere = previousSqrMagnitude / 2;

        bool endReached = false;

        _lineRend.SetPosition(0, MissleSpawnPoint.position);

        Invoke("FadeInvoker", .1f);

        while (!Equals(startPosition, endPoint))
        {
            if (!Equals(currentPosition, endPoint))
                currentPosition = currentPosition + direction * DrawSpeed * Time.deltaTime;

            var currentSqrMagnitude = (currentPosition - endPoint).sqrMagnitude;

            if (currentSqrMagnitude > previousSqrMagnitude && !endReached)
            {
                currentPosition = endPoint;
                previousSqrMagnitude = (startPosition - endPoint).sqrMagnitude;
                endReached = true;

//                StartCoroutine(WidthFade());
            }

            //            if (currentSqrMagnitude <= halfWayThere || endReached)
            //            {
            //                startPosition = startPosition + direction * DrawSpeed / 5 * Time.deltaTime;
            //
            //                if (endReached)
            //                {
            //                    currentSqrMagnitude = (startPosition - endPoint).sqrMagnitude;
            //                }
            //
            //                if (currentSqrMagnitude > previousSqrMagnitude && endReached)
            //                {
            //                    startPosition = endPoint;
            //                }
            //
            //            }

            _lineRend.SetPosition(0, startPosition);
            _lineRend.SetPosition(1, currentPosition);

            previousSqrMagnitude = currentSqrMagnitude;

            yield return new WaitForFixedUpdate();
        }

    }

    void FadeInvoker()
    {
        StartCoroutine(WidthFade());
    }

    IEnumerator WidthFade()
    {
        while (_lineRendWidth != 0)
        {
            _lineRendWidth = Mathf.Lerp(_lineRendWidth, 0, Time.deltaTime * DrawSpeed / 10);
            _lineRend.SetWidth(_lineRendWidth, _lineRendWidth);

            yield return null;
        }

        _lineRend.SetPosition(0, Vector3.zero);
        _lineRend.SetPosition(1, Vector3.zero);
    }

}
