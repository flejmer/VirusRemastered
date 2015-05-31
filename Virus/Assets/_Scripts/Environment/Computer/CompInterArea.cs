using UnityEngine;
using System.Collections;

public class CompInterArea : MonoBehaviour
{
    private CompController _cc;

    void Start()
    {
        _cc = GetComponentInParent<CompController>();
    }

    void OnTriggerEnter(Collider other)
    {
    }

    void OnTriggerExit(Collider other)
    {
        if (!_cc.IsHacked)
        {
            _cc.StopHacking();
        }

    }
}
