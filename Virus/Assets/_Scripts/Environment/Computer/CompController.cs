﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompController : DelayedActivation
{
    public Enums.BuffType TypeOfBuff = Enums.BuffType.None;
    public float HackingDuration = 3;

    private List<Light> _lights = new List<Light>();
    private List<Component> _halos = new List<Component>();

    public bool IsHacked
    {
        get { return GameManager.IsComputerHacked(this); }
    }

    private bool _hackInProgress;

    public bool IsHackInProgress
    {
        get { return _hackInProgress; }
    }

    private ConnectionLine _line;

    void Awake()
    {
        _line = GetComponent<ConnectionLine>();

        foreach (var item in gameObject.GetComponentsInChildren<Light>())
        {
            _lights.Add(item);
        }

        foreach (var item in _lights)
        {
            _halos.Add(item.gameObject.GetComponent("Halo"));
        }

        for (int i = 0; i < _lights.Count; i++)
        {
            _lights[i].enabled = false;
            _halos[i].GetType().GetProperty("enabled").SetValue(_halos[i], false, null);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(_lights.Count + " " + _halos.Count);

            for (int i = 0; i < _lights.Count; i++)
            {
                _lights[i].enabled = true;
                _halos[i].GetType().GetProperty("enabled").SetValue(_halos[i], true, null);
            }
        }
    }

    void OnEnable()
    {
        SetActivationDuration(HackingDuration);

        OnActivationFinished += HackingFinished;
        OnDeactivationFinished += DehackingFinished;

        GameManager.AddComputer(this);
    }

    void OnDisable()
    {
        OnActivationFinished -= HackingFinished;
        OnDeactivationFinished -= DehackingFinished;

        ResetActivator();
        GameManager.RemoveComputer(this);
    }

    IEnumerator Writer()
    {
        while (true)
        {
            Debug.Log(GetActivationProgress());
            yield return new WaitForSeconds(.2f);
        }
    }

    public void StartHacking(PlayerController player)
    {
        StartHacking(player, HackingDuration);
    }

    public void StartHacking(PlayerController player, float duration)
    {
        _hackInProgress = true;

        StartActivation(duration);

        _line.SetDestination(player.transform);
        _line.AnimateLine(Enums.AnimType.FromOriginToDestination, duration);
    }

    public void StopHacking()
    {
        StopActivation();
    }

    void HackingFinished()
    {
        _hackInProgress = false;
        GameManager.AddHackedComputer(this);
    }

    public void StartDehacking()
    {
        StartDehacking(HackingDuration);
    }

    public void StartDehacking(float duration)
    {
        StartDeactivation(duration);
        _line.AnimateLine(Enums.AnimType.FromDestinationToOrigin, duration);
    }

    public void StopDehacking()
    {
        StopActivation();
    }

    void DehackingFinished()
    {
        GameManager.RemoveHackedComputer(this);
    }
}
