using UnityEngine;
using System.Collections;

public class CompInterArea : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter inter" + other.name);
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("stay inter" + other.name);
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit inter" + other.name);
    }
}
