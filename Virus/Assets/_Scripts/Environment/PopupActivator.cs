using System;
using UnityEngine;
using System.Collections;

public class PopupActivator : MonoBehaviour
{
    public string Title = "defaultTitle";
    public string Text = "defaultText";

    public bool CanBeActivated = true;
    public bool CanBeReactivated = true;
    public float ReactivationDelay = 8;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !CanBeActivated) return;

        GUIController.ActivateTextPopup(Title, Text);

        CanBeActivated = false;

        if (CanBeReactivated)
        {
            Invoke("ReactivatePopup", ReactivationDelay);
        }
    }

    void ReactivatePopup()
    {
        CanBeActivated = true;
    }
}
