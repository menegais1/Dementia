using System;
using UnityEngine;

public class PlayerMiscellaneousMovement : BasicPhysicsMovement
{
    public MiscellaneousPressMovementState MiscellaneousPressMovementState { get; private set; }

    public delegate void SceneryInteractionDelegate();

    private event SceneryInteractionDelegate sceneryInteractionEvent;

    public delegate void ItemEffectDelegate();

    private event ItemEffectDelegate itemEffectEvent;


    private float cameraZoomSize;

    private BasicCollisionHandler playerCollisionHandler;
    private PlayerController playerController;
    private PlayerStatusVariables playerStatusVariables;


    public PlayerMiscellaneousMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour, float cameraZoomSize,
        BasicCollisionHandler playerCollisionHandler,
        PlayerController playerController, PlayerStatusVariables playerStatusVariables
    )
    {
        FillInstance(monoBehaviour);
        this.playerController = playerController;
        this.playerCollisionHandler = playerCollisionHandler;
        this.playerStatusVariables = playerStatusVariables;
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
            MiscellaneousPressMovementState = MiscellaneousPressMovementState.ZoomCamera;
            playerStatusVariables.isCameraZoomed = !playerStatusVariables.isCameraZoomed;
        }
        else if (playerStatusVariables.isTakingItem)
        {
            MiscellaneousPressMovementState = MiscellaneousPressMovementState.TakeItem;
        }
        else if (playerStatusVariables.isInteractingWithScenery)
        {
            MiscellaneousPressMovementState = MiscellaneousPressMovementState.InteractWithScenery;
        }
    }

    public override void PressMovementHandler()
    {
        switch (MiscellaneousPressMovementState)
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
        MiscellaneousPressMovementState = MiscellaneousPressMovementState.None;
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