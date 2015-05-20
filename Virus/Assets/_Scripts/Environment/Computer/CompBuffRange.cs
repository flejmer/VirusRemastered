using UnityEngine;
using System.Collections;

public class CompBuffRange : MonoBehaviour
{
    private CompController _cc;

    void Start()
    {
        _cc = GetComponentInParent<CompController>();
    }

    void OnTriggerEnter(Collider other)
    {
        _cc.PlayerInBuffRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        _cc.PlayerInBuffRange = false;
    }
}
