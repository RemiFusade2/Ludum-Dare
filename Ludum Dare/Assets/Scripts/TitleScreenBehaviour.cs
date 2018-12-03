using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenBehaviour : MonoBehaviour
{
    public Animator titleScreenAnimator;

    public AudioSource startGameAudio;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PressStartJamButton()
    {
        titleScreenAnimator.SetTrigger("Disappear");
        startGameAudio.Stop();
        startGameAudio.Play();
    }

    public void DestroyTitleScreen()
    {
        Destroy(this.gameObject);
    }

    public void StartGame()
    {
        GameEngineBehaviour.instance.StartGame();
    }
}
