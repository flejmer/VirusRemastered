﻿using UnityEngine;
using System.Collections;

public class CompBuffRange : MonoBehaviour
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
        Debug.Log("enter range" + other.name);
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("stay range" + other.name);
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit range" + other.name);
    }
}
