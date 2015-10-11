using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour
{
    enum ScreenStates { Menu, Settings }

    private MenuController _menu;
    private SettingsController _settings;

    private ScreenStates _state = ScreenStates.Menu;

    void Awake()
    {
        _menu = GetComponentInChildren<MenuController>();
        _settings = GetComponentInChildren<SettingsController>();
    }

    public void GoToSettings()
    {
        if(_state != ScreenStates.Menu) return;

        _menu.Anim.SetTrigger("FadeOut");
        StartCoroutine(ActivateSettingsWin());
    }

    public void GoToMainMenu()
    {
        if(_state != ScreenStates.Settings) return;

        _settings.Anim.SetTrigger("FadeOut");
        StartCoroutine(ActivateMainMenuWin());
    }

    IEnumerator ActivateSettingsWin()
    {
        yield return new WaitForSeconds(.01f);

        while (!(_menu.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && !_menu.Anim.IsInTransition(0)))
        {
            yield return null;
        }

        _settings.Anim.SetTrigger("FadeIn");
        _state = ScreenStates.Settings;
    }

    IEnumerator ActivateMainMenuWin()
    {
        yield return new WaitForSeconds(.01f);

        while (!(_settings.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f && !_settings.Anim.IsInTransition(0)))
        {
            yield return null;
        }

        _menu.Anim.SetTrigger("FadeIn");
        _state = ScreenStates.Menu;
    }

}
