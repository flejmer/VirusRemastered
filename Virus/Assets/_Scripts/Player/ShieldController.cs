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
    }

    public void ActivateShield()
    {
        ShieldActivated = true;

        if (_enumeratorDone)
        {
            StartCoroutine(_shieldEnumerator);
        }
    }

    public void DeactivateShield()
    {
        ShieldActivated = false;

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
            if (ShieldActivated)
            {
                _currentStrength += Time.deltaTime / (AnimationTime / 2);

                if (_currentStrength >= 2)
                {
                    _enumeratorDone = true;
                    _col.enabled = true;
                }
            }
            else
            {
                _currentStrength -= Time.deltaTime / (AnimationTime / 2);

                if (_currentStrength <= 0)
                {
                    _enumeratorDone = true;
                    _col.enabled = false;
                }
            }

            _shieldMaterial.SetFloat("_Strength", _currentStrength);
            yield return new WaitForEndOfFrame();
        }

        _shieldEnumerator = ShieldAnimation();
    }
}
