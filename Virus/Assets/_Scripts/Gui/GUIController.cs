using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour
{
    public static GUIController Instance { get; private set; }

    private StartScreen _startScreen;
    private PauseScreen _pauseScreen;
    private GameUI _gameUi;
    private Popup _popup;

    void Awake()
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

        _startScreen.gameObject.SetActive(false);
        _pauseScreen.gameObject.SetActive(false);
        _gameUi.gameObject.SetActive(false);
        _popup.gameObject.SetActive(false);
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

    public static void PauseScreenActivate()
    {
        if (!GameManager.Instance.GameState.Equals(Enums.GameStates.GamePlay) ||
            GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause)) return;

        if (GameManager.Instance.InGameState.Equals(Enums.InGameStates.InitTutorial))
        {
            Instance._gameUi.DeactivateTutorial();
        }

        Instance._pauseScreen.gameObject.SetActive(true);
        GameManager.Instance.InGameState = Enums.InGameStates.Pause;
    }

    public static void PauseScreenDeactivate()
    {
        if (!GameManager.Instance.GameState.Equals(Enums.GameStates.GamePlay) ||
            !GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause)) return;

        Instance._pauseScreen.gameObject.SetActive(false);
        GameManager.Instance.InGameState = Enums.InGameStates.Normal;
    }

    public static bool IsInstanceNull()
    {
        return Instance == null;
    }
}
