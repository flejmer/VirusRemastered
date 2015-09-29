using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public Animator MenuAnimator;
    private bool _pressed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.A))
	    {
	        if (!_pressed)
	        {
	            MenuAnimator.SetTrigger("FadeOut");
                _pressed = !_pressed;
            }
	        else
	        {
                MenuAnimator.SetTrigger("FadeIn");
	            _pressed = !_pressed;
	        }
	    }
	}
}
