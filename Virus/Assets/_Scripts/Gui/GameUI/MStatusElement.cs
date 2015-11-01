using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MStatusElement : MonoBehaviour
{
    private Text _text;

    void Awake()
    {
        var texts = GetComponentsInChildren<Text>();

        foreach (var text in texts)
        {
            if (!text.transform.parent.gameObject.Equals(gameObject)) continue;
            _text = text;
            return;
        }

        Debug.Log("hello");
    }

    public void SetMStatusText(string text)
    {
        _text.text = text;
    }
}
