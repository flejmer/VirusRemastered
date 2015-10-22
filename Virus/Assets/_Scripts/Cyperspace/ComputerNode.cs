using UnityEngine;
using System.Collections;

public class ComputerNode : Node
{
    public CompController RealComputer;

    public bool Overload;
    public bool CanBeOverloaded;

    [SerializeField]
    private Texture _computerNormal;
    [SerializeField]
    private Texture _computerOverloaded;

    private Material _mat;

    void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (Overload)
	    {
            if(!_mat.mainTexture.Equals(_computerOverloaded))
                _mat.mainTexture = _computerOverloaded;
	    }
	    else
	    {
            if (!_mat.mainTexture.Equals(_computerNormal))
                _mat.mainTexture = _computerNormal;
        }
	}
}
