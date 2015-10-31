using UnityEngine;
using System.Collections;

public class RealCyberManager : MonoBehaviour
{
    public static RealCyberManager Instance { get; private set; }

    private PlayerController _player;
    private CyberPlayer _cyberPlayer;

    private Camera _realCam;
    private CameraFollow _realCamScript;
    private Camera _cyberCam;
    private CameraFollow _cyberCamScript;

    public bool InCyberspace { get; private set; }

    public delegate void ExitCyberspace();
    public static ExitCyberspace ExitHappened;

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
                _realCamScript = _realCam.gameObject.GetComponent<CameraFollow>();
            }
            else if (camera1.name.Equals("CyberCamera"))
            {
                _cyberCam = camera1;
                _cyberCamScript = _cyberCam.gameObject.GetComponent<CameraFollow>();
            }
        }

        if (_cyberCam != null)
        {
            _cyberCam.gameObject.SetActive(false);
        }

        if (_cyberPlayer != null)
        {
            _cyberPlayer.gameObject.SetActive(false);
        }
    }

    public static void ShowPointOfInterest(Vector3 point)
    {
        Instance._cyberCamScript.CheckThisOut(point);
    }

    public static bool IsInstanceNull()
    {
        return Instance == null;
    }

    public static void GoToCyberspace(CompController comp)
    {
        if (comp.EnemiesAround)
        {
            Debug.Log("enemies are around main frame INFO");
        }
        else
        {
            SoundManager.PlayCyberspaceSound(comp.GetAudioSource());

            Instance._gw = GameWorld.Cyber;

            Instance._cyberPlayer.gameObject.SetActive(true);
            Instance._cyberPlayer.transform.position = comp.CyberComputer.transform.position;
            Instance._cyberPlayer.CurrentNode = comp.CyberComputer;

            Instance._player.gameObject.SetActive(false);
            Instance._cyberCam.gameObject.SetActive(true);

            Instance.InCyberspace = true;
        }
    }

    public static void GoToRealWorld(ComputerNode comp)
    {
        SoundManager.PlayTeleportSound(comp.RealComputer.GetAudioSource());

        Instance._gw = GameWorld.Real;

        Instance._player.gameObject.SetActive(true);

        var pos = comp.RealComputer.GetHackPosition();
        Instance._player.transform.position = new Vector3(pos.x, Instance._player.transform.position.y, pos.z);

        Instance._cyberCam.gameObject.SetActive(false);
        Instance._cyberPlayer.gameObject.SetActive(false);

        Instance.InCyberspace = false;

        if(comp.RealComputer.CompareTag("MainFrame"))
            ExitHappened();
    }

    public static PlayerController GetPlayer()
    {
        return Instance._player;
    }

    public static CyberPlayer GetCyberPlayer()
    {
        return Instance._cyberPlayer;
    }
}
