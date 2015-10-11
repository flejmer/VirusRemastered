using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour
{
    private static GUIController Instance;

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
    }

    void Start()
    {
        _startScreen = GetComponentInChildren<StartScreen>();
        _pauseScreen = GetComponentInChildren<PauseScreen>();
        _gameUi = GetComponentInChildren<GameUI>();
        _popup = GetComponentInChildren<Popup>();

        _startScreen.gameObject.SetActive(true);
        _pauseScreen.gameObject.SetActive(false);
        _gameUi.gameObject.SetActive(false);
        _popup.gameObject.SetActive(false);


    }

    public static void StartGame()
    {
        Instance._startScreen.gameObject.SetActive(false);
        Instance._gameUi.gameObject.SetActive(true);
        GameManager.Instance.GameState = Enums.GameStates.GamePlay;
    }
}
