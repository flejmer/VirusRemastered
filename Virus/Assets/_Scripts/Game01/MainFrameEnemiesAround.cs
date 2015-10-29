using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainFrameEnemiesAround : MonoBehaviour
{
    private CompController _comp;
    private List<GameObject> _enemiesAround = new List<GameObject>();

    void OnEnable()
    {
        _comp = GetComponent<CompController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(_enemiesAround.Count);
        }
    }

    public void AddEnemy(GameObject o)
    {
        if (_enemiesAround.Contains(o)) return;

        _enemiesAround.Add(o);

        if (_enemiesAround.Count >= 0)
        {
            _comp.EnemiesAround = true;
        }
    }
    public void RemoveEnemy(GameObject o)
    {
        if (!_enemiesAround.Contains(o)) return;

        _enemiesAround.Remove(o);

        if (_enemiesAround.Count == 0)
        {
            _comp.EnemiesAround = false;
            Debug.Log("mute music here");
        }
    }
}
