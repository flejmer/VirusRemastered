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
        if (other.CompareTag("Player"))
        {
            GameManager.AddComputerInPlayerInterRange(_player, _cc);
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemySimpleAI>();
            GameManager.AddComputerInEnemyInterRange(enemy, _cc);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (!_cc.IsHacked)
        {
            _cc.StopHacking();
        }

        if (other.CompareTag("Player"))
        {
            GameManager.RemoveComputerInPlayerInterRange(_player, _cc);
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemySimpleAI>();
            GameManager.RemoveComputerInEnemyInterRange(enemy, _cc);
        }

    }
}
