using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class SkillsSprites
{
    public Sprite LaserActivated;
    public Sprite LaserDeactivated;

    public Sprite MindControlActivated;
    public Sprite MindControlDeactivated;

    public Sprite ShieldActivated;
    public Sprite ShieldDeactivated;

    public Sprite SlowMotionActivated;
    public Sprite SlowMotionDeactivated;

    public Sprite HologramActivated;
    public Sprite HologramDeactivated;
}

public class SkillsPanelController : MonoBehaviour
{
    public SkillsSprites Sprites;

    private Image _laser;
    private Image _mind;
    private Image _shield;
    private Image _slow;
    private Image _holo;

    // Use this for initialization
    void Start()
    {
        var skills = GetComponentsInChildren<Image>();

        foreach (var skill in skills)
        {
            if (skill.name.Equals("Laser"))
            {
                _laser = skill;
            }
            else if (skill.name.Equals("MindControl"))
            {
                _mind = skill;
            }
            else if (skill.name.Equals("Shield"))
            {
                _shield = skill;
            }
            else if (skill.name.Equals("SlowMotion"))
            {
                _slow = skill;
            }
            else if (skill.name.Equals("Hologram"))
            {
                _holo = skill;
            }
        }
    }

    public void UpdateAll()
    {
        UpdateLaserSprite();
        UpdateMindControlSprite();
        UpdateShieldSprite();
        UpdateSlowMotionSprite();
        UpdateHologramSprite();
    }

    public void UpdateLaserSprite()
    {
        _laser.sprite = RealCyberManager.GetPlayer().LaserUnlocked ? Sprites.LaserActivated : Sprites.LaserDeactivated;
    }

    public void UpdateMindControlSprite()
    {
        _mind.sprite = RealCyberManager.GetPlayer().MindControlUnlocked ? Sprites.MindControlActivated : Sprites.MindControlDeactivated;
    }

    public void UpdateShieldSprite()
    {
        _shield.sprite = RealCyberManager.GetPlayer().ShieldUnlocked ? Sprites.ShieldActivated : Sprites.ShieldDeactivated;
    }

    public void UpdateSlowMotionSprite()
    {
        _slow.sprite = RealCyberManager.GetPlayer().SlowMotionUnlocked ? Sprites.SlowMotionActivated : Sprites.SlowMotionDeactivated;
    }

    public void UpdateHologramSprite()
    {
        _holo.sprite = RealCyberManager.GetPlayer().HologramUnlocked ? Sprites.HologramActivated : Sprites.HologramDeactivated;
    }
}
