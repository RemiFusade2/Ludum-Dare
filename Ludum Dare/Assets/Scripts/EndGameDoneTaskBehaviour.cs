using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameDoneTaskBehaviour : MonoBehaviour
{ 
    [Header("References in prefab")]
    public Text taskNameText;

    [Header("Status")]
    public string taskName;

    public void InitializeInfo(TaskInfo info)
    {
        taskName = info.resultText;
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
