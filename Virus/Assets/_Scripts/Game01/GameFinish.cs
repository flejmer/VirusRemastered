using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameFinish : MonoBehaviour
{
    public List<InteractionNode> NodesToActive = new List<InteractionNode>();
    private bool _notActivated = true;

    void OnEnable()
    {
        RealCyberManager.ExitHappened += FinishGame;
    }

    void OnDisable()
    {
        // ReSharper disable once DelegateSubtraction
        RealCyberManager.ExitHappened -= FinishGame;
    }

    void FinishGame()
    {
        var counter = NodesToActive.Count(node => node.IsUnlocked());

        if (counter == NodesToActive.Count)
        {
            GameManager.Instance.GameWon();
            GUIController.UpdateMissionStatus("LEVEL CLEARED");
        }
    }

    void Update()
    {
        if (_notActivated)
        {
            if (NodesToActive.Count(node => node.IsUnlocked()) == NodesToActive.Count)
            {
                _notActivated = false;
                GUIController.UpdateMissionStatus("Exit cyberspace");
            }
        }
    }
}
