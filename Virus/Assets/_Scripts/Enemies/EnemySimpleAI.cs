using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemySimpleAI : MonoBehaviour
{
    [SerializeField]
    private float _hpPoints = 100;
    [SerializeField]
    private float _maxHpPoints = 100;

    public float HealthPoints{ get { return _hpPoints; }}

    protected NavMeshAgent Agent;

    private List<SkinnedMeshRenderer> _Mesh;
    private Color _originalColor;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        _Mesh = new List<SkinnedMeshRenderer>(GetComponentsInChildren<SkinnedMeshRenderer>());
        _originalColor = _Mesh[0].material.color;
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

    public void Highlight()
    {
        CancelInvoke("Dehighlight");

        foreach (var mrender in _Mesh)
        {
            mrender.material.color = _originalColor + Color.red / 2;
        }

        Invoke("Dehighlight", 0.1f);
    }

    void Dehighlight()
    {
        foreach (var mrender in _Mesh)
        {
            mrender.material.color = _originalColor;
        }
    }

    void OnEnable()
    {
        GameManager.AddEnemy(this);
    }

    void OnDisable()
    {
        GameManager.RemoveEnemy(this);
    }

    public abstract void TakeOver();
}
