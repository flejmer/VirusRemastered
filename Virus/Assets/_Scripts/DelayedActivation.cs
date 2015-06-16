using UnityEngine;
using System.Collections;

public class DelayedActivation : MonoBehaviour
{
    private float _defaultDuration = 3;

    private bool _activationInProgress;
    private bool _deactivationInProgress;

    private float _progressionFloat;

    private IEnumerator _activationEnumerator;
    private IEnumerator _deactivationEnumerator;

    protected delegate void ActivationAction();
    protected event ActivationAction OnActivationStarted;
    protected event ActivationAction OnActivationUpdate;
    protected event ActivationAction OnActivationFinished;
    protected event ActivationAction OnDeactivationStarted;
    protected event ActivationAction OnDeactivationUpdate;
    protected event ActivationAction OnDeactivationFinished;

    protected void SetActivationDuration(float duration)
    {
        if (duration >= 0)
            _defaultDuration = duration;
    }

    protected float GetActivationProgress()
    {
        return _progressionFloat;
    }

    protected void ResetActivator()
    {
        StopActivation();
        StopDeactivation();
        _progressionFloat = 0;

        if (OnActivationStarted != null)
            OnActivationStarted -= ActivationStarted;

        if(OnActivationFinished != null)
            OnActivationFinished -= ActivationFinished;

        if (OnActivationStarted != null)
            OnActivationStarted -= DeactivationStarted;

        if(OnDeactivationFinished != null)
            OnDeactivationFinished -= DeactivationFinished;
    }

    protected void ActivationStarted()
    {
        _activationInProgress = true;
    }

    protected void ActivationFinished()
    {
        _progressionFloat = 1;
    }

    protected void DeactivationStarted()
    {
        _deactivationInProgress = true;
    }

    protected void DeactivationFinished()
    {
        _progressionFloat = 0;
    }

    protected void StartActivation()
    {
        StartActivation(_defaultDuration);
    }

    protected void StartActivation(float duration)
    {
        OnActivationStarted += ActivationStarted;
        OnActivationFinished += ActivationFinished;

        if (_activationInProgress) return;

        if (_deactivationInProgress)
        {
            StopDeactivation();
        }

        _activationEnumerator = Activation(duration);
        StartCoroutine(_activationEnumerator);
    }

    protected void StopActivation()
    {
        if (!_activationInProgress) return;

        StopCoroutine(_activationEnumerator);
        _activationInProgress = false;
    }

    protected void StartDeactivation()
    {
        StartDeactivation(_defaultDuration);
    }

    protected void StartDeactivation(float duration)
    {
        OnDeactivationStarted += DeactivationStarted;
        OnDeactivationFinished += DeactivationFinished;

        if (_deactivationInProgress) return;

        if (_activationInProgress)
        {
            StopActivation();
        }

        _deactivationEnumerator = Deactivation(duration);
        StartCoroutine(_deactivationEnumerator);
    }

    protected void StopDeactivation()
    {
        if (!_deactivationInProgress) return;

        StopCoroutine(_deactivationEnumerator);
        _deactivationInProgress = false;
    }

    private IEnumerator Activation(float duration)
    {
        if (OnActivationStarted != null)
            OnActivationStarted();

        while (_progressionFloat < 1)
        {
            if (OnActivationUpdate != null)
                OnActivationUpdate();

            _progressionFloat += Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        if (OnActivationFinished != null)
            OnActivationFinished();
    }

    private IEnumerator Deactivation(float duration)
    {
        if (OnDeactivationStarted != null)
            OnDeactivationStarted();

        while (_progressionFloat > 0)
        {
            if (OnDeactivationUpdate != null)
                OnDeactivationUpdate();

            _progressionFloat -= Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        if (OnDeactivationFinished != null)
            OnDeactivationFinished();
    }
}
