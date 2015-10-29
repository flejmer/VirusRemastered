﻿using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour
{
    public Node UpNode;
    public Node DownNode;
    public Node LeftNode;
    public Node RightNode;

    public AudioSource AudioSource { get; private set; }

    public bool Active;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }
}