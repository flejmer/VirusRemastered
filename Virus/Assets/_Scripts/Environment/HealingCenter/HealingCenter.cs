using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealingCenter : MonoBehaviour
{
    public float HealthPerTick = 20;

    private readonly List<EnemySimpleAI> _enemiesList = new List<EnemySimpleAI>();
    private bool _healingWave = true;

    public bool Heal = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("EnemyGuard") || other.tag.Equals("EnemyTech"))
        {
            EnemySimpleAI enemyAI = other.gameObject.GetComponent<EnemySimpleAI>();

            if (!_enemiesList.Contains(enemyAI))
            {
                _enemiesList.Add(enemyAI);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("EnemyGuard") || other.tag.Equals("EnemyTech"))
        {
            EnemySimpleAI enemyAI = other.gameObject.GetComponent<EnemySimpleAI>();

            if (_enemiesList.Contains(enemyAI))
            {
                _enemiesList.Remove(enemyAI);
            }
        }
    }

    void Update()
    {
        if (_enemiesList.Count <= 0) return;

        if (_healingWave)
        {
            foreach (EnemySimpleAI enemy in _enemiesList)
            {
                if(Heal)
                    enemy.AddHp(HealthPerTick);
                else
                    enemy.RemoveHp(HealthPerTick);
            }
            _healingWave = false;
            Invoke("ActivateHeal", 1);
        }
    }

    void ActivateHeal()
    {
        _healingWave = true;
    }

    void OnEnable()
    {
        GameManager.AddHealingCenter(this);
    }

    void OnDisable()
    {
        GameManager.RemoveHealingCenter(this);
    }
}
