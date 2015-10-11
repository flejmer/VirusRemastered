using UnityEngine;
using System.Collections;

public class SettingsController : MonoBehaviour
{
    public Animator Anim { get; private set; }

    // Use this for initialization
    void Start()
    {
        Anim = GetComponent<Animator>();
    }
}
