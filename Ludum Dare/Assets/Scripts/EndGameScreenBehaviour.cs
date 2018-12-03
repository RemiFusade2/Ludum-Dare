using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreenBehaviour : MonoBehaviour
{
    public Animator EndGameScreenAnimator;
    public RectTransform endGameScreenDoneTasksContentRectTransform;
    public GameObject EndGameDoneTaskPrefab;
    private List<TaskInfo> tasksDone;

    public Text youhavemadeText;
    public Text resultText;
    public Text ratingsText;
    public GameObject ratingsPanel;

    public Slider overallSlider;
    public Slider moodSlider;
    public Slider themeSlider;
    public Slider funSlider;
    public Slider graphicsSlider;
    public Slider humorSlider;

    public Button tryAgainButton;

    private bool resultIsAGame;
    
    void Start()
    {
        resultIsAGame = false;
    }

    public void PressTryAgainButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("game");
    }

    private void DisplayDoneTasksOnEndGameScreen()
    {
        float heightOfTask = 40;
        float spaceBetweenTasks = 5;

        endGameScreenDoneTasksContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightOfTask * tasksDone.Count + spaceBetweenTasks * (tasksDone.Count + 1));

        Vector2 taskCellPosition = new Vector2(0, -spaceBetweenTasks);
        foreach (TaskInfo doneTaskInfo in tasksDone)
        {
            if (!string.IsNullOrEmpty(doneTaskInfo.resultText))
            {
                GameObject taskCellObj = Instantiate(EndGameDoneTaskPrefab, endGameScreenDoneTasksContentRectTransform.transform);
                taskCellObj.GetComponent<EndGameDoneTaskBehaviour>().InitializeInfo(doneTaskInfo);
                taskCellObj.GetComponent<RectTransform>().anchoredPosition = taskCellPosition;

                taskCellPosition.y -= (heightOfTask + spaceBetweenTasks);
            }
        }
    }

    private int GetStarValueFromCount(int count)
    {
        int starValue = 1;
        if (count <= 0)
        {
            starValue = 1;
        }
        else if (count <= 1)
        {
            starValue = 3;
        }
        else if (count <= 2)
        {
            starValue = 4;
        }
        else if (count <= 3)
        {
            starValue = 5;
        }
        else if (count <= 4)
        {
            starValue = 6;
        }
        else if (count <= 5)
        {
            starValue = 7;
        }
        else if (count <= 7)
        {
            starValue = 8;
        }
        else if (count <= 9)
        {
            starValue = 9;
        }
        else
        {
            starValue = 10;
        }
        Debug.Log("count = " + count);
        Debug.Log("starValue = " + starValue);
        return starValue;
    }

    private void ComputeResults()
    {
        int countOfArtTasks = 0;
        int countOfCodeTasks = 0;
        int countOfGDTasks = 0;
        int countOfSDTasks = 0;
        int countOfCriticalTasks = 0;
        int countOfUselessTasks = 0;
        foreach (TaskInfo task in tasksDone)
        {
            if (task.type == TaskSkillType.ART)
            {
                countOfArtTasks++;
            }
            else if (task.type == TaskSkillType.CODE)
            {
                countOfCodeTasks++;
            }
            else if(task.type == TaskSkillType.GAME_DESIGN)
            {
                countOfGDTasks++;
            }
            else if(task.type == TaskSkillType.SOUND)
            {
                countOfSDTasks++;
            }
            else if (task.type == TaskSkillType.USELESS)
            {
                countOfUselessTasks++;
            }
            if (task.critical)
            {
                countOfCriticalTasks++;
            }
        }

        if ((countOfArtTasks+countOfCodeTasks + countOfGDTasks+ countOfSDTasks) >= 2)
        {
            resultIsAGame = true;
            resultText.text = "A GAME!";
        }
        else
        {
            resultIsAGame = false;
            resultText.text = "Huh... not much...";
        }

        Debug.Log("countOfSDTasks = " + countOfSDTasks);
        Debug.Log("countOfGDTasks = " + countOfGDTasks);
        Debug.Log("countOfArtTasks = " + countOfArtTasks);
        Debug.Log("countOfCodeTasks = " + countOfCodeTasks);
        Debug.Log("countOfUselessTasks = " + countOfUselessTasks);

        moodSlider.value = GetStarValueFromCount (countOfSDTasks + countOfArtTasks) ; // sound + Art
        themeSlider.value = GetStarValueFromCount(countOfGDTasks + countOfUselessTasks); // Useless
        funSlider.value = GetStarValueFromCount(countOfGDTasks + countOfCodeTasks); // GD + code
        graphicsSlider.value = GetStarValueFromCount(countOfArtTasks * 2); // Art
        humorSlider.value = GetStarValueFromCount(countOfUselessTasks + countOfGDTasks + countOfCriticalTasks); // Useless + Critical + GD
        overallSlider.value = Mathf.CeilToInt((moodSlider.value + themeSlider.value + funSlider.value + graphicsSlider.value + humorSlider.value)/5.0f); // mean of above
    }

    public void DisplayEndGameScreen(List<TaskInfo> listOfDoneTasks)
    {
        tasksDone = listOfDoneTasks;
        ComputeResults();
        DisplayDoneTasksOnEndGameScreen();
        EndGameScreenAnimator.SetTrigger("Appear");
        StartCoroutine(WaitAndDisplayAnnounce(2.0f));
    }

    private IEnumerator WaitAndDisplayAnnounce(float delay)
    {
        yield return new WaitForSeconds(delay);
        youhavemadeText.gameObject.SetActive(true);
        StartCoroutine(WaitAndDisplayResult(2.0f));
    }
    private IEnumerator WaitAndDisplayResult(float delay)
    {
        yield return new WaitForSeconds(delay);
        resultText.gameObject.SetActive(true);
        if (resultIsAGame)
        {
            StartCoroutine(WaitAndDisplayRatings(2.0f));
        }
        else
        {
            StartCoroutine(WaitAndDisplayTryAgain(2.0f));
        }
    }
    private IEnumerator WaitAndDisplayRatings(float delay)
    {
        yield return new WaitForSeconds(delay);
        ratingsText.gameObject.SetActive(true);
        StartCoroutine(WaitAndDisplayRatingsPanel(2.0f));
    }
    private IEnumerator WaitAndDisplayRatingsPanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        ratingsPanel.gameObject.SetActive(true);
        StartCoroutine(WaitAndDisplayTryAgain(2.0f));
    }
    private IEnumerator WaitAndDisplayTryAgain(float delay)
    {
        yield return new WaitForSeconds(delay);
        tryAgainButton.gameObject.SetActive(true);
    }
}
