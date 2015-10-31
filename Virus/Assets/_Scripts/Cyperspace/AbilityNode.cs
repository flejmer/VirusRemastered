using UnityEngine;
using System.Collections;

public class AbilityNode : Node
{
    public Enums.Abilities NodeType = Enums.Abilities.Laser;
    public bool Unlocked;

    [SerializeField]
    private Texture _abilityUnlocked;
    [SerializeField]
    private Texture _abilityLocked;

    private Material _mat;
    private bool _firstTime = true;

    void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (Unlocked)
        {
            if (!_mat.mainTexture.Equals(_abilityUnlocked))
            {
                _mat.mainTexture = _abilityUnlocked;
                UnlockAbility();
                _firstTime = false;
            }
        }
        else
        {
            if (!_mat.mainTexture.Equals(_abilityLocked))
                _mat.mainTexture = _abilityLocked;
        }
    }

    public void UnlockAbility()
    {
        if (_firstTime)
            SoundManager.PlayNodeUnlockSound(AudioSource);

        if (NodeType == Enums.Abilities.Laser)
        {
            RealCyberManager.GetPlayer().UnlockLaser();
            GUIController.ActivateLaserInfo();
        }
        else if (NodeType == Enums.Abilities.Hologram)
        {
            RealCyberManager.GetPlayer().UnlockHologram();
            GUIController.ActivateHologramInfo();
        }
        else if (NodeType == Enums.Abilities.MindControl)
        {
            RealCyberManager.GetPlayer().UnlockMindControl();
            GUIController.ActivateMindControlInfo();
        }
        else if (NodeType == Enums.Abilities.Shield)
        {
            RealCyberManager.GetPlayer().UnlockShield();
            GUIController.ActivateShieldInfo();
        }
        else
        {
            RealCyberManager.GetPlayer().UnlockSlowMotion();
            GUIController.ActivateSlowMotionInfo();
        }

        if (!Unlocked)
        {
            Unlocked = true;
        }
    }
}