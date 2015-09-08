using UnityEngine;
using System.Collections;

[System.Serializable]
public class NodeTextures
{
    public Texture DataLocked;
    public Texture DataUnlocked;

    public Texture DoorLocked;
    public Texture DoorUnlocked;

    public Texture ExtinguisherLocked;
    public Texture ExtinguisherUnlocked;

    public Texture TurretLocked;
    public Texture TurretUnlocked;
}

public class InteractionNode : Node
{
    public NodeTextures Textures;
    public Enums.InteractionNodes Type = Enums.InteractionNodes.Data;

    public bool Unlocked;

    private Material _mat;


    void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        Texture tex;

        if (Type.Equals(Enums.InteractionNodes.Data))
        {
            tex = Unlocked ? Textures.DataUnlocked : Textures.DataLocked;
        }
        else if (Type.Equals(Enums.InteractionNodes.Door))
        {
            tex = Unlocked ? Textures.DoorUnlocked : Textures.DoorLocked;
        }
        else if (Type.Equals(Enums.InteractionNodes.Extinguisher))
        {
            tex = Unlocked ? Textures.ExtinguisherUnlocked : Textures.ExtinguisherLocked;
        }
        else
        {
            tex = Unlocked ? Textures.TurretUnlocked : Textures.TurretLocked;
        }

        if (!_mat.mainTexture.Equals(tex))
            _mat.mainTexture = tex;
    }
}