using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameFinish : MonoBehaviour
{
    public List<InteractionNode> NodesToActive = new List<InteractionNode>();

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
            // finish the game here
            Debug.Log("finish the game here");
        }
    }
}
