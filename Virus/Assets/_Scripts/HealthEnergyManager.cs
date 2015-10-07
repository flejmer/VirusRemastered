using UnityEngine;
using System.Collections;

public class HealthEnergyManager : MonoBehaviour
{
    [SerializeField]
    private float _health = 100;
    [SerializeField]
    private float _energy = 100;

    public float MaxHealth = 100;
    public float MaxEnergy = 100;

    public float GetHealth()
    {
        return _health;
    }

    public float GetEnergy()
    {
        return _energy;
    }

    public void AddHp(float amount)
    {
        var hpAfterHeal = _health + amount;
        _health = hpAfterHeal > MaxHealth ? MaxHealth : hpAfterHeal;
    }

    public void RemoveHp(float amount)
    {
        var hpAfterDamage = _health - amount;
        _health = hpAfterDamage < 0 ? 0 : hpAfterDamage;
    }

    public void AddEnergy(float amount)
    {
        var eneAfterHeal = _energy + amount;
        _energy = eneAfterHeal > MaxEnergy ? MaxEnergy : eneAfterHeal;
    }

    public void RemoveEnergy(float amount)
    {
        var eneAfterDamage = _energy - amount;
        _energy = eneAfterDamage < 0 ? 0 : eneAfterDamage;
    }
}
