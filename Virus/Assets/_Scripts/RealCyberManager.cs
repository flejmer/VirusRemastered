using UnityEngine;
using System.Collections;

public class RealCyberManager : MonoBehaviour
{
    public static RealCyberManager Instance { get; private set; }

    private PlayerController _player;
    private CyberPlayer _cyberPlayer;

    private Camera _realCam;
    private Camera _cyperCam;

    public bool InCyberspace { get; private set; }

    private enum GameWorld
    {
        Real,
        Cyber
    }

    private GameWorld _gw;

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
        _player = GameManager.GetPlayer();
        _cyberPlayer = GameManager.GetCyberPlayer();
        _gw = GameWorld.Real;

        var cameras = new Camera[Camera.allCamerasCount];
        Camera.GetAllCameras(cameras);

        foreach (var camera1 in cameras)
        {
            if (camera1.name.Equals("MainCamera"))
            {
                _realCam = camera1;
            }
            else if (camera1.name.Equals("CyberCamera"))
            {
                _cyperCam = camera1;
            }
        }

        if (_cyperCam != null)
            _cyperCam.gameObject.SetActive(false);

        if(_cyberPlayer != null)
            _cyberPlayer.gameObject.SetActive(false);
    }

    public static bool IsInstanceNull()
    {
        return Instance == null;
    }

    public static void GoToCyberspace(CompController comp)
    {
        Instance._gw = GameWorld.Cyber;

        Instance._cyberPlayer.gameObject.SetActive(true);
        Instance._cyberPlayer.transform.position = comp.CyberComputer.transform.position;
        Instance._cyberPlayer.CurrentNode = comp.CyberComputer;

        Instance._player.gameObject.SetActive(false);
        Instance._cyperCam.gameObject.SetActive(true);

        Instance.InCyberspace = true;
    }

    public static void GoToRealWorld(ComputerNode comp)
    {
        Instance._gw = GameWorld.Real;

        Instance._player.gameObject.SetActive(true);

        var pos = comp.RealComputer.GetHackPosition();
        Instance._player.transform.position = new Vector3(pos.x, Instance._player.transform.position.y, pos.z);

        Instance._cyperCam.gameObject.SetActive(false);
        Instance._cyberPlayer.gameObject.SetActive(false);

        Instance.InCyberspace = false;
    }
}
