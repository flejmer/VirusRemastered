using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour
{
    private bool _active;

    public bool Active
    {
        get { return _active; }
        private set
        {
            _active = value;

            if (!value)
            {
                gameObject.SetActive(false);
                DisableAllPopups();

                GameManager.Instance.InGameState = Enums.InGameStates.Normal;

                if (GameManager.Instance.SlowMotionActivated)
                {
                    Time.timeScale = Time.timeScale = 1 * GameManager.Instance.SlowMotionRate;
                    RealCyberManager.GetPlayer().ResumeAnimations();
                    return;
                }

                Time.timeScale = 1;
                RealCyberManager.GetPlayer().ResumeAnimations();

            }
            else
            {
                gameObject.SetActive(true);

                if (!GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause))
                {
                    Time.timeScale = 0;
                    RealCyberManager.GetPlayer().StopAnimations();
                }
            }
        }
    }

    private TextPopup _textPopup;
    private HackingInfoPopup _hackingInfoPopup;
    private LaserUnlockedPopup _laserUnlockedPopup;
    private MindControlUnlockedPopup _mindControlUnlockedPopup;
    private HologramUnlockedPopup _hologramUnlockedPopup;
    private SlowMotionUnlockedPopup _slowMotionUnlockedPopup;
    private ShieldUnlockedPopup _shieldUnlockedPopup;

    void Awake()
    {
        _textPopup = GetComponentInChildren<TextPopup>();
        _hackingInfoPopup = GetComponentInChildren<HackingInfoPopup>();
        _laserUnlockedPopup = GetComponentInChildren<LaserUnlockedPopup>();
        _mindControlUnlockedPopup = GetComponentInChildren<MindControlUnlockedPopup>();
        _hologramUnlockedPopup = GetComponentInChildren<HologramUnlockedPopup>();
        _slowMotionUnlockedPopup = GetComponentInChildren<SlowMotionUnlockedPopup>();
        _shieldUnlockedPopup = GetComponentInChildren<ShieldUnlockedPopup>();

        DisableAllPopups();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.InGameState.Equals(Enums.InGameStates.Pause))
        {
            Active = false;
        }
    }

    void DisableAllPopups()
    {
        _textPopup.gameObject.SetActive(false);
        _hackingInfoPopup.gameObject.SetActive(false);
        _laserUnlockedPopup.gameObject.SetActive(false);
        _mindControlUnlockedPopup.gameObject.SetActive(false);
        _hologramUnlockedPopup.gameObject.SetActive(false);
        _slowMotionUnlockedPopup.gameObject.SetActive(false);
        _shieldUnlockedPopup.gameObject.SetActive(false);
    }

    public void DeactivatePopup()
    {
        Active = false;
    }

    public void ActivateTextPopup(string title, string text)
    {
        Active = true;
        _textPopup.gameObject.SetActive(true);
        _textPopup.SetTextPopup(title, text);
    }

    public void ActivateHackingInfo()
    {
        Active = true;
        _hackingInfoPopup.gameObject.SetActive(true);
    }

    public void ActivateLaserUnlockedInfo()
    {
        Active = true;
        _mindControlUnlockedPopup.gameObject.SetActive(true);
    }

    public void ActivateMindControlUnlockedInfo()
    {
        Active = true;
        _mindControlUnlockedPopup.gameObject.SetActive(true);
    }

    public void ActivateHologramUnlockedInfo()
    {
        Active = true;
        _hologramUnlockedPopup.gameObject.SetActive(true);
    }

    public void ActivateSlowMotionUnlockedInfo()
    {
        Active = true;
        _slowMotionUnlockedPopup.gameObject.SetActive(true);
    }

    public void ActivateShieldUnlockedInfo()
    {
        Active = true;
        _shieldUnlockedPopup.gameObject.SetActive(true);
    }
}
