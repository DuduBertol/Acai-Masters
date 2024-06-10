using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public event EventHandler OnButtonClick;

    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private TextMeshProUGUI recipesLostText;
    [SerializeField] private TextMeshProUGUI recipesTotalText;
    
    [SerializeField] private Button restartButton;

    private void Awake() 
    {
        restartButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
            OnButtonClick?.Invoke(this, EventArgs.Empty);
        });    
    }
    private void Start() 
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if(KitchenGameManager.Instance.IsGameOver())
        {
            Show();

            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();

            recipesLostText.text = "-" + DeliveryManager.Instance.GetFailedRecipesAmount().ToString();
            recipesTotalText.text = DeliveryManager.Instance.GetTotalRecipesAmount().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Update() 
    {

    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
