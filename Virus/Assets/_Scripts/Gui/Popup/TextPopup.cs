using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    private Text _titleText;
    private Text _infoText;

    // Use this for initialization
    void Awake()
    {
        var texts = GetComponentsInChildren<Text>();

        foreach (var text in texts)
        {
            if (text.gameObject.name.Equals("TitleText"))
            {
                _titleText = text;
            }
            else
            {
                _infoText = text;
            }
        }
    }

    public void SetTextPopup(string titleText, string infoText)
    {
        _titleText.text = titleText;
        _infoText.text = infoText;
    }
}
