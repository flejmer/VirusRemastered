using UnityEngine;
using System.Collections;

public class CompInterArea : MonoBehaviour
{
    private CompController _cc;
    private PlayerController _player;

    void Start()
    {
        _cc = GetComponentInParent<CompController>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        _cc.PlayerInInterArea = true;
        _player.ComputersInInterRange.Add(_cc);
    }

    void OnTriggerExit(Collider other)
    {
        if (!_cc.IsHacked)
        {
            _cc.StopHacking();
        }

        _cc.PlayerInInterArea = false;
        _player.ComputersInInterRange.Remove(_cc);
    }
}
