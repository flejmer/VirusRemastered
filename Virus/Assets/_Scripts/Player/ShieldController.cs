using System;
using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour
{
    public float AnimationTime = 1;
    public bool ShieldActivated { get; private set; }

    private CapsuleCollider _col;
    private Material _shieldMaterial;
    private float _currentStrength;

    private IEnumerator _shieldEnumerator;
    private bool _enumeratorDone;
    private bool _shieldActivation;

    private bool _energyRemoverActivation;
    private bool _energyRemover;

    void Start()
    {
        _col = GetComponent<CapsuleCollider>();
        _shieldMaterial = GetComponent<MeshRenderer>().material;
        _currentStrength = _shieldMaterial.GetFloat("_Strength");

        _shieldEnumerator = ShieldAnimation();

        _col.enabled = false;
        StartCoroutine(_shieldEnumerator);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;

        if (ShieldActivated)
        {
            if (_energyRemover)
            {
                GameManager.GetPlayer().RemoveEnergy(10);
                _energyRemover = false;

                if (Math.Abs(GameManager.GetPlayer().GetEnergy()) < 0.5f)
                {
                    DeactivateShield();
                }
            }
            else
            {
                if (!_energyRemoverActivation)
                {
                    Invoke("EnergyRemoverActivator", 2);
                    _energyRemoverActivation = true;
                }

            }
        }
        else
        {
            if (_energyRemover)
                _energyRemover = false;
        }
    }

    void EnergyRemoverActivator()
    {
        _energyRemover = true;
        _energyRemoverActivation = false;
    }

    public void ActivateShield()
    {
        if (!(GameManager.GetPlayer().GetEnergy() > 0)) return;

        _shieldActivation = true;

        if (_enumeratorDone)
        {
            StartCoroutine(_shieldEnumerator);
        }
    }

    public void DeactivateShield()
    {
        _shieldActivation = false;

        if (_enumeratorDone)
        {
            StartCoroutine(_shieldEnumerator);
        }
    }

    IEnumerator ShieldAnimation()
    {
        _enumeratorDone = false;

        while (!_enumeratorDone)
        {
            if (_shieldActivation)
            {
                _currentStrength += Time.deltaTime / (AnimationTime / 2);

                if (_currentStrength >= 2)
                {
                    _enumeratorDone = true;
                    _col.enabled = true;
                    ShieldActivated = true;
                }
            }
            else
            {
                _currentStrength -= Time.deltaTime / (AnimationTime / 2);

                if (_currentStrength <= 0)
                {
                    _enumeratorDone = true;
                    _col.enabled = false;
                    ShieldActivated = false;
                }
            }

            _shieldMaterial.SetFloat("_Strength", _currentStrength);
            yield return new WaitForEndOfFrame();
        }

        _shieldEnumerator = ShieldAnimation();
    }
}
