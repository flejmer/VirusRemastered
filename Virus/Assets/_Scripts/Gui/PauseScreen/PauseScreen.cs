using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    enum ScreenStates { Menu, Settings }

    private ScreenStates _state = ScreenStates.Menu;

    private PauseMenuController _menu;
    private PauseSettingsController _settings;

    private Button[] _buttons;

    void Awake()
    {
        _menu = GetComponentInChildren<PauseMenuController>();
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

        ButtonsDeactivate();
        StartCoroutine(SettingsDelay(.1f));
    }

    public void GoToMenu()
    {
        if (!_state.Equals(ScreenStates.Settings)) return;

        ButtonsDeactivate();
        StartCoroutine(MenuDelay(.1f));
    }

    IEnumerator SettingsDelay(float time)
    {
        float counter = 0;

        while (counter < time)
        {
            yield return new WaitForEndOfFrame();
            counter += Time.unscaledDeltaTime;
        }

        _state = ScreenStates.Settings;

        _menu.gameObject.SetActive(false);
        _settings.gameObject.SetActive(true);
        ButtonsActivate();
    }

    IEnumerator MenuDelay(float time)
    {
        float counter = 0;

        while (counter < time)
        {
            yield return new WaitForEndOfFrame();
            counter += Time.unscaledDeltaTime;
        }

        _state = ScreenStates.Menu;

        _menu.gameObject.SetActive(true);
        _settings.gameObject.SetActive(false);
        ButtonsActivate();
    }
}
