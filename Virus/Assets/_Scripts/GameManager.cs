using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PlayerController _player;
    private readonly List<GameObject> _enemiesList = new List<GameObject>();
    private readonly List<CompController> _computersList = new List<CompController>();

    private readonly List<CompController> _hackedComputersList = new List<CompController>();

    void Awake()
    {
        Debug.Log("Awake gm");

        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("level loaded");
    }

    void Start()
    {
        Debug.Log("game manager start");
    }

    public static void SetPlayer(PlayerController player)
    {
        Instance._player = player;
    }

    public static void AddEnemy(GameObject enemy)
    {
        if (!Instance._enemiesList.Contains(enemy))
        {
            Instance._enemiesList.Add(enemy);
        }
    }

    public static void RemoveEnemy(GameObject enemy)
    {
        if (Instance._enemiesList.Contains(enemy))
        {
            Instance._enemiesList.Remove(enemy);
        }
    }

    public static void AddComputer(CompController comp)
    {
        if (!Instance._computersList.Contains(comp))
        {
            Instance._computersList.Add(comp);
        }
    }

    public static void RemoveComputer(CompController comp)
    {
        if (Instance._computersList.Contains(comp))
        {
            if (Instance._player.ComputersInInterRange.Contains(comp))
            {
                Instance._player.ComputersInInterRange.Remove(comp);
            }

            Instance._computersList.Remove(comp);
            RemoveHackedComputer(comp);
        }
    }

    public static void AddHackedComputer(CompController comp)
    {
        if (!Instance._hackedComputersList.Contains(comp))
        {
            Instance._hackedComputersList.Add(comp);
        }
    }

    public static void RemoveHackedComputer(CompController comp)
    {
        if (Instance._hackedComputersList.Contains(comp))
        {
            Instance._hackedComputersList.Remove(comp);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("computers = " + _computersList.Count);

            if (_player == null)
            {
                Debug.Log("player = null");
            }
            else
            {
                Debug.Log("player = " + _player.name);
            }

        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Comus in range = " + _player.ComputersInInterRange.Count);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Application.LoadLevel("TestingScene");
        }
    }
}
