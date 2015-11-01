using UnityEngine;
using System.Collections;

public class MissionStatusUpdater : MonoBehaviour
{
    public string Status;
    public bool CanBeActivated = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !CanBeActivated) return;

        GUIController.UpdateMissionStatus(Status);
        CanBeActivated = false;
    }
}
