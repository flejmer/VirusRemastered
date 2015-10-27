using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TechActivator : MonoBehaviour
{
    public List<EnemyTechAI> TechList = new List<EnemyTechAI>();

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var ai in TechList)
        {
            ai.Active = true;
        }
    }
}
