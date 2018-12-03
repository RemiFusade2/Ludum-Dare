using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoingTaskBehaviour : MonoBehaviour
{
    [Header("References in prefab")]
    public Slider completionSlider;
    public Text taskNameText;

    [Header("Status")]
    public TaskInfo taskInfo;
    public string taskName;
    public float timeToCompleteTask; // in minutes
    public float completion; // between 0 and 1
    public bool isCompleting;
    public bool isCompleted;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
    public void InitializeTask(float time)
    {
        timeToCompleteTask = time;
        completion = 0;
        isCompleted = false;
        UpdateSlider();
    }*/

    public void InitializeInfo(TaskInfo info)
    {
        taskInfo = info;
        taskName = info.taskName;
        completion = info.GetCompletion();
        timeToCompleteTask = info.baseDuration;
        isCompleted = false;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        taskNameText.text = taskName;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        completionSlider.value = completion;
    }

    public void IncreaseCompletion(float minutesPassed)
    {
        isCompleting = true;
        completion += (100 + GameDevStatusBehaviour.instance.GetEfficiencyBonus())/100.0f * (minutesPassed / timeToCompleteTask);
        if (completion >= 1)
        {
            completion = 1;
            isCompleted = true;
            isCompleting = false;
        }
        UpdateSlider();
    }

    public void SacrificeTask()
    {
        GameEngineBehaviour.instance.PlaySacrificeSound();
        // definitely remove task from pool
        GameEngineBehaviour.instance.RemoveTaskFromInProgressList(taskInfo);
    }
}
