using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public enum TaskSkillType
{
    CODE, // add bugs
    ART, // add last minute tweaks
    SOUND, // add last minute tweaks
    GAME_DESIGN, // add balancing
    COFFEE, // improve motivation but decrease HP
    SLEEP, // increase HP
    USELESS
}

[System.Serializable]
public class TaskInfo
{
    public string taskName;
    public string resultText;
    public int baseDuration;
    private float completion;
    public bool onlyOnce;
    public bool critical;
    public TaskSkillType type;
    
    public TaskInfo(string name, string result, int duration, float completionRatio, bool once, bool critic, TaskSkillType t)
    {
        this.taskName = name;
        this.resultText = result;
        this.baseDuration = duration;
        this.completion = completionRatio;
        this.onlyOnce = once;
        this.critical = critic;
        this.type = t;
    }

    public static TaskInfo CopyFrom(TaskInfo from)
    {
        TaskInfo newTask = new TaskInfo(from.taskName, from.resultText, from.baseDuration, from.completion, from.onlyOnce, from.critical, from.type);
        return newTask;
    }

    public float GetCompletion()
    {
        return completion;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        TaskInfo c = obj as TaskInfo;
        if ((object)c == null)
            return false;
        return taskName.Equals(c.taskName);
    }

    public bool Equals(TaskInfo c)
    {
        if ((object)c == null)
            return false;
        return taskName.Equals(c.taskName);
    }

    public void ResetCompletion()
    {
        completion = 0;
    }
}

public class GameEngineBehaviour : MonoBehaviour
{
    public static GameEngineBehaviour instance;
    
    [Header("References in scene")]
    public Text timerText;
    public Transform taskPanelTransform;
    public RectTransform toDoTasksContentRectTransform;
    public RectTransform inProgressTasksContentRectTransform;
    public RectTransform inProgressTasksPanelRectTransform;
    public RectTransform doneTasksContentRectTransform;

    public MainButtonBehaviour doStuffButton;
    public MainButtonBehaviour coffeeButton;
    public MainButtonBehaviour sleepButton;

    public GameObject workingCharacter;
    public Animator characterAnimator;

    public EndGameScreenBehaviour EndGameScreen;

    [Header("Audio references")]
    public AudioSource dragStartAudio;
    public AudioSource dragEndAudio;
    public AudioSource sacrificeAudio;
    public AudioSource taskFinishedAudio;
    public AudioSource backToWorkAudio;
    public AudioSource prepareCoffeeAudio;
    public AudioSource goToSleepAudio;

    [Header("Prefabs")]
    public GameObject ToDoTaskPrefab;
    public GameObject DoingTaskPrefab;
    public GameObject FloatingTaskPrefab;
    public GameObject DoneTaskPrefab;

    [Header("Settings")]
    public float gameSpeed; // inGameMinutesByRealTimeSeconds
    public float remainingMinutes;

    [Header("Content")]
    public List<TaskInfo> mustHaveTasks; // main functionalities for a working game
    public List<TaskInfo> polishTasks; // comes near the end of the project, don't add any value
    public List<TaskInfo> debugTasks; // comes after coding tasks
    public List<TaskInfo> lastMinuteArtTweaksTasks; // comes after art tasks
    public List<TaskInfo> lastMinuteSoundTweaksTasks; // comes after sound tasks
    public List<TaskInfo> balancingTasks; // comes after game design tasks
    public List<TaskInfo> procrastinationTasks; // polution

    [Header("Status")]
    public bool isDraggingTask;
    private FloatingTaskBehaviour draggedTask;
    public List<TaskInfo> tasksToDo;
    private List<TaskInfo> tasksInProgress;
    private List<DoingTaskBehaviour> tasksInProgressScripts;
    private List<TaskInfo> tasksDone;

    [Header("Debug")]
    public DoingTaskBehaviour activeTask;

    private List<DoingTaskBehaviour> listOfTasksToDo;

    private Coroutine passTimeCoroutine;
    private Coroutine refillToDoListCoroutine;

    public bool gameIsRunning;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        //tasksToDo = new List<TaskInfo>();
        tasksInProgressScripts = new List<DoingTaskBehaviour>();
        tasksInProgress = new List<TaskInfo>();
        tasksDone = new List<TaskInfo>();
        gameIsRunning = false;

        // To Do Tasks
        FillToDoTasks(true);
        for (int i = 0; i < 3; i++)
        {
            FillToDoTasks(false);
        }
        UpdateToDoListVisuals();

