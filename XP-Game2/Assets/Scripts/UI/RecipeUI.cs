using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeUI : MonoBehaviour
{
    public event EventHandler OnButtonClick;

    [SerializeField] private Transform recipeTutorialTransfom;
    [SerializeField] private Transform backgroundTransfom;
    [SerializeField] private bool isEnable;
    [SerializeField] private Transform pressTriangleTextTransform;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        GameInput.Instance.OnOpenRecipe += GameInput_OnOpenRecipe;
        GameInput.Instance.OnOpenRecipe_2 += GameInput_OnOpenRecipe_2;

    }

    private void GameInput_OnOpenRecipe(object sender, System.EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        isEnable =! isEnable;
        if(isEnable)
        {
            Show();
            OnButtonClick?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Hide();
            OnButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
    private void GameInput_OnOpenRecipe_2(object sender, System.EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        isEnable =! isEnable;
        if(isEnable)
        {
            Show();
            OnButtonClick?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Hide();
            OnButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if(KitchenGameManager.Instance.IsRecipeTutorialActive())
        {
            Show();
            pressTriangleTextTransform.gameObject.SetActive(true);
        }
        if(KitchenGameManager.Instance.IsSkinSelectionActive())
        {
            Hide();
            pressTriangleTextTransform.gameObject.SetActive(false);
            OnButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
    private void Show()
    {
        LeanTween.moveY(recipeTutorialTransfom.GetComponent<RectTransform>(), 80, 0.5f).setEaseOutBack();
        backgroundTransfom.gameObject.SetActive(true);
    }
    private void Hide()
    {
        LeanTween.moveY(recipeTutorialTransfom.GetComponent<RectTransform>(), -1000, 0.5f).setEaseOutBack();
        backgroundTransfom.gameObject.SetActive(false);
    }

}
