using System;
using UnityEngine;

public class PlayerMiscellaneousMovement : BasicPhysicsMovement
{
    public MiscellaneousPressMovementState miscellaneousPressMovementState;

    private static PlayerMiscellaneousMovement instance;

    public delegate void SceneryInteractionDelegate();

    private event SceneryInteractionDelegate sceneryInteractionEvent;

    public delegate void ItemEffectDelegate();

    private event ItemEffectDelegate itemEffectEvent;


    private float cameraZoomSize;

    private BasicCollisionHandler basicCollisionHandler;
    private PlayerController playerController;
    private PlayerStatusVariables playerStatusVariables;


    public static PlayerMiscellaneousMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerMiscellaneousMovement();
        }

        return instance;
    }

    private PlayerMiscellaneousMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour, float cameraZoomSize,
        BasicCollisionHandler basicCollisionHandler,
        PlayerController playerController
    )
    {
        FillInstance(monoBehaviour);
        this.playerController = playerController;
        this.basicCollisionHandler = basicCollisionHandler;
        this.playerStatusVariables = PlayerStatusVariables.GetInstance();
        this.cameraZoomSize = cameraZoomSize;
    }

    public override void StartMovement()
    {
        playerController.CheckForMiscellaneousInput();

        if (playerStatusVariables.canTakeItem && playerController.TakeItemPress)
        {
            playerStatusVariables.isTakingItem = true;
        }

        if (playerStatusVariables.canInteractWithScenery && playerController.InteractWithSceneryPress)
        {
            playerStatusVariables.isInteractingWithScenery = true;
        }

        if (playerController.ZoomCameraPress)
        {
            miscellaneousPressMovementState = MiscellaneousPressMovementState.ZoomCamera;
            playerStatusVariables.isCameraZoomed = !playerStatusVariables.isCameraZoomed;
        }
        else if (playerStatusVariables.isTakingItem)
        {
            miscellaneousPressMovementState = MiscellaneousPressMovementState.TakeItem;
        }
        else if (playerStatusVariables.isInteractingWithScenery)
        {
            miscellaneousPressMovementState = MiscellaneousPressMovementState.InteractWithScenery;
        }
    }

    public override void PressMovementHandler()
    {
        switch (miscellaneousPressMovementState)
        {
            case MiscellaneousPressMovementState.None:
                break;
            case MiscellaneousPressMovementState.TakeItem:
                TakeItem();
                break;
            case MiscellaneousPressMovementState.InteractWithScenery:
                InteractWithScenery();
                break;
            case MiscellaneousPressMovementState.ZoomCamera:
                ZoomCamera(playerStatusVariables.isCameraZoomed);
                break;
            default:
                Debug.Log("Error");
                break;
        }
        miscellaneousPressMovementState = MiscellaneousPressMovementState.None;
    }

    public override void HoldMovementHandler()
    {
    }

    public override void ResolvePendencies()
    {
    }

    public void ReadNotes()
    {
    }

    private void InteractWithScenery()
    {
        if (sceneryInteractionEvent == null) return;
        sceneryInteractionEvent();
        playerStatusVariables.isInteractingWithScenery = false;
        // Debug.Log("teste");
    }

    public void ZoomCamera(bool zoomOut)
    {
        CoroutineManager.AddCoroutine(CameraController.ZoomCameraCoroutine(cameraZoomSize, 100, zoomOut),
            "ZoomCameraCoroutine");
    }

    public void SubscribeInteractiveScenery(SceneryInteractionDelegate sceneryInteraction)
    {
        this.sceneryInteractionEvent = null;
        this.sceneryInteractionEvent += sceneryInteraction;
    }

    public void SubscribeItemEffect(ItemEffectDelegate itemEffect)
    {
        this.itemEffectEvent = null;
        this.itemEffectEvent += itemEffect;
    }

    private void TakeItem()
    {
        if (itemEffectEvent == null) return;
        itemEffectEvent();
        playerStatusVariables.isTakingItem = false;
    }
}