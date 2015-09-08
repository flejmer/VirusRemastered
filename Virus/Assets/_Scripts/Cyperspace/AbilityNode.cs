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
                _mat.mainTexture = _abilityUnlocked;
        }
        else
        {
            if (!_mat.mainTexture.Equals(_abilityLocked))
                _mat.mainTexture = _abilityLocked;
        }
    }
}