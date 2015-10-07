using UnityEngine;
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
    private bool _dehackInProgress;

    private Transform _dropHackZone;

    public bool IsHackInProgress
    {
        get { return _hackInProgress; }
    }

    public bool IsDehackInProgress
    {
        get { return _dehackInProgress; }
    }

    private ConnectionLine _line;

    void Awake()
    {
        _line = GetComponentInChildren<ConnectionLine>();
        _dropHackZone = transform.FindChild("DropHack");

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
        if (_hackInProgress || _dehackInProgress)
        {
            var light1 = (int) (GetActivationProgress()*10/2);
            var i = 0;

            if (_dehackInProgress && !(GetActivationProgress() <= 0))
            {
                light1++;
            }

            while (i < _lights.Count)
            {
                if (light1 > 0)
                {
                    _lights[i].enabled = true;
                    _halos[i].GetType().GetProperty("enabled").SetValue(_halos[i], true, null);
                }
                else
                {
                    _lights[i].enabled = false;
                    _halos[i].GetType().GetProperty("enabled").SetValue(_halos[i], false, null);
                }

                i++;
                light1--;
            }
        }

//        if (Input.GetKeyDown(KeyCode.P))
//        {
//            Debug.Log(_lights.Count + " " + _halos.Count);
//
//            for (int i = 0; i < _lights.Count; i++)
//            {
//                _lights[i].enabled = true;
//                _halos[i].GetType().GetProperty("enabled").SetValue(_halos[i], true, null);
//            }
//        }
    }

    public Vector3 GetHackPosition()
    {
        return _dropHackZone.position;
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
        _hackInProgress = false;
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
        if(GetActivationProgress() <= 0) return;

        _dehackInProgress = true;
        StartDeactivation(duration);
        _line.AnimateLine(Enums.AnimType.FromDestinationToOrigin, duration);
    }

    public void StopDehacking()
    {
        StopActivation();
        _dehackInProgress = false;
    }

    void DehackingFinished()
    {
        _dehackInProgress = false;
        GameManager.RemoveHackedComputer(this);
    }
}
