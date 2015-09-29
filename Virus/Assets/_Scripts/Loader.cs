using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;			//GameManager prefab to instantiate.
//    public GameObject soundManager;			//SoundManager prefab to instantiate.
    public GameObject laser;


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

        if (LaserPreAlloc.IsInstanceNull())
        {
            Instantiate(laser);
        }
    }
}
