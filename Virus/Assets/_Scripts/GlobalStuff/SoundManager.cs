using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip NodeUnlockSound;
    public AudioClip ChuckDeathSound;
    public AudioClip EnemyAttackSound;
    public AudioClip TurretAttackSound;
    public AudioClip PlayerBasicAttackSound;
    public AudioClip PlayerSecondaryAttackSound;
    public AudioClip PlayerMindControlAttackSound;
    public AudioClip ExplosionSound;
    public AudioClip TeleportSound;
    public AudioClip InfectionSound;
    public AudioClip ImpactSound;
    public AudioClip DoorSound;
    public AudioClip ComputerHackedSound;

    public AudioClip MenuMusic;
    public AudioClip GameMusic;

    private AudioSource _audioSource;
    public List<AudioSource> AllSources = new List<AudioSource>();



    void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void MuteBackgroundMusic()
    {
        StartCoroutine(VolumeToZero());
    }

    IEnumerator VolumeToZero()
    {
        while (_audioSource.volume > 0)
        {
            yield return new WaitForEndOfFrame();

            _audioSource.volume -= Time.deltaTime * 1;
        }
    }

    public static void SetSourcesForThisScene(AudioSource[] sources)
    {
        Instance.AllSources = new List<AudioSource>(sources);
    }

    public static void SetPitch(float pitch)
    {
        foreach (var source in Instance.AllSources)
        {
            if (!source.gameObject.CompareTag("Computer"))
            {
                source.pitch = pitch;
            }
        }
    }

    public static void PlayMenuMusic()
    {
        Instance._audioSource.clip = Instance.MenuMusic;
        Instance._audioSource.timeSamples = 0;
        Instance._audioSource.loop = true;
        Instance._audioSource.Play();
    }

    public static void PlayGameMusic()
    {
        Instance._audioSource.clip = Instance.GameMusic;
        Instance._audioSource.timeSamples = 0;
        Instance._audioSource.loop = true;
        Instance._audioSource.Play();
    }

    public static void StopBackgroundMusic()
    {
        Instance._audioSource.Stop();
    }

    public static void PlayNodeUnlockSound(AudioSource source)
    {
        source.PlayOneShot(Instance.NodeUnlockSound, 1);
    }

    public static void PlayChuckDeathSound(AudioSource source)
    {
        source.PlayOneShot(Instance.ChuckDeathSound, 1);
    }

    public static void PlayEnemyAttackSound(AudioSource source)
    {
        source.PlayOneShot(Instance.EnemyAttackSound, 1);
    }

    public static void PlayTurretAttackSound(AudioSource source)
    {
        source.PlayOneShot(Instance.TurretAttackSound, 1);
    }

    public static void PlayPlayerAttackSound(AudioSource source)
    {
        source.PlayOneShot(Instance.PlayerBasicAttackSound, 1);
    }

    public static void PlayPlayerAttack2Sound(AudioSource source)
    {
        source.PlayOneShot(Instance.PlayerSecondaryAttackSound, 1);
    }

    public static void PlayPlayerMindControlSound(AudioSource source)
    {
        source.PlayOneShot(Instance.PlayerMindControlAttackSound, 1);
    }

    public static void PlayEnemyBurstSound(AudioSource source)
    {
        source.PlayOneShot(Instance.ExplosionSound, 1);
    }

    public static void PlayCyberspaceSound(AudioSource source)
    {
        source.clip = Instance.TeleportSound;
        source.timeSamples = source.clip.samples - 1;
        source.pitch = -1;
        source.Play();
    }

    public static void PlayTeleportSound(AudioSource source)
    {
        source.clip = Instance.TeleportSound;
        source.timeSamples = 0;
        source.pitch = 1;
        source.Play();
    }

    public static void PlayInfectionSound(AudioSource source)
    {
        source.PlayOneShot(Instance.InfectionSound, 1);
    }

    public static void PlayImpactSound(AudioSource source)
    {
        source.PlayOneShot(Instance.ImpactSound, 1);
    }

    public static void PlayDoorSound(AudioSource source)
    {
        source.PlayOneShot(Instance.DoorSound, 1);
    }

    public static void PlayComputerHackedSound(AudioSource source)
    {
        source.PlayOneShot(Instance.ComputerHackedSound, 1);
    }
}
