using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoneTaskBehaviour : MonoBehaviour {

    [Header("References in prefab")]
    public Text taskNameText;

    [Header("Info")]
    public TaskInfo taskInfo;

    [Header("Status")]
    public string taskName;

    public void InitializeInfo(TaskInfo info)
    {
        taskInfo = info;
        taskName = info.taskName;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        try
        {
            taskNameText.text = taskName;
        }
        catch (System.Exception)
        {

        }
    }
}
