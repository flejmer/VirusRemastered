using System;
using UnityEngine;
using System.Collections;

public class PopHackActivator : MonoBehaviour
{
    public float ReactivationDelay = 5;
    public bool CanBeActivated = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !CanBeActivated) return;

        GUIController.ActivateHackingInfo();

        CanBeActivated = false;

        Invoke("ReactivatePopup", ReactivationDelay);
    }

    void ReactivatePopup()
    {
        CanBeActivated = true;
    }
}
