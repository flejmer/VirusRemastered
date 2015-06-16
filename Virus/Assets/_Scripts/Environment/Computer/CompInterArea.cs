﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompInterArea : MonoBehaviour
{
    private CompController _cc;
    private List<MeshRenderer> _ccMesh;
    private Color _originalColor;

    void Start()
    {
        _cc = GetComponentInParent<CompController>();
        _ccMesh = new List<MeshRenderer>(_cc.GetComponentsInChildren<MeshRenderer>());
        _originalColor = _ccMesh[0].material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.AddComputerInPlayerInterRange(GameManager.GetPlayer(), _cc);

            foreach (var mrender in _ccMesh)
            {
                mrender.material.color = _originalColor + Color.red;
            }

        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemySimpleAI>();
            GameManager.AddComputerInEnemyInterRange(enemy, _cc);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (!_cc.IsHacked)
        {
            _cc.StopHacking();
            _cc.StartDehacking();
        }

        if (other.CompareTag("Player"))
        {
            GameManager.RemoveComputerInPlayerInterRange(GameManager.GetPlayer(), _cc);

            foreach (var mrender in _ccMesh)
            {
                mrender.material.color = _originalColor;
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<EnemySimpleAI>();
            GameManager.RemoveComputerInEnemyInterRange(enemy, _cc);
        }

    }
}
