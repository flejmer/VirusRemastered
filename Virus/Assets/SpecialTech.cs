using UnityEngine;
using System.Collections;

public class SpecialTech : MonoBehaviour
{
    public DoorDownController Doors;
    private EnemyTechAI _tech;
    private bool _activated;

    // Use this for initialization
    void Start()
    {
        _tech = GetComponent<EnemyTechAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_activated || !_tech.Active) return;

        _activated = true;
        Doors.SetLockType(Enums.DoorLockType.OpenForever);
    }
}
