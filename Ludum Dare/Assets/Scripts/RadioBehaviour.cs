using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioBehaviour : MonoBehaviour
{

    public bool radioActive;

    public Animator radioAnimator;

    public AudioSource ambientMusic;

	// Use this for initialization
	void Start ()
    {
        UpdateRadio();
    }

    public void ToggleRadio()
    {
        radioActive = !radioActive;
        UpdateRadio();
    }

    private void UpdateRadio()
    {
        radioAnimator.SetBool("Active", radioActive);
        if (radioActive)
        {
            ambientMusic.UnPause();
        }
        else
        {
            ambientMusic.Pause();
        }
    }
}
