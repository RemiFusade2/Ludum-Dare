using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkyColorForRemainingTime
{
    public int remainingMinutes;
    public Color color;
}

public class CameraBehaviour : MonoBehaviour
{
    public static CameraBehaviour instance;

    public List<SkyColorForRemainingTime> allSkyColorsList;

    public Color lastColor;
    public Color targetColor;

    public float timeOfLastColorChange;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        lastColor = Color.black;
        targetColor = Color.black;
        CheckNextColor();
        StartCoroutine(WaitAndCheckNextColor(2.0f));
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (lastColor != targetColor)
        {
            float ratio = (Time.time - timeOfLastColorChange) / 3.0f;
            ratio = (ratio <= 0) ? 0 : ((ratio >= 1) ? 1 : ratio);
            this.GetComponent<Camera>().backgroundColor = Color.Lerp(lastColor, targetColor, ratio);
            if (ratio >= 1)
            {
                lastColor = targetColor;
            }
        }
    }

    private IEnumerator WaitAndCheckNextColor(float delay)
    {
        yield return new WaitForSeconds(delay);

        CheckNextColor();

        StartCoroutine(WaitAndCheckNextColor(2.0f));
    }

    private void CheckNextColor()
    {
        foreach (SkyColorForRemainingTime skyColor in allSkyColorsList)
        {
            if (GameEngineBehaviour.instance.remainingMinutes > skyColor.remainingMinutes)
            {
                if (targetColor != skyColor.color)
                {
                    targetColor = skyColor.color;
                    timeOfLastColorChange = Time.time;
                }
                break;
            }
        }
    }

}
