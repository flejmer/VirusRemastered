using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderAndText : MonoBehaviour
{
    public Slider Slider { get; private set; }
    public Text Text { get; private set; }

    // Use this for initialization
    void Start()
    {
        Slider = GetComponent<Slider>();

        foreach (var child in GetComponentsInChildren<Text>())
        {
            if (child.name.Equals("BigOne"))
            {
                Text = child;
                break;
            }
        }

        Slider.value = AudioListener.volume;

        ValueChanged();
    }

    public void ValueChanged()
    {
        Text.text = "Sound: " + (int)(Slider.value * 100) + "%";
        AudioListener.volume = Slider.value;
        GUIController.UpdateSlidersAndTexts(Slider.value, this);
    }

    public void UpdateST(float value)
    {
        if (Slider != null)
        {
            Slider.value = value;
            Text.text = "Sound: " + (int)(Slider.value * 100) + "%";
        }
    }
}
