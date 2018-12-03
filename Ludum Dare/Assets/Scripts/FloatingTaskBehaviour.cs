using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTaskBehaviour : MonoBehaviour
{
    [Header("References in prefab")]
    public Text taskNameText;

    [Header("Status")]
    public int durationInMinutes;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void InitializeTaskInfo(string taskName, int duration)
    {
        taskNameText.text = taskName;
        this.durationInMinutes = duration;
    }
}
