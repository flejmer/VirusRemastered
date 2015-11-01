using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    enum ScreenStates { Menu, Settings }

    private ScreenStates _state = ScreenStates.Menu;

    private WinMenuController _menu;
    private PauseSettingsController _settings;

    private Button[] _buttons;

    void Awake()
    {
        _menu = GetComponentInChildren<WinMenuController>();
        _settings = GetComponentInChildren<PauseSettingsController>();
        _buttons = GetComponentsInChildren<Button>();

        _settings.gameObject.SetActive(false);
    }

    public void ButtonsActivate()
    {
        foreach (var button in _buttons)
        {
            button.interactable = true;
        }
    }

    public void ButtonsDeactivate()
    {
        foreach (var button in _buttons)
        {
            button.interactable = false;
        }
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
