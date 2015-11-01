using UnityEngine;
using System.Collections;

public class MinimapPlayer : MonoBehaviour
{
    public Transform Player;
    private Vector3 _offset;

    // Use this for initialization
    void Start()
    {
        _offset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player)
        {
            transform.position = Player.position + _offset;
        }
    }
}
