using UnityEngine;
using System.Collections;

public class EnemySimpleAI : MonoBehaviour
{
    [SerializeField]
    private float _hpPoints = 100;
    [SerializeField]
    private float _maxHpPoints = 100;

    public float HealthPoints
    {
        get { return _hpPoints; }
    }

    protected NavMeshAgent Agent;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    public void AddHp(float count)
    {
        var hpAfterHeal = _hpPoints + count;
        _hpPoints = hpAfterHeal > _maxHpPoints ? _maxHpPoints : hpAfterHeal;
    }

    public void RemoveHp(float count)
    {
        var hpAfterDamage = _hpPoints - count;
        _hpPoints = hpAfterDamage < 0 ? 0 : hpAfterDamage;
    }

    void OnEnable()
    {
        GameManager.AddEnemy(this);
    }

    void OnDisable()
    {
        GameManager.RemoveEnemy(this);
    }
}
