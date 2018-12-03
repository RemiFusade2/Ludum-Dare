using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToDoTaskBehaviour : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("References in prefab")]
    public Text taskNameText;
    public Text taskDurationText;
    public List<Text> taskDurationTextShadows;
    public Text skillBonusText;
    public Text skillInfoText;

    [Header("Info")]
    public TaskInfo taskInfo;

    [Header("Status")]
    public string taskName;
    public int durationInMinutes;
    public int skillBonusMalus;

    [Header("Settings")]
    public Color normalTextColor;
    public Color skillBonusTextColor;
    public Color skillMalusTextColor;

    private Coroutine updateVisualsCoroutine;

    public void OnDrag(PointerEventData eventData)
    {
        if (GameEngineBehaviour.instance.gameIsRunning)
        {
            GameEngineBehaviour.instance.DragTask(taskInfo);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameEngineBehaviour.instance.ValidateDragTask(taskInfo);
    }

    // Use this for initialization
    void Start ()
    {
    }

    public void InitalizeInfo(TaskInfo info)
    {
        taskInfo = info;
        taskName = info.taskName;
        durationInMinutes = info.baseDuration;
        UpdateVisuals();
        updateVisualsCoroutine = StartCoroutine(WaitAndUpdateVisuals(1));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator WaitAndUpdateVisuals(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateVisuals();
        if (GameEngineBehaviour.instance.remainingMinutes > 0)
        {
            updateVisualsCoroutine = StartCoroutine(WaitAndUpdateVisuals(delay));
        }
    }

    public void UpdateVisuals()
    {
        try
        {
            taskNameText.text = taskName;
            int durationAfterCorrection = Mathf.RoundToInt( durationInMinutes * 100.0f / (100 + GameDevStatusBehaviour.instance.GetEfficiencyBonus()) );

            string durationStr = "";
            if ((durationAfterCorrection / 60) > 0)
            {
                durationStr += (durationAfterCorrection / 60).ToString() + "H";
            }
            if ((durationAfterCorrection % 60) > 0)
            {
                durationStr += (durationAfterCorrection % 60).ToString("00") + "MN";
            }
            taskDurationText.text = durationStr;

            foreach (Text shadowText in taskDurationTextShadows)
            {
                shadowText.text = taskDurationText.text;
            }
            taskDurationText.color = GameDevStatusBehaviour.instance.GetColorFromDevStatus();
            if (skillBonusMalus == 0)
            {
                skillBonusText.text = "";
                skillInfoText.text = "";
                skillBonusText.color = skillInfoText.color = normalTextColor;
            }
            else if (skillBonusMalus < 0)
            {
                skillBonusText.text = "(" + skillBonusMalus.ToString() + "%)";
                skillInfoText.text = "Skilled";
                skillBonusText.color = skillInfoText.color = skillBonusTextColor;
            }
            else if (skillBonusMalus > 0)
            {
                skillBonusText.text = "(+" + skillBonusMalus.ToString() + "%)";
                skillInfoText.text = "Not skilled";
                skillBonusText.color = skillInfoText.color = skillMalusTextColor;
            }
        }
        catch(System.Exception)
        {

        }
    }

    public void SacrificeTask()
    {
        GameEngineBehaviour.instance.PlaySacrificeSound();
        // definitely remove task from pool
        GameEngineBehaviour.instance.RemoveTaskFromToDoList(taskInfo, false);
    }
}
