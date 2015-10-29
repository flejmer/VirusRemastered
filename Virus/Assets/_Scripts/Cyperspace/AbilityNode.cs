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
        SoundManager.PlayNodeUnlockSound(AudioSource);

        if (NodeType == Enums.Abilities.Laser)
        {
            RealCyberManager.GetPlayer().UnlockLaser();
        }
        else if (NodeType == Enums.Abilities.Hologram)
        {
            RealCyberManager.GetPlayer().UnlockHologram();
        }
        else if (NodeType == Enums.Abilities.MindControl)
        {
            RealCyberManager.GetPlayer().UnlockMindControl();
        }
        else if (NodeType == Enums.Abilities.Shield)
        {
            RealCyberManager.GetPlayer().UnlockShield();
        }
        else
        {
            RealCyberManager.GetPlayer().UnlockSlowMotion();
        }

        if (!Unlocked)
        {
            Unlocked = true;
        }
    }
}