using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDevStatusBehaviour : MonoBehaviour
{
    public static GameDevStatusBehaviour instance;

    [Header("References in scene")]
    public Slider HPSlider;
    public Image HPSliderFill;
    public Image HPSliderBackground;
    public Text StatusText;
    public Text BonusText;

    [Header("Visuals")]
    public Color MotivatedColor;
    public Color OkayColor;
    public Color TiredColor;
    public Color ExhaustedColor;

    [Header("Status")]
    public float HPMax;
    public float currentHP;
    public float hpLossByMinutes;
    public float HPRatioForTiredStatus;
    public float HPRatioForExhaustedStatus;
    public bool motivationBonus;
    public float motivationBonusDelay;
    public float consecutiveMinutesOfSleep;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        currentHP = HPMax;
        consecutiveMinutesOfSleep = 0;
        UpdateVisuals();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GiveMotivationBoost(float minutesOfMotivation, int damage)
    {
        motivationBonus = true;
        motivationBonusDelay += minutesOfMotivation;
        currentHP -= damage;
        currentHP = (currentHP < 0) ? 0 : currentHP;
        UpdateVisuals();
    }

    public void Sleep(float minutesPassed)
    {
        consecutiveMinutesOfSleep += minutesPassed;

        currentHP += minutesPassed * hpLossByMinutes * Mathf.CeilToInt(consecutiveMinutesOfSleep / 30);
        if (currentHP >= HPMax)
        {
            currentHP = HPMax;
            motivationBonus = true;
            motivationBonusDelay += minutesPassed;
        }
        UpdateVisuals();
    }

    public int GetEfficiencyBonus()
    {
        int bonus = 0;
        if (motivationBonus)
        {
            bonus = 50;
        }
        else
        {
            float hpRatio = currentHP * 1.0f / HPMax;
            if (hpRatio < HPRatioForExhaustedStatus)
            {
                bonus = -50;
            }
            else if (hpRatio < HPRatioForTiredStatus)
            {
                bonus = -25;
            }
        }
        return bonus;
    }

    private void UpdateVisuals()
    {
        try
        {
            if (motivationBonus)
            {
                StatusText.text = "MOTIVATED";
                StatusText.color = MotivatedColor;
                BonusText.text = "Speed +" + GetEfficiencyBonus().ToString() + "%";
                BonusText.color = MotivatedColor;
                HPSlider.value = 1;
                HPSliderFill.color = MotivatedColor;
                HPSliderBackground.color = MotivatedColor;
            }
            else
            {
                float hpRatio = currentHP * 1.0f / HPMax;
                if (hpRatio < HPRatioForExhaustedStatus)
                {
                    StatusText.text = "EXHAUSTED";
                    StatusText.color = ExhaustedColor;
                    BonusText.text = "Speed " + GetEfficiencyBonus().ToString() + "%";
                    BonusText.color = ExhaustedColor;
                    HPSliderFill.color = ExhaustedColor;
                    HPSliderBackground.color = Color.black;
                    HPSlider.value = (hpRatio / HPRatioForExhaustedStatus);
                }
                else if (hpRatio < HPRatioForTiredStatus)
                {
                    StatusText.text = "TIRED";
                    StatusText.color = TiredColor;
                    BonusText.text = "Speed " + GetEfficiencyBonus().ToString() + "%";
                    BonusText.color = TiredColor;
                    HPSliderFill.color = TiredColor;
                    HPSliderBackground.color = ExhaustedColor;
                    HPSlider.value = (hpRatio - HPRatioForExhaustedStatus) / (HPRatioForTiredStatus - HPRatioForExhaustedStatus);
                }
                else
                {
                    StatusText.text = "OKAY";
                    StatusText.color = OkayColor;
                    BonusText.text = "";
                    BonusText.color = OkayColor;
                    HPSliderFill.color = OkayColor;
                    HPSliderBackground.color = TiredColor;
                    HPSlider.value = (hpRatio - HPRatioForTiredStatus) / (1 - HPRatioForTiredStatus);
                }
            }
        }
        catch (System.Exception)
        {
        }
    }

    public void TimePass(float minutesPassed)
    {
        consecutiveMinutesOfSleep = 0;

        if (motivationBonus)
        {
            motivationBonusDelay -= minutesPassed;
            if (motivationBonusDelay <= 0)
            {
                motivationBonus = false;
            }
        }

        currentHP -= hpLossByMinutes * minutesPassed;
        currentHP = (currentHP < 0) ? 0 : currentHP;
        UpdateVisuals();
    }

    public Color GetColorFromDevStatus()
    {
        if (motivationBonus)
        {
            return MotivatedColor;
        }
        else
        {
            float hpRatio = currentHP * 1.0f / HPMax;
            if (hpRatio < HPRatioForExhaustedStatus)
            {
                return ExhaustedColor;
            }
            else if (hpRatio < HPRatioForTiredStatus)
            {
                return TiredColor;
            }
            else
            {
                return OkayColor;
            }
        }
    }
}
