using UnityEngine;
using System.Collections;

public class CompController : MonoBehaviour
{
    public Enums.BuffType TypeOfBuff = Enums.BuffType.None;
    public float HackingDuration = 3;

    public bool PlayerInInterArea { get; set; }
    public bool PlayerInBuffRange { get; set; }

    public bool IsHacked { get; set; }

    private bool _hackInProgress;
    private bool _dehackInProgress;
    private float _hackProgress;
    private IEnumerator _hackEnumerator;
    private IEnumerator _dehackEnumerator;

    void Start()
    {
        StartCoroutine(Writer());
    }

    IEnumerator Writer()
    {
        while (true)
        {
            Debug.Log(_hackProgress);
            yield return new WaitForSeconds(.2f);
        }
    }

    void Update()
    {
    }

    public void StartHacking()
    {
        if (_hackInProgress) return;

        if (_dehackInProgress)
        {
            StopDehacking();
        }

        _hackEnumerator = Hacking();
        StartCoroutine(_hackEnumerator);
    }

    public void StopHacking()
    {
        if (_hackInProgress)
        {
            StopCoroutine(_hackEnumerator);
            _hackInProgress = false;
        }

        StartDehacking();
    }

    public void StartDehacking()
    {
        if (_dehackInProgress) return;

        _dehackEnumerator = Dehacking();
        StartCoroutine(_dehackEnumerator);
    }

    public void StopDehacking()
    {
        if (!_dehackInProgress) return;

        StopCoroutine(_dehackEnumerator);
        _dehackInProgress = false;
    }

    IEnumerator Hacking()
    {
        _hackInProgress = true;

        while (_hackProgress < 1)
        {
            _hackProgress += Time.deltaTime / HackingDuration;
            yield return new WaitForEndOfFrame();
        }

        _hackInProgress = false;
    }

    IEnumerator Dehacking()
    {
        StopCoroutine(_dehackEnumerator);

        _dehackEnumerator = Dehacking(HackingDuration);
        StartCoroutine(_dehackEnumerator);

        yield return null;
    }

    IEnumerator Dehacking(float duration)
    {
        _dehackInProgress = true;

        while (_hackProgress > 0)
        {
            _hackProgress -= Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        _dehackInProgress = false;
    }
}