        /*
        TaskInfo procrastinating = new TaskInfo("Procrastinate", "", 4000, 0.1f, false, false, TaskSkillType.USELESS);
        procrastinating.ResetCompletion();
        tasksInProgress.Add(procrastinating);*/

        UpdateInProgressListVisuals();
        if (tasksInProgressScripts.Count > 0)
        {
            activeTask = tasksInProgressScripts[0];
        }

        UpdateDoneTasksListVisuals();
        GameDevStatusBehaviour.instance.GiveMotivationBoost(180, 0);
    }

    public void StartGame()
    {
        refillToDoListCoroutine = StartCoroutine(WaitAndRefillToDoList(0.5f));
        

        UpdateDoneTasksListVisuals();

        isDraggingTask = false;

        gameIsRunning = true;

        characterAnimator.SetBool("IsWorking", true);
    }


    public void EndGame()
    {
        characterAnimator.SetBool("IsWorking", false);
        EndGameScreen.DisplayEndGameScreen(tasksDone);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (remainingMinutes > 0)
        {
            if (gameIsRunning)
            {
                PassTime();
            }
        }
        else
        {
            if (gameIsRunning)
            {
                // end game
                gameIsRunning = false;
                EndGame();
            }
        }

    }

    private IEnumerator WaitAndRefillToDoList(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isDraggingTask)
        {
            FillToDoTasks(false);
            UpdateToDoListVisuals();
        }
        if (remainingMinutes > 0)
        {
            refillToDoListCoroutine = StartCoroutine(WaitAndRefillToDoList(delay));
        }
    }

    public void ResetButtons()
    {
        doStuffButton.ResetButton();
        coffeeButton.ResetButton();
        sleepButton.ResetButton();
    }


    public void PrepareCoffee()
    {
        StartCoroutine(WaitAndDrinkCoffee(2.0f));        
        prepareCoffeeAudio.Stop();
        prepareCoffeeAudio.Play();
    }
    private IEnumerator WaitAndDrinkCoffee(float delay)
    {
        yield return new WaitForSeconds(delay);
        TakeCoffee();
    }
    public void TakeCoffee()
    {
        GameDevStatusBehaviour.instance.GiveMotivationBoost(120, 30);
        ResetButtons();
        doStuffButton.ButtonPressed();
    }

    public void GoToSleep()
    {
        workingCharacter.SetActive(false);
        goToSleepAudio.Stop();
        goToSleepAudio.Play();
    }
    public void BackToWork()
    {
        workingCharacter.SetActive(true);
        characterAnimator.SetBool("IsWorking", true);
        backToWorkAudio.Stop();
        backToWorkAudio.Play();
    }

    private void PassTime()
    {        
        float minutesPassed = gameSpeed * Time.deltaTime;

        // Sleep
        if (sleepButton.actionActive)
        {
            GameDevStatusBehaviour.instance.Sleep(minutesPassed);
        }
        else
        {
            // Change game dev status
            GameDevStatusBehaviour.instance.TimePass(minutesPassed);
        }

        /*
        if (activeTask != null && activeTask.taskName.Equals("Procrastinate") && tasksInProgressScripts.Count > 1)
        {
            tasksInProgress.Remove(activeTask.taskInfo);
            tasksInProgressScripts.Remove(activeTask);
            UpdateInProgressListVisuals();
        }*/

        // Complete task
        if (activeTask != null && doStuffButton.actionActive)
        {
            activeTask.IncreaseCompletion(minutesPassed);
            if (activeTask.isCompleted)
            {
                taskFinishedAudio.Stop();
                taskFinishedAudio.Play();

                if (activeTask.taskInfo.type == TaskSkillType.COFFEE)
                {
                    // We just made coffee
                    coffeeButton.RefillCoffee();
                }
                else
                {
                    // we did something else which is more important
                    lock (tasksDone)
                    {
                        tasksDone.Add(activeTask.taskInfo);
                    }
                    UpdateDoneTasksListVisuals();
                }

                lock (tasksInProgress)
                {
                    tasksInProgress.Remove(activeTask.taskInfo);
                }
                activeTask = null;
                UpdateInProgressListVisuals();
            }
        }
        if (activeTask == null && tasksInProgressScripts.Count > 0)
        {
            activeTask = tasksInProgressScripts[0];
        }

        // Update timer value
        remainingMinutes -= minutesPassed;
        remainingMinutes = (remainingMinutes <= 0) ? 0 : remainingMinutes;
        int remainingHours = Mathf.FloorToInt(remainingMinutes / 60);
        int timerMinutes = Mathf.FloorToInt(remainingMinutes) % 60;
        timerText.text = remainingHours.ToString() + ":" + timerMinutes.ToString("00");
        float timerHeight = 30 - ((remainingMinutes - 60) / (200)) * 10;
        timerHeight = (timerHeight < 20) ? 20 : ((timerHeight > 30) ? 30 : timerHeight);
        timerText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, timerHeight);
    }

    public void DragTask(TaskInfo task)
    {
        if (!isDraggingTask)
        {
            dragStartAudio.Stop();
            dragStartAudio.Play();

            GameObject taskObj = Instantiate(FloatingTaskPrefab, taskPanelTransform);
            draggedTask = taskObj.GetComponent<FloatingTaskBehaviour>();
            draggedTask.InitializeTaskInfo(task.taskName, task.baseDuration);
            isDraggingTask = true;
        }

        draggedTask.transform.position = Input.mousePosition;
    }

    public void ValidateDragTask(TaskInfo task)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(inProgressTasksPanelRectTransform, Input.mousePosition))
        {
            dragEndAudio.Stop();
            dragEndAudio.Play();

            RemoveTaskFromToDoList(task, false);
            lock (tasksInProgress)
            {
                task.ResetCompletion();
                tasksInProgress.Add(task);
            }
            UpdateInProgressListVisuals();
        }
        Destroy(draggedTask.gameObject);
        draggedTask = null;
        isDraggingTask = false;
    }

    public void FillToDoTasks(bool reset)
    {
        int maxNumberOfTasks = 15;
        if (reset)
        {
            lock (tasksToDo)
            {
                tasksToDo.Clear();
            }
        }

        if (tasksToDo.Count < maxNumberOfTasks)
        {
            int codeTaskDone = 0;
            int artTaskDone = 0;
            int soundTaskDone = 0;
            int gdTaskDone = 0;
            foreach (TaskInfo task in tasksDone)
            {
                switch (task.type)
                {
                    case TaskSkillType.CODE:
                        codeTaskDone++;
                        break;
                    case TaskSkillType.ART:
                        artTaskDone++;
                        break;
                    case TaskSkillType.SOUND:
                        soundTaskDone++;
                        break;
                    case TaskSkillType.GAME_DESIGN:
                        gdTaskDone++;
                        break;
                    default:
                        break;
                }
            }

            List<List<TaskInfo>> randomListOfTasks = new List<List<TaskInfo>>();

            randomListOfTasks.Add(procrastinationTasks);
            randomListOfTasks.Add(mustHaveTasks);
            randomListOfTasks.Add(mustHaveTasks);
            randomListOfTasks.Add(mustHaveTasks);
            randomListOfTasks.Add(mustHaveTasks);
            randomListOfTasks.Add(mustHaveTasks);
            randomListOfTasks.Add(mustHaveTasks);
            randomListOfTasks.Add(mustHaveTasks);

            if (remainingMinutes < 1440)
            {
                randomListOfTasks.Add(polishTasks);
            }
            if (codeTaskDone > 2)
            {
                randomListOfTasks.Add(debugTasks);
            }
            if (artTaskDone > 2)
            {
                randomListOfTasks.Add(lastMinuteArtTweaksTasks);
            }
            if (soundTaskDone > 2)
            {
                randomListOfTasks.Add(lastMinuteSoundTweaksTasks);
            }
            if (gdTaskDone > 2)
            {
                randomListOfTasks.Add(balancingTasks);
            }

            if (GameDevStatusBehaviour.instance.GetEfficiencyBonus() < -30)
            {
                randomListOfTasks.Add(procrastinationTasks);
                randomListOfTasks.Add(procrastinationTasks);
                randomListOfTasks.Add(procrastinationTasks);
                randomListOfTasks.Add(procrastinationTasks);
                randomListOfTasks.Add(procrastinationTasks);
            }
            else if (GameDevStatusBehaviour.instance.GetEfficiencyBonus() < -10)
            {
                randomListOfTasks.Add(procrastinationTasks);
                randomListOfTasks.Add(procrastinationTasks);
            }
            else if (GameDevStatusBehaviour.instance.GetEfficiencyBonus() < 25)
            {
                randomListOfTasks.Add(mustHaveTasks);
                randomListOfTasks.Add(mustHaveTasks);
                randomListOfTasks.Add(procrastinationTasks);
            }
            else
            {
                randomListOfTasks.Add(mustHaveTasks);
                randomListOfTasks.Add(mustHaveTasks);
                randomListOfTasks.Add(mustHaveTasks);
                randomListOfTasks.Add(mustHaveTasks);
            }


            if (coffeeButton.availableCoffees <= 0)
            {
                // NO MORE COFFEES ?
                TaskInfo makeCoffeeTask = new TaskInfo("Make Coffee", "", 30, 0, false, false, TaskSkillType.COFFEE);
                makeCoffeeTask.ResetCompletion();

                // Check if coffee is already in todo list or doing list
                if (!tasksToDo.Contains(makeCoffeeTask) && !tasksInProgress.Contains(makeCoffeeTask))
                {
                    lock (tasksToDo)
                    {
                        tasksToDo.Add(makeCoffeeTask);
                    }
                }
            }
            
            List<TaskInfo> selectedListOfTask = randomListOfTasks[Random.Range(0, randomListOfTasks.Count)];
            if (selectedListOfTask.Count > 0)
            {
                TaskInfo selectedTask = TaskInfo.CopyFrom(selectedListOfTask[Random.Range(0, selectedListOfTask.Count)]);

                if (!tasksToDo.Contains(selectedTask) && !tasksInProgress.Contains(selectedTask) && (!tasksDone.Contains(selectedTask) || !selectedTask.onlyOnce))
                {
                    selectedTask.ResetCompletion();
                    lock (tasksToDo)
                    {
                        tasksToDo.Add(selectedTask);
                    }
                }
            }

            /*
            float maximumDuration = 0.1f;
            float timeBeforeLoop = Time.time;
            bool taskHasBeenAdded = false;
            while ((tasksToDo.Count <= maxNumberOfTasks) && !taskHasBeenAdded && ((Time.time - timeBeforeLoop) < maximumDuration))
            {
                List<TaskInfo> selectedListOfTask = randomListOfTasks[Random.Range(0, randomListOfTasks.Count)];
                if (selectedListOfTask.Count <= 0)
                {
                    continue;
                }

                TaskInfo selectedTask = selectedListOfTask[Random.Range(0, selectedListOfTask.Count)];

                if (!tasksToDo.Contains(selectedTask) && !tasksInProgress.Contains(selectedTask) && (!tasksDone.Contains(selectedTask) || !selectedTask.onlyOnce))
                {
                    lock (tasksToDo)
                    {
                        selectedTask.ResetCompletion();
                        tasksToDo.Add(selectedTask);
                    }
                    taskHasBeenAdded = true;
                }
            }*/
        }
    }

    private void UpdateToDoListVisuals()
    {
        float heightOfTask = 90;
        float spaceBetweenTasks = 5;
        foreach (Transform todoTasks in toDoTasksContentRectTransform.transform)
        {
            Destroy(todoTasks.gameObject);
        }
        
        lock (tasksToDo)
        {
            toDoTasksContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightOfTask * tasksToDo.Count + spaceBetweenTasks * (tasksToDo.Count + 1));
            Vector2 taskCellPosition = new Vector2(0, -spaceBetweenTasks);
            foreach (TaskInfo taskInfo in tasksToDo)
            {
                GameObject taskCellObj = Instantiate(ToDoTaskPrefab, toDoTasksContentRectTransform.transform);
                taskCellObj.GetComponent<ToDoTaskBehaviour>().InitalizeInfo(TaskInfo.CopyFrom(taskInfo));
                taskCellObj.GetComponent<RectTransform>().anchoredPosition = taskCellPosition;
                taskCellPosition.y -= (heightOfTask + spaceBetweenTasks);
            }
        }
    }

    private void UpdateInProgressListVisuals()
    {
        float heightOfTask = 90;
        float spaceBetweenTasks = 5;

        bool first = true;
        bool activeTaskStillActive = false;
        foreach (Transform doingTask in inProgressTasksContentRectTransform.transform)
        {
            if (first && activeTask != null && activeTask == doingTask.GetComponent<DoingTaskBehaviour>())
            {
                first = false;
                activeTaskStillActive = true;
                continue;
            }
            tasksInProgressScripts.Remove(doingTask.GetComponent<DoingTaskBehaviour>());
            Destroy(doingTask.gameObject);
        }
        
        lock (tasksInProgress)
        {
            inProgressTasksContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightOfTask * tasksInProgress.Count + spaceBetweenTasks * (tasksInProgress.Count + 1));
            Vector2 taskCellPosition = new Vector2(0, -spaceBetweenTasks);
            first = true;
            foreach (TaskInfo taskInfo in tasksInProgress)
            {
                if (first && activeTask != null && activeTaskStillActive)
                {
                    first = false;
                    taskCellPosition.y -= (heightOfTask + spaceBetweenTasks);
                    continue;
                }
                
                GameObject taskCellObj = Instantiate(DoingTaskPrefab, inProgressTasksContentRectTransform.transform);
                taskCellObj.GetComponent<DoingTaskBehaviour>().InitializeInfo(TaskInfo.CopyFrom( taskInfo ));
                tasksInProgressScripts.Add(taskCellObj.GetComponent<DoingTaskBehaviour>());
                taskCellObj.GetComponent<RectTransform>().anchoredPosition = taskCellPosition;

                taskCellPosition.y -= (heightOfTask + spaceBetweenTasks);
            }
        }
    }

    private void UpdateDoneTasksListVisuals()
    {
        float heightOfTask = 70;
        float spaceBetweenTasks = 5;

        int doneTasksDisplayCount = 0;
        foreach (Transform doneTask in doneTasksContentRectTransform.transform)
        {
            doneTasksDisplayCount++;
        }

        if (tasksDone.Count > doneTasksDisplayCount)
        {
            lock (tasksDone)
            {
                doneTasksContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightOfTask * tasksDone.Count + spaceBetweenTasks * (tasksDone.Count + 1));

                List<TaskInfo> tasksDoneToAdd = new List<TaskInfo>();

                foreach (TaskInfo doneTaskInfo in tasksDone)
                {
                    bool taskAlreadyDisplayed = false;
                    foreach (Transform displayedTask in doneTasksContentRectTransform.transform)
                    {
                        if (displayedTask.GetComponent<DoneTaskBehaviour>().taskInfo.Equals(doneTaskInfo))
                        {
                            taskAlreadyDisplayed = true;
                            break;
                        }
                    }
                    if (!taskAlreadyDisplayed)
                    {
                        tasksDoneToAdd.Add(doneTaskInfo);
                    }
                }

                Vector2 taskCellPosition = new Vector2(0, -spaceBetweenTasks);
                foreach (Transform displayedTask in doneTasksContentRectTransform.transform)
                {
                    taskCellPosition.y -= (heightOfTask + spaceBetweenTasks);
                }

                foreach (TaskInfo doneTaskInfo in tasksDoneToAdd)
                {
                    GameObject taskCellObj = Instantiate(DoneTaskPrefab, doneTasksContentRectTransform.transform);
                    taskCellObj.GetComponent<DoneTaskBehaviour>().InitializeInfo(doneTaskInfo);
                    taskCellObj.GetComponent<RectTransform>().anchoredPosition = taskCellPosition;

                    taskCellPosition.y -= (heightOfTask + spaceBetweenTasks);
                }
            }
        }
    }

    public void RemoveTaskFromToDoList(TaskInfo taskInfo, bool eraseFromPool)
    {
        lock (tasksToDo)
        {
            tasksToDo.Remove(taskInfo);
        }
        if (eraseFromPool && !taskInfo.critical)
        {
            mustHaveTasks.Remove(taskInfo);
            polishTasks.Remove(taskInfo);
            debugTasks.Remove(taskInfo);
            lastMinuteArtTweaksTasks.Remove(taskInfo);
            lastMinuteSoundTweaksTasks.Remove(taskInfo);
            balancingTasks.Remove(taskInfo);
        }
        UpdateToDoListVisuals();
    }

    public void RemoveTaskFromInProgressList(TaskInfo taskInfo)
    {
        MethodBase methodBase = MethodBase.GetCurrentMethod();
        Debug.Log(methodBase.Name);

        lock (tasksInProgress)
        {
            if (activeTask.taskInfo.Equals(taskInfo))
            {
                activeTask = null;
            }
            tasksInProgress.Remove(taskInfo);
        }
        UpdateInProgressListVisuals();
    }

    public void PlaySacrificeSound()
    {
        MethodBase methodBase = MethodBase.GetCurrentMethod();
        Debug.Log(methodBase.Name);

        sacrificeAudio.Stop();
        sacrificeAudio.Play();
    }
}
