using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;


public class GameManager : MonoBehaviour
{
    public Enums.GameStates GameState;

    public Enums.InGameStates InGameState;

    public bool TutorialActive { get; private set; }

    public static GameManager Instance { get; private set; }

    private PlayerController _player;

    private readonly List<EnemySimpleAI> _enemiesList = new List<EnemySimpleAI>();

    private readonly List<CompController> _computersList = new List<CompController>();

    private readonly List<HealingCenter> _healingCentersList = new List<HealingCenter>();

    private readonly Dictionary<PlayerController, List<CompController>> _computersInPlayerInterRange = new Dictionary<PlayerController, List<CompController>>();

    private readonly Dictionary<PlayerController, List<CompController>> _computersInPlayerBuffArea = new Dictionary<PlayerController, List<CompController>>();

    private readonly Dictionary<EnemySimpleAI, List<CompController>> _computersInEnemyInterRange = new Dictionary<EnemySimpleAI, List<CompController>>();

    private readonly List<CompController> _hackedComputersList = new List<CompController>();

    private Stack<CompController> _hackedComputersStack = new Stack<CompController>();

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
        OnLevelWasLoaded(Application.loadedLevel);
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            GUIController.MenuScreen();
            GameState = Enums.GameStates.MainMenu;
        }
        else
        {
            GUIController.Game();
            GameState = Enums.GameStates.GamePlay;
            InGameState = Enums.InGameStates.Normal;
        }
    }

    void Update()
    {
        if(!GameState.Equals(Enums.GameStates.GamePlay)) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!InGameState.Equals(Enums.InGameStates.Pause))
            {
                GUIController.PauseScreenActivate();
                Time.timeScale = 0;
            }
            else
            {
                GUIController.PauseScreenDeactivate();

                if(GUIController.IsPopupActivated()) return;

                Time.timeScale = 1;
            }
        }
    }

    public static bool IsInstanceNull()
    {
        return Instance == null;
    }

    public static void SetPlayer(PlayerController player)
    {
        if (player != null)
        {
            if (Instance._player == null)
            {
                Instance._player = player;
                Instance._computersInPlayerBuffArea.Add(player, new List<CompController>());
                Instance._computersInPlayerInterRange.Add(player, new List<CompController>());
            }
            else
            {
                List<CompController> compsInInterRangeForPlayer;
                Instance._computersInPlayerInterRange.TryGetValue(Instance._player, out compsInInterRangeForPlayer);

                List<CompController> compsInBuffAreaForPlayer;
                Instance._computersInPlayerBuffArea.TryGetValue(Instance._player, out compsInBuffAreaForPlayer);

                RemoveAllComputersInPlayerInterRange(Instance._player);
                RemoveAllComputersInPlayerBuffArea(Instance._player);

                Instance._player = player;


                if (compsInInterRangeForPlayer != null)
                {
                    foreach (var compController in compsInInterRangeForPlayer)
                    {
                        AddComputerInPlayerInterRange(player, compController);
                    }
                }

                if (compsInBuffAreaForPlayer != null)
                {
                    foreach (var compController in compsInBuffAreaForPlayer)
                    {
                        AddComputerInPlayerBuffArea(player, compController);
                    }
                }
            }
        }
        else
        {
            if (Instance._player == null) return;

            RemoveAllComputersInPlayerInterRange(Instance._player);
            RemoveAllComputersInPlayerBuffArea(Instance._player);

            Instance._player = null;
        }
    }

    public static PlayerController GetPlayer()
    {
        return Instance._player;
    }

    public static void DamagePlayerFromDirection(GameObject hitObj, float amount, Vector3 point, Vector3 moveDir, LayerMask layerMask, GameObject whoFired)
    {
        GetPlayer().RemoveHealth(20);
    }

    public static void AddEnemy(EnemySimpleAI enemy)
    {
        if (!Instance._enemiesList.Contains(enemy))
        {
            Instance._enemiesList.Add(enemy);
        }
    }

    public static void RemoveEnemy(EnemySimpleAI enemy)
    {
        if (Instance._enemiesList.Contains(enemy))
        {
            Instance._enemiesList.Remove(enemy);

            RemoveAllComputersInEnemyInterRange(enemy);
        }
    }

    public static void DamageEnemy(GameObject enemyGo, float amount)
    {
        DamageEnemyFromDirection(enemyGo, amount, Vector3.forward, Vector3.forward, new LayerMask(), null);
    }

    public static void DamageEnemyFromDirection(GameObject enemyHit, float amount, Vector3 point, Vector3 dir, LayerMask layer, GameObject whoFired)
    {
        foreach (var enemy in Instance._enemiesList)
        {
            if (!enemy.gameObject.Equals(enemyHit)) continue;
            if (enemy.gameObject.Equals(whoFired)) return;


            if (whoFired.CompareTag("EnemyGuard"))
            {
                var guardWhoFired = whoFired.GetComponent<EnemySimpleAI>();

                if (((enemy.PlayerControlled && !guardWhoFired.PlayerControlled) && (!enemy.PlayerControlled && guardWhoFired.PlayerControlled)))
                {
                    return;
                }


            }

            enemy.RemoveHp(amount);
            enemy.HitPoint(point, dir, 1000, layer);
            enemy.GotHitBy(whoFired);

            return;
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
        if (!Instance._computersList.Contains(comp)) return;

        Instance._computersList.Remove(comp);

        RemoveHackedComputer(comp);
        RemoveComputerInPlayerBuffArea(comp);
        RemoveComputerInPlayerInterRange(comp);
        RemoveComputerInEnemyInterRange(comp);
    }

    public static void AddHealingCenter(HealingCenter center)
    {
        if (!Instance._healingCentersList.Contains(center))
        {
            Instance._healingCentersList.Add(center);
        }
    }

    public static void RemoveHealingCenter(HealingCenter center)
    {
        if (!Instance._healingCentersList.Contains(center)) return;

        Instance._healingCentersList.Remove(center);
    }

    public static HealingCenter GetClosestHealingCenter(GameObject obj)
    {
        HealingCenter hc = Instance._healingCentersList[0];

        foreach (var center in Instance._healingCentersList)
        {
            var dist1 = (hc.transform.position - obj.transform.position).sqrMagnitude;
            var dist2 = (center.transform.position - obj.transform.position).sqrMagnitude;

            if (dist1 > dist2)
            {
                hc = center;
            }
        }

        return hc;
    }

    public static void AddComputerInPlayerInterRange(PlayerController player, CompController comp)
    {
        List<CompController> compsInInterRangeForPlayer;
        Instance._computersInPlayerInterRange.TryGetValue(player, out compsInInterRangeForPlayer);

        if (compsInInterRangeForPlayer == null)
        {
            var startList = new List<CompController> { comp };
            Instance._computersInPlayerInterRange.Add(player, startList);
        }
        else if (!compsInInterRangeForPlayer.Contains(comp))
        {
            compsInInterRangeForPlayer.Add(comp);
        }
    }

    public static void RemoveComputerInPlayerInterRange(CompController comp)
    {
        RemoveComputerInPlayerInterRange(null, comp);
    }

    public static void RemoveComputerInPlayerInterRange(PlayerController player, CompController comp)
    {
        if (player == null)
        {
            foreach (var keyPair in Instance._computersInPlayerInterRange)
            {
                if (keyPair.Value == null) continue;

                if (keyPair.Value.Contains(comp))
                {
                    keyPair.Value.Remove(comp);
                }
            }
        }
        else
        {
            List<CompController> compsInInterRangeForPlayer;
            Instance._computersInPlayerInterRange.TryGetValue(player, out compsInInterRangeForPlayer);

            if (compsInInterRangeForPlayer != null && compsInInterRangeForPlayer.Contains(comp))
            {
                compsInInterRangeForPlayer.Remove(comp);
            }
        }
    }

    static void RemoveAllComputersInPlayerInterRange(PlayerController player)
    {
        if (player == null) return;

        List<CompController> compsInInterRangeForPlayer;
        Instance._computersInPlayerInterRange.TryGetValue(player, out compsInInterRangeForPlayer);

        if (compsInInterRangeForPlayer != null)
        {
            compsInInterRangeForPlayer.Clear();
        }

        Instance._computersInPlayerInterRange.Remove(player);
    }

    public static List<CompController> GetComputersInPlayerInterRange(PlayerController player)
    {
        List<CompController> compsInInterRangeForPlayer;
        Instance._computersInPlayerInterRange.TryGetValue(player, out compsInInterRangeForPlayer);

        return compsInInterRangeForPlayer != null ? new List<CompController>(compsInInterRangeForPlayer) : null;
    }

    public static void AddComputerInPlayerBuffArea(PlayerController player, CompController comp)
    {
        List<CompController> compsInBuffAreaForPlayer;
        Instance._computersInPlayerBuffArea.TryGetValue(player, out compsInBuffAreaForPlayer);

        if (compsInBuffAreaForPlayer == null)
        {
            var startList = new List<CompController> { comp };
            Instance._computersInPlayerBuffArea.Add(player, startList);
        }
        else if (!compsInBuffAreaForPlayer.Contains(comp))
        {
            compsInBuffAreaForPlayer.Add(comp);
        }
    }

    public static void RemoveComputerInPlayerBuffArea(CompController comp)
    {
        RemoveComputerInPlayerBuffArea(null, comp);
    }

    public static bool IsPlayerInComputerBuffArea(CompController comp)
    {
        List<CompController> compsInBuffAreaForPlayer;
        Instance._computersInPlayerBuffArea.TryGetValue(GetPlayer(), out compsInBuffAreaForPlayer);

        return compsInBuffAreaForPlayer != null && compsInBuffAreaForPlayer.Contains(comp);
    }

    public static void RemoveComputerInPlayerBuffArea(PlayerController player, CompController comp)
    {
        if (player == null)
        {
            foreach (var keyPair in Instance._computersInPlayerBuffArea)
            {
                if (keyPair.Value == null) continue;

                if (keyPair.Value.Contains(comp))
                {
                    keyPair.Value.Remove(comp);
                }
            }
        }
        else
        {
            List<CompController> compsInBuffAreaForPlayer;
            Instance._computersInPlayerBuffArea.TryGetValue(player, out compsInBuffAreaForPlayer);

            if (compsInBuffAreaForPlayer != null && compsInBuffAreaForPlayer.Contains(comp))
            {
                compsInBuffAreaForPlayer.Remove(comp);
            }
        }
    }

    static void RemoveAllComputersInPlayerBuffArea(PlayerController player)
    {
        if (player == null) return;

        List<CompController> compsInBuffAreaForPlayer;
        Instance._computersInPlayerBuffArea.TryGetValue(player, out compsInBuffAreaForPlayer);

        if (compsInBuffAreaForPlayer != null)
        {
            compsInBuffAreaForPlayer.Clear();
        }

        Instance._computersInPlayerInterRange.Remove(player);
    }

    private static List<CompController> GetComputersInPlayerBuffArea(PlayerController player)
    {
        List<CompController> compsInBuffAreaForPlayer;
        Instance._computersInPlayerBuffArea.TryGetValue(player, out compsInBuffAreaForPlayer);

        return compsInBuffAreaForPlayer != null ? new List<CompController>(compsInBuffAreaForPlayer) : null;
    }

    public static void AddComputerInEnemyInterRange(EnemySimpleAI enemy, CompController comp)
    {
        List<CompController> compsInInterRangeForEnemy;
        Instance._computersInEnemyInterRange.TryGetValue(enemy, out compsInInterRangeForEnemy);

        if (compsInInterRangeForEnemy == null)
        {
            var startList = new List<CompController> { comp };
            Instance._computersInEnemyInterRange.Add(enemy, startList);
        }
        else if (!compsInInterRangeForEnemy.Contains(comp))
        {
            compsInInterRangeForEnemy.Add(comp);
        }
    }

    public static void RemoveComputerInEnemyInterRange(CompController comp)
    {
        RemoveComputerInEnemyInterRange(null, comp);
    }

    public static void RemoveComputerInEnemyInterRange(EnemySimpleAI enemy, CompController comp)
    {
        if (enemy == null)
        {
            foreach (var keyPair in Instance._computersInEnemyInterRange)
            {
                if (keyPair.Value == null) continue;

                if (keyPair.Value.Contains(comp))
                {
                    keyPair.Value.Remove(comp);
                }
            }
        }
        else
        {
            List<CompController> compsInInterRangeForEnemy;
            Instance._computersInEnemyInterRange.TryGetValue(enemy, out compsInInterRangeForEnemy);

            if (compsInInterRangeForEnemy != null && compsInInterRangeForEnemy.Contains(comp))
            {
                compsInInterRangeForEnemy.Remove(comp);
            }
        }
    }

    static void RemoveAllComputersInEnemyInterRange(EnemySimpleAI enemy)
    {
        if (enemy == null) return;

        List<CompController> compsInInterRangeForEnemy;
        Instance._computersInEnemyInterRange.TryGetValue(enemy, out compsInInterRangeForEnemy);

        if (compsInInterRangeForEnemy != null)
        {
            compsInInterRangeForEnemy.Clear();
        }


        Instance._computersInEnemyInterRange.Remove(enemy);
    }

    public static void AddHackedComputer(CompController comp)
    {
        if (!Instance._hackedComputersList.Contains(comp))
        {
            Instance._hackedComputersList.Add(comp);
            Instance._hackedComputersStack.Push(comp);
        }
    }

    public static void RemoveHackedComputer(CompController comp)
    {
        if (Instance._hackedComputersList.Contains(comp))
        {
            Instance._hackedComputersList.Remove(comp);

            if (Instance._hackedComputersStack.Peek().Equals(comp))
            {
                Instance._hackedComputersStack.Pop();
            }
            else
            {
                var tempList = Instance._hackedComputersStack.ToList();
                tempList.Remove(comp);

                Instance._hackedComputersStack = new Stack<CompController>(tempList);
            }
        }
    }

    public static CompController GetLastHackedComputer()
    {
        return Instance._hackedComputersStack.Count > 0 ? Instance._hackedComputersStack.Peek() : null;
    }

    public static bool IsComputerHacked(CompController comp)
    {
        return Instance._hackedComputersList.Contains(comp);
    }
}
