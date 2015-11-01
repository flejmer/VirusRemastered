﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LaserUnlockedPopup : MonoBehaviour
{
    private Text _energy;
    // Use this for initialization
    void Awake()
    {
        _energy = transform.FindChild("Energy").gameObject.GetComponent<Text>();
        _energy.text = "Energy cost: " + (int) (RealCyberManager.GetPlayer().ProjectilesProperties.LaserAttack.EnergyCost/10) +
                       " bars";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
