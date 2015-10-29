using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

[System.Serializable]
public class DataNode
{

}

[System.Serializable]
public class DoorNode
{
    public DoorDownController Doors;
}

[System.Serializable]
public class ExtinguisherNode
{
    public List<GameObject> FirewallsList = new List<GameObject>();
}

[System.Serializable]
public class TurretNode
{
    public TurretAI Turret;
}

[System.Serializable]
public class NodeFuncionality
{
    public DataNode DataNode;
    public DoorNode DoorNode;
    public ExtinguisherNode ExtinguisherNode;
    public TurretNode TurretNode;

}

public class InteractionNode : Node
{
    public NodeTextures Textures;
    public NodeFuncionality Funcionality;
    public Enums.InteractionNodes Type = Enums.InteractionNodes.Data;

    private bool _unlocked;

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
            tex = _unlocked ? Textures.DataUnlocked : Textures.DataLocked;
        }
        else if (Type.Equals(Enums.InteractionNodes.Door))
        {
            tex = _unlocked ? Textures.DoorUnlocked : Textures.DoorLocked;
        }
        else if (Type.Equals(Enums.InteractionNodes.Extinguisher))
        {
            tex = _unlocked ? Textures.ExtinguisherUnlocked : Textures.ExtinguisherLocked;
        }
        else
        {
            tex = _unlocked ? Textures.TurretUnlocked : Textures.TurretLocked;
        }

        if (!_mat.mainTexture.Equals(tex))
            _mat.mainTexture = tex;
    }

    public void UnlockNode()
    {
        if (_unlocked) return;
        _unlocked = true;


        if (Type.Equals(Enums.InteractionNodes.Data))
        {
            SoundManager.PlayNodeUnlockSound(AudioSource);
        }
        else if (Type.Equals(Enums.InteractionNodes.Door))
        {
            SoundManager.PlayNodeUnlockSound(AudioSource);
            Funcionality.DoorNode.Doors.SetLockType(Enums.DoorLockType.Unlocked);
        }
        else if (Type.Equals(Enums.InteractionNodes.Extinguisher))
        {
            SoundManager.PlayNodeUnlockSound(AudioSource);

            var focusPoint = Vector3.zero;

            foreach (var o in Funcionality.ExtinguisherNode.FirewallsList)
            {
                focusPoint.x += o.transform.position.x;
                focusPoint.z += o.transform.position.z;
                Destroy(o, 1);
            }

            focusPoint = focusPoint / Funcionality.ExtinguisherNode.FirewallsList.Count;

            RealCyberManager.ShowPointOfInterest(focusPoint);
        }
        else
        {
            SoundManager.PlayNodeUnlockSound(AudioSource);
            Funcionality.TurretNode.Turret.PlayerControlled = true;

        }
    }

    public bool IsUnlocked()
    {
        return _unlocked;
    }
}