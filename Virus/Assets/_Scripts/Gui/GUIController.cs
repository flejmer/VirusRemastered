using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GUIController : MonoBehaviour
{
    public static GUIController Instance { get; private set; }

    private StartScreen _startScreen;
    private PauseScreen _pauseScreen;
    private DeadScreen _deadScreen;
    private GameUI _gameUi;
    private Popup _popup;

    private EventSystem _eSys;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _startScreen = GetComponentInChildren<StartScreen>();
        _pauseScreen = GetComponentInChildren<PauseScreen>();
        _gameUi = GetComponentInChildren<GameUI>();
        _popup = GetComponentInChildren<Popup>();
        _deadScreen = GetComponentInChildren<DeadScreen>();
        _eSys = EventSystem.current;

        _startScreen.gameObject.SetActive(false);
        _pauseScreen.gameObject.SetActive(false);
        _gameUi.gameObject.SetActive(false);
        _popup.gameObject.SetActive(false);
        _deadScreen.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _popup.ActivateHackingInfo();
        }
    }

    public static bool IsPopupActivated()
    {
        return Instance._popup.Active;
    }

    public static void MenuScreen()
    {
        Instance._startScreen.gameObject.SetActive(true);
        Instance._gameUi.gameObject.SetActive(false);
        GameManager.Instance.GameState = Enums.GameStates.MainMenu;

        var canvas = Instance._startScreen.gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public static void Game()
    {
        Instance._startScreen.gameObject.SetActive(false);
        Instance._gameUi.gameObject.SetActive(true);
        GameManager.Instance.GameState = Enums.GameStates.GamePlay;
    }

    public static void DeadScreenActivate()
    {
        if (!GameManager.Instance.GameState.Equals(Enums.GameStates.GamePlay) ||
    GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause))
            return;

        Instance._deadScreen.gameObject.SetActive(true);
        Instance._deadScreen.ButtonsActivate();
        GameManager.Instance.InGameState = Enums.InGameStates.Pause;
    }

    public static void DeadScreenDeactivate()
    {
        if (!GameManager.Instance.GameState.Equals(Enums.GameStates.GamePlay) ||
    !GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause))
            return;

        Instance.Invoke("DeadScreenCloseDelay", 0.1f);
        Instance._deadScreen.ButtonsDeactivate();
    }

    void DeadScreenCloseDelay()
    {
        Instance._deadScreen.gameObject.SetActive(false);
        Instance._deadScreen.GoToMenu();
        GameManager.Instance.InGameState = Enums.InGameStates.Normal;

        if (!Instance._popup.Active)
        {
            Time.timeScale = 1;
        }
    }

    public static void PauseScreenActivate()
    {
        if (!GameManager.Instance.GameState.Equals(Enums.GameStates.GamePlay) ||
            GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause) || RealCyberManager.GetPlayer().PlayerState.Equals(Enums.PlayerStates.Dead))
            return;

        if (GameManager.Instance.InGameState.Equals(Enums.InGameStates.InitTutorial))
        {
            Instance._gameUi.DeactivateTutorial();
        }

        Instance._pauseScreen.gameObject.SetActive(true);
        Instance._pauseScreen.ButtonsActivate();

        GameManager.Instance.InGameState = Enums.InGameStates.Pause;
    }

    public static void PauseScreenDeactivate()
    {
        if (!GameManager.Instance.GameState.Equals(Enums.GameStates.GamePlay) ||
            !GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause))
            return;

        Instance._eSys.SetSelectedGameObject(Instance._eSys.gameObject);

        Instance.DelayPauseClose();
        Instance._pauseScreen.ButtonsDeactivate();
    }

    void DelayPauseClose()
    {
        StartCoroutine(PauseScreenCloseDelay(0.1f));
    }

    static IEnumerator PauseScreenCloseDelay(float time)
    {
        float counter = 0;

        while (counter < time)
        {
            yield return new WaitForEndOfFrame();
            counter += Time.unscaledDeltaTime;
        }

        Instance._pauseScreen.gameObject.SetActive(false);
        Instance._pauseScreen.GoToMenu();

        GameManager.Instance.InGameState = Enums.InGameStates.Normal;

        if (!Instance._popup.Active)
        {
            Time.timeScale = 1;
        }
    }
    public static bool IsInstanceNull()
    {
        return Instance == null;
    }

    public static void ToMenu()
    {
        Instance._popup.DeactivatePopup();
        GameManager.Instance.AppQuit = true;
        Application.LoadLevel("Menu");
    }

    public static void Restart()
    {
        Instance._popup.DeactivatePopup();
        GameManager.Instance.AppQuit = true;
        Application.LoadLevel("Game01");
    }
}
