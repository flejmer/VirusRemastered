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

    public DoorDownController BoundDoors;

    void Update()
    {
        if (BoundDoors.GetLockType().Equals(Enums.DoorLockType.Unlocked) ||
            BoundDoors.GetLockType().Equals(Enums.DoorLockType.OpenForever))
        {
            CancelInvoke();
            CanBeActivated = false;
            CanBeReactivated = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !CanBeActivated) return;

        Title = Title.Replace("NEWLINE", "\n");
        Text = Text.Replace("NEWLINE", "\n");

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
