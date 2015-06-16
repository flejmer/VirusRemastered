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

    void HackingFinished()
    {
        GameManager.AddHackedComputer(this);
        // keep on drawing line
    }

    void DehackingFinished()
    {
        GameManager.RemoveHackedComputer(this);
        // if player in buff area > quick line break
    }

    public void StartHacking()
    {
        StartActivation();
        // start drawing line
    }

    public void StartHacking(float duration)
    {
        StartActivation(duration);
        // start drawing line
    }

    public void StopHacking()
    {
        StopActivation();
    }

    public void StartDehacking()
    {
        StartDeactivation();
        // if player in buff area > slowly break line
    }

    public void StartDehacking(float duration)
    {
        StartDeactivation(duration);
        // if player in buff area > slowly break line
    }

    public void StopDehacking()
    {
        StopActivation();
    }
}
