using UnityEngine;
using System.Collections;

public class SimpleMissileController : MonoBehaviour
{
    public float missileSpeed = 20;
    public GameObject missile;

    public bool bouncy = false;
    public int maxBounces = 5;

    private int bouncesCount = 0;

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Collider collid;
    private Rigidbody myRigidbody;

    private Vector3 moveDir;
    private ParticleSystem pSys;
    private Component hlo;
    private LineRenderer lineR;
    private Light lght;
    private Collider cld;

    private Rigidbody rbody;
    private Vector3 movementThisStep;
    private float movementSqrMagnitude;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        myRigidbody = rbody;
        previousPosition = myRigidbody.position;
        collid = GetComponent<Collider>();
        minimumExtent = Mathf.Min(Mathf.Min(collid.bounds.extents.x, collid.bounds.extents.y), collid.bounds.extents.z);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    void Start()
    {
        moveDir = Vector3.forward;

        pSys = GetComponentInChildren<ParticleSystem>();
        hlo = GetComponent("Halo");
        lineR = GetComponent<LineRenderer>();
        lght = GetComponent<Light>();
        cld = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter()
    {
        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            backupObstacleHit();
        }
        else
        {
            obstacleHit();
        }
    }

    void FixedUpdate()
    {
        transform.Translate(moveDir * missileSpeed * Time.deltaTime, this.transform);

        movementThisStep = myRigidbody.position - previousPosition;
        movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            backupObstacleHit();
        }

        previousPosition = myRigidbody.position;
    }

    void obstacleHit()
    {
        Vector3 movementThisStep = transform.TransformDirection(Vector3.forward);
        float movementMagnitude = movementThisStep.magnitude;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, movementThisStep, out hitInfo, movementMagnitude))
        {
            if (bouncy && bouncesCount <= maxBounces)
            {
                Debug.DrawRay(transform.position, movementThisStep * movementMagnitude, Color.cyan, 2);
                Vector3 reflected = Vector3.Reflect((hitInfo.point - transform.position).normalized, hitInfo.normal);
                Debug.DrawRay(hitInfo.point, reflected * 2, Color.cyan, 2);

                Quaternion lookAt = Quaternion.LookRotation(reflected);
                rbody.rotation = lookAt;

                bouncesCount++;
            }
            else
            {
                stopProjectile();
            }
        }
    }

    void backupObstacleHit()
    {
        Vector3 movementThisStep = myRigidbody.position - previousPosition;
        float movementMagnitude = movementThisStep.magnitude;
        RaycastHit hitInfo;

        if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude))
        {
            if (bouncy && bouncesCount <= maxBounces)
            {
                Debug.DrawRay(previousPosition, movementThisStep * movementMagnitude, Color.cyan, 2);
                Vector3 reflected = Vector3.Reflect((hitInfo.point - transform.position).normalized, hitInfo.normal);
                Debug.DrawRay(hitInfo.point, -reflected * 2, Color.cyan, 2);

                Quaternion lookAt = Quaternion.LookRotation(-reflected);
                rbody.rotation = lookAt;

                bouncesCount++;
            }
            else
            {
                stopProjectile();
            }
        }
    }

    void stopProjectile()
    {
        moveDir = Vector3.zero;

        if (cld)
            cld.enabled = false;
        if (pSys)
            pSys.emissionRate = 0;
        if (hlo)
            hlo.GetType().GetProperty("enabled").SetValue(hlo, false, null);
        if (lineR)
            lineR.enabled = false;
        if (lght)
            lght.enabled = false;
    }
}
