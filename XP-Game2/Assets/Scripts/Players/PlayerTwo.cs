using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.EventSystems;
//using UnityEngine.Scripting.APIUpdating;

public class PlayerTwo : MonoBehaviour, IKitchenObjectParent
{

    //Static pertence a classe
    public static PlayerTwo Instance { get; private set; }

    public event EventHandler OnFirstInteractAcaiFruit;

    public event EventHandler OnPickedSomething;
    public event EventHandler OnDash_2;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs: EventArgs
    {
        public BaseCounter selectedCounter;
    }

    private enum PlayerState
    {
        Normal,
        Dashing
    }
    private PlayerState playerState;
    

    [SerializeField] private float moveSpeed;
    [SerializeField] private float initialMoveSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private float dashTimer;
    [SerializeField] private float dashTimerMax = 0.1f;
    private float dashCooldown;
    [SerializeField] private float dashCooldownMax = 0.5f;
    private bool canDash = true;

    private void Awake() 
    {
        if(Instance != null)
        {
            UnityEngine.Debug.Log("There is more than one Player instance");
        }
        Instance = this;    

        initialMoveSpeed = moveSpeed;
    }


    private void Start() 
    {
        gameInput.OnInteractAction_2 += GameInput_OnInteractAction_2;
        gameInput.OnInteractAlternateAction_2 += GameInput_OnInteractAlternateAction_2;
        
        gameInput.OnDashAction_2 += GameInput_OnDashAction_2;
    }

    private void GameInput_OnInteractAlternateAction_2(object sender, System.EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        if(selectedCounter != null)
        {
            selectedCounter.InteractAlternate_2(this);
        }
    }

    private void GameInput_OnInteractAction_2(object sender, System.EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;
        
        if(selectedCounter != null)
        {
            selectedCounter.Interact_2(this);
        }

        if(!KitchenGameManager.Instance.GetWasPlayerFirstInteractedAcaiFruit())
        {
            if(GetKitchenObject().GetKitchenObjectSO() == KitchenGameManager.Instance.GetAcaiFruitKitchenObjectSO())
            {
                KitchenGameManager.Instance.SetWasPlayerFirstInteractedAcaiFruit(true);
                OnFirstInteractAcaiFruit?.Invoke(this, EventArgs.Empty);
            }
        } 
    }
    private void GameInput_OnDashAction_2(object sender, System.EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        if(canDash)
        {
            playerState = PlayerState.Dashing;
            OnDash_2?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
        Dashing();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void Dashing()
    {
        switch(playerState)
        {
            case PlayerState.Normal:
                if(!canDash)
                {
                    dashCooldown += Time.deltaTime;
                }
                if(dashCooldown >= dashCooldownMax)
                {
                    dashCooldown = 0f;
                    canDash = true;
                }
                break;
            case PlayerState.Dashing:    
                canDash = false;
                dashTimer += Time.deltaTime;
                moveSpeed = dashForce;
                if(dashTimer >= dashTimerMax)
                {
                    playerState = PlayerState.Normal;
                    moveSpeed = initialMoveSpeed;
                    dashTimer = 0f;
                }
               break;
        }
    }

    private void HandleInteraction()
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        Vector2 inputVector = gameInput.GetMovementVectorNormalizedPlayerTwo();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Has ClearCounter
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        Vector2 inputVector = gameInput.GetMovementVectorNormalizedPlayerTwo();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        
        if (!canMove)
        {
            //Cannot move towards moveDir
            
            //Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                //Cannot move only on the X

                //Attemp only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove =  (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    //Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    //Cannot move in any direction
                }
            }
        }

        if(canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjetFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        //Anim Grab Item

        if(kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
