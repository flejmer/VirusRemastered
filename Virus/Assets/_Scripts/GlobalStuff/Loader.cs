using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loader : MonoBehaviour
{
    public GameObject GameMan;
    public GameObject SoundMan;
    public GameObject GuiMan;
    public GameObject RealCyberMan;

    void Awake()
    {
        if (GameManager.IsInstanceNull())
        {
            Instantiate(GameMan);
        }

        if (GUIController.IsInstanceNull())
        {
            Instantiate(GuiMan);
        }

        if (RealCyberManager.IsInstanceNull())
        {
            Instantiate(RealCyberMan);
        }

        if (SoundManager.Instance == null)
        {
            Instantiate(SoundMan);
        }
    }
}
