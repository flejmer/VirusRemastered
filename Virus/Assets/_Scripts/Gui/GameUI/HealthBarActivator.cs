using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarActivator : MonoBehaviour
{
    private Image _image;

    [SerializeField]
    private Sprite _activatedSprite;
    [SerializeField]
    private Sprite _deactivatedSprite;

    public bool Activated;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        _image.sprite = Activated ? _activatedSprite : _deactivatedSprite;
    }
}
