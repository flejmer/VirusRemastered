using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;
    private AudioListener _allsound;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
