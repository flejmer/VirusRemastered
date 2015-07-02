using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MovementProperties
{
    public float MovementSpeed = 15;
    public float RotationSpeed = 10;
}

[System.Serializable]
public class ProjectilesProperties
{
    public BasicAttack BasicAttack;
}

[System.Serializable]
public class BasicAttack
{
    public GameObject Missile;
    public float MissileLifetime = 10;
    public GameObject Follower;
}

public class PlayerController : MonoBehaviour
{
    public MovementProperties MovementProperties;
    public ProjectilesProperties ProjectilesProperties;

    private Rigidbody _rbody;

    private GameObject _gun;
    private GameObject _missileSpawn;

    private bool _spawnInWall;

    private Animator _anim;

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _gun = GameObject.Find("Body/Gun");
        _missileSpawn = GameObject.Find("Body/Gun/missileSpawn");
        _anim = GetComponentInChildren<Animator>();
    }


    void OnEnable()
    {
        GameManager.SetPlayer(this);
    }

    void OnDisable()
    {
        GameManager.SetPlayer(null);
    }

    void Update()
    {
        Shooting();
        Interaction();
    }

    void FixedUpdate()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        MouseRotation();

        var dirLocal = Movement(vertical, horizontal);

        horizontal = dirLocal.x;
        vertical = dirLocal.z;

        if(_anim != null)
            Animations(vertical, horizontal);
    }

    void Interaction ()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameManager.GetComputersInPlayerInterRange(this).Count > 0)
        {
            foreach (var computer in GameManager.GetComputersInPlayerInterRange(this).Where(computer => !computer.IsHacked))
            {
                computer.StartHacking(this);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Computer") || other.CompareTag("Obstacle"))
        {
            if (IsInvoking("UpdateSpawnInWall"))
                CancelInvoke("UpdateSpawnInWall");

            _spawnInWall = true;

            Invoke("UpdateSpawnInWall", Time.deltaTime + .05f);
        }
    }

    void UpdateSpawnInWall()
    {
        _spawnInWall = false;
    }

    void MouseRotation()
    {
        var playerPlane = new Plane(Vector3.up, transform.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitdist = 0.0f;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            var targetPoint = ray.GetPoint(hitdist);
            var targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            _rbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, MovementProperties.RotationSpeed * Time.deltaTime);

            //            var point = new Vector3(targetPoint.x, _missileSpawn.transform.position.y, targetPoint.z);
            //            _gun.transform.LookAt(point);

            targetRotation = Quaternion.LookRotation(targetPoint - _gun.transform.position);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            _gun.transform.rotation = Quaternion.Slerp(_gun.transform.rotation, targetRotation, MovementProperties.RotationSpeed * Time.deltaTime);
        }
    }

    Vector3 Movement(float vertical, float horizontal)
    {
        if (Math.Abs(vertical) > 0.05f || Math.Abs(horizontal) > 0.05f)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical);

            transform.Translate(movement * MovementProperties.MovementSpeed * Time.deltaTime, Space.World);
//            _rbody.MovePosition(transform.position + movement * MovementProperties.MovementSpeed * Time.deltaTime);

            Vector3 movRelative = transform.InverseTransformDirection(movement);

            return movRelative;
        }

        return Vector3.zero;
    }

    void Animations(float vertical, float horizontal)
    {
        //Math.Abs(vertical) < 0.05f && Math.Abs(horizontal) < 0.05f
        if (Math.Abs(vertical) < 0.05f && Math.Abs(horizontal) < 0.05f)
        {
            _anim.SetBool("NotMoving", true);
        }
        else
        {
            _anim.SetBool("NotMoving", false);
            _anim.SetFloat("Vertical", vertical);
            _anim.SetFloat("Horizontal", horizontal);
        }
    }

    void Shooting()
    {

        bool fireInput = Input.GetButtonDown("Fire1");

        if (fireInput)
        {


            if (!_spawnInWall)
            {
                CancelInvoke("Aiming");

                _anim.SetBool("Aiming", true);
                _anim.SetTrigger("FireGun");

                var instance = (GameObject)
                    Instantiate(ProjectilesProperties.BasicAttack.Missile, _missileSpawn.transform.position,
                        _missileSpawn.transform.rotation);

//                var follower =
//                    (GameObject)
//                        Instantiate(ProjectilesProperties.BasicAttack.Follower, _missileSpawn.transform.position,
//                            _missileSpawn.transform.rotation);
//
//                follower.GetComponent<FollowPath>().TargetToFollow = instance;

                Invoke("Aiming", 1.5f);

                Destroy(instance, ProjectilesProperties.BasicAttack.MissileLifetime);
            }
        }
    }

    void Aiming()
    {
        _anim.SetBool("Aiming", false);
    }
}
