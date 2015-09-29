using UnityEngine;
using System.Collections;

public class LaserPreAlloc : MonoBehaviour
{
    private Laser script;
    private static LaserPreAlloc Instance;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        script = GetComponent<Laser>();
        script.MissleSpawnPoint = transform;
        script.FireLaser();
    }

    public static bool IsInstanceNull()
    {
        return Instance == null;
    }
}
