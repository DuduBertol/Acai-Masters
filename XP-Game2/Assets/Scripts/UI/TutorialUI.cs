using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public event EventHandler OnButtonClick;

    [SerializeField] private Transform controlTutorialImageTransform; 

    [Header("Useless")]
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    // [SerializeField] private TextMeshProUGUI keyGamepadMoveText;
    // [SerializeField] private TextMeshProUGUI keyGamepadInteractText;
    // [SerializeField] private TextMeshProUGUI keyGamepadInteractAlternateText;
    // [SerializeField] private TextMeshProUGUI keyGamepadPauseText;

    private void Start() 
    {
        GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        
        UpdateVisual();
        
        LeanTween.moveY(controlTutorialImageTransform.GetComponent<RectTransform>(), 0, 0.5f).setEaseOutBack();
        Show();    
    }
    

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if(KitchenGameManager.Instance.IsRecipeTutorialActive())
        {
            Hide();
            OnButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }
    
    private void UpdateVisual() 
    {
        keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up_1);    
        keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down_1);    
        keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left_1);    
        keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right_1);    
        keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_1);    
        keyInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate_1);    
        keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause_1);  
        // keyGamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact_1);    
        // keyGamepadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate_1);    
        // keyGamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause_1);    
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
