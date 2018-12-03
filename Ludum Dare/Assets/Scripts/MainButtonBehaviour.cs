using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LeftMenuButton
{
    DOSTUFF,
    COFFEE,
    SLEEP
}

public class MainButtonBehaviour : MonoBehaviour
{
    public LeftMenuButton buttonAction;

    public Animator buttonAnimator;
    public Image bkgImage;
    public Image iconImage;

    public Color inactiveColor;
    public Color activeColor;

    public bool actionActive;

    [Header("Specifics for coffee")]
    public Sprite coffeeMachineEmptySprite;
    public Sprite coffeeMachine1cupSprite;
    public Sprite coffeeMachine2cupsSprite;
    public Sprite coffeeMachine3cupsSprite;
    public int availableCoffees;

    // Use this for initialization
    void Start ()
    {
        UpdateVisuals();
    }

    public void ButtonPressed()
    {
        if (!actionActive && !(buttonAction == LeftMenuButton.COFFEE && availableCoffees <= 0) )
        {
            GameEngineBehaviour.instance.ResetButtons();
            actionActive = true;
            bkgImage.color = actionActive ? activeColor : inactiveColor;
            buttonAnimator.SetBool("Active", actionActive);

            if (actionActive)
            {
                switch (buttonAction)
                {
                    case LeftMenuButton.DOSTUFF:
                        GameEngineBehaviour.instance.BackToWork();
                        break;
                    case LeftMenuButton.COFFEE:
                        availableCoffees--;
                        GameEngineBehaviour.instance.PrepareCoffee();
                        break;
                    case LeftMenuButton.SLEEP:
                        GameEngineBehaviour.instance.GoToSleep();
                        break;
                }
            }
        }
    }

    public void RefillCoffee()
    {
        availableCoffees = 3;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        bkgImage.color = actionActive ? activeColor : inactiveColor;
        buttonAnimator.SetBool("Active", actionActive);

        if (buttonAction == LeftMenuButton.COFFEE)
        {
            if (availableCoffees <= 0)
            {
                iconImage.sprite = coffeeMachineEmptySprite;
            }
            else if (availableCoffees <= 1)
            {
                iconImage.sprite = coffeeMachine1cupSprite;
            }
            else if(availableCoffees <= 2)
            {
                iconImage.sprite = coffeeMachine2cupsSprite;
            }
            else
            {
                iconImage.sprite = coffeeMachine3cupsSprite;
            }
        }
    }

    public void ResetButton()
    {
        actionActive = false;
        UpdateVisuals();
    }
}
