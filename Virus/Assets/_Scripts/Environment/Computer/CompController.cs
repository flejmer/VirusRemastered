using UnityEngine;
using System.Collections;

public class CompController : DelayedActivation
{
    public Enums.BuffType TypeOfBuff = Enums.BuffType.None;
    public float HackingDuration = 3;

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
