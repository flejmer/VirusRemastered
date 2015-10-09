﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;			//GameManager prefab to instantiate.
//    public GameObject soundManager;			//SoundManager prefab to instantiate.


    void Awake()
    {
        if (GameManager.IsInstanceNull())
        {
            Instantiate(gameManager);
        }

        //        if (SoundManager.Instance == null)
        //        {
        //            Instantiate(soundManager);
        //        }
    }
}
