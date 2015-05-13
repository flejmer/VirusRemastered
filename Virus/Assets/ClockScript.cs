using System;
using UnityEngine;
using System.Collections;

public class ClockScript : MonoBehaviour
{
    public Transform Minutes;
    public Transform Seconds;
    public Transform Hours;

    private const float SecondsToDegrees = -6;
    private const float MinutesToDegrees = -6;
    private const float HoursToDegrees = -30;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        Clock();
    }

    void Clock()
    {
        var time = DateTime.Now;

        Minutes.localRotation = Quaternion.Euler(new Vector3(time.Minute * MinutesToDegrees, 0, 0));
        Seconds.localRotation = Quaternion.Euler(new Vector3(time.Second * SecondsToDegrees, 0, 0));
        Hours.localRotation = Quaternion.Euler(new Vector3(time.Hour * HoursToDegrees, 0, 0));
    }
}
