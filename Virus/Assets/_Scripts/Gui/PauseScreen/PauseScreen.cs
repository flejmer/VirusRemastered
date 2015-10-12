﻿using UnityEngine;
using System.Collections;

public class PauseScreen : MonoBehaviour
{
    enum ScreenStates { Menu, Settings }

    private ScreenStates _state = ScreenStates.Menu;

    private PauseMenuController _menu;
    private PauseSettingsController _settings;

    void Start()
    {
        _menu = GetComponentInChildren<PauseMenuController>();
        _settings = GetComponentInChildren<PauseSettingsController>();

        _settings.gameObject.SetActive(false);
    }

    public void GoToSettings()
    {
        if (!_state.Equals(ScreenStates.Menu)) return;

        _state = ScreenStates.Settings;

        _menu.gameObject.SetActive(false);
        _settings.gameObject.SetActive(true);
    }

    public void GoToMenu()
    {
        if (!_state.Equals(ScreenStates.Settings)) return;

        _state = ScreenStates.Menu;

        _menu.gameObject.SetActive(true);
        _settings.gameObject.SetActive(false);
    }
}
