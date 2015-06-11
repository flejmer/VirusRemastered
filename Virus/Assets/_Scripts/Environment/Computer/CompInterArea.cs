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
        if (other.CompareTag("Player"))
        {
            GameManager.AddComputerInPlayerInterRange(GameManager.GetPlayer(), _cc);
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
            GameManager.RemoveComputerInPlayerInterRange(GameManager.GetPlayer(), _cc);
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemySimpleAI>();
            GameManager.RemoveComputerInEnemyInterRange(enemy, _cc);
        }

    }
}
