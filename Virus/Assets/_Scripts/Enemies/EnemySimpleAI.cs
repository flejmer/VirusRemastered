using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemySimpleAI : MonoBehaviour
{
    public LayerMask MindControlRayMask;
    public GameObject Burst;

    [SerializeField]
    private float _hpPoints = 100;
    [SerializeField]
    protected float MaxHpPoints = 100;

    public float HealthPoints{ get { return _hpPoints; }}

    public bool PlayerControlled { get; protected set; }

    protected NavMeshAgent Agent;

    private List<SkinnedMeshRenderer> _mesh;
    private Color _originalColor;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        Agent = GetComponent<NavMeshAgent>();

        _mesh = new List<SkinnedMeshRenderer>(GetComponentsInChildren<SkinnedMeshRenderer>());
        _originalColor = _mesh[0].material.color;
    }

    public void AddHp(float count)
    {
        var hpAfterHeal = _hpPoints + count;
        _hpPoints = hpAfterHeal > MaxHpPoints ? MaxHpPoints : hpAfterHeal;
    }

    public void RemoveHp(float count)
    {
        var hpAfterDamage = _hpPoints - count;
        _hpPoints = hpAfterDamage < 0 ? 0 : hpAfterDamage;
    }

    public AudioSource GetAudioSource()
    {
        return _audioSource;
    }

    public abstract void HitPoint(Vector3 pos, Vector3 dir, float force, LayerMask mask);

    public void Highlight()
    {
        CancelInvoke("Dehighlight");

        foreach (var mrender in _mesh)
        {
            mrender.material.color = _originalColor + Color.red / 2;
        }

        Invoke("Dehighlight", 0.1f);
    }

    void Dehighlight()
    {
        foreach (var mrender in _mesh)
        {
            mrender.material.color = _originalColor;
        }
    }

    void OnEnable()
    {
        GameManager.AddEnemy(this);
    }

    void OnDisable()
    {
        GameManager.RemoveEnemy(this);
        SoundManager.Instance.AllSources.Remove(_audioSource);
    }

    public abstract void TakeOver();
    public abstract void GotHitBy(GameObject shooter);
}
