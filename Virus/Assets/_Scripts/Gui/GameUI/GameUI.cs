using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    private MStatusElement _mStatusElement;

    private TutElement[] _tutorialElements;

    private HealthBarActivator[] _healthBars;
    private EnergyBarActivator[] _energyBars;

    private SkillsPanelController _skillsPanelController;

    private float _lastHealth = 0;
    private float _lastEnergy = 0;

    void Start()
    {
        _mStatusElement = GetComponentInChildren<MStatusElement>();
        _healthBars = GetComponentsInChildren<HealthBarActivator>();
        _energyBars = GetComponentsInChildren<EnergyBarActivator>();
        _tutorialElements = GetComponentsInChildren<TutElement>();
        _skillsPanelController = GetComponentInChildren<SkillsPanelController>();

        foreach (var tutorialElement in _tutorialElements)
        {
            tutorialElement.gameObject.SetActive(false);
        }

        _skillsPanelController.UpdateAll();
    }

    void Update()
    {
        if (!GameManager.GetPlayer()) return;

        UpdateHealth();
        UpdateEnergy();


    }

    public void ActivateTutorial()
    {
        foreach (var tutorialElement in _tutorialElements)
        {
            tutorialElement.gameObject.SetActive(true);
        }
    }

    public void DeactivateTutorial()
    {
        foreach (var tutorialElement in _tutorialElements)
        {
            tutorialElement.gameObject.SetActive(false);
        }
    }

    public void UpdateMissionStatus(string text)
    {
        _mStatusElement.SetMStatusText(text);
    }

    void UpdateHealth()
    {
        if (!(Math.Abs(_lastHealth - GameManager.GetPlayer().GetHealth()) > 0)) return;

        _lastHealth = GameManager.GetPlayer().GetHealth();

        var hp = (int)_lastHealth / 10;

        for (var i = 0; i < _healthBars.Length; i++)
        {
            _healthBars[i].Activated = i < hp;
        }
    }

    void UpdateEnergy()
    {
        if (!(Math.Abs(_lastEnergy - GameManager.GetPlayer().GetEnergy()) > 0)) return;

        _lastEnergy = GameManager.GetPlayer().GetEnergy();

        var en = (int)_lastEnergy / 10;

        for (var i = 0; i < _energyBars.Length; i++)
        {
            _energyBars[i].Activated = i < en;
        }
    }
}
