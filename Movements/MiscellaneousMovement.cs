using System;
using UnityEngine;

public class MiscellaneousMovement
{
    public MiscellaneousPressMovementState miscellaneousPressMovementState;

    private static MiscellaneousMovement instance;

    public delegate void SceneryInteractionDelegate();

    private event SceneryInteractionDelegate sceneryInteractionEvent;

    public delegate void ItemEffectDelegate();

    private event ItemEffectDelegate itemEffectEvent;

    /*private float characterHeight;
    private bool existNote;
    private bool canReadNote;
    private bool readingNote;
   */
    private float cameraZoomSize;


    private MonoBehaviour monoBehaviour;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    public static MiscellaneousMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new MiscellaneousMovement();
        }

        return instance;
    }

    private MiscellaneousMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour, float cameraZoomSize)
    {
        this.monoBehaviour = monoBehaviour;
        this.cameraZoomSize = cameraZoomSize;
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.boxCollider2D = monoBehaviour.GetComponent<BoxCollider2D>();
    }

    public void StartMiscellaneousMovement()
    {
        PlayerController.CheckForMiscellaneousPlayerInput();

        if (PlayerStatusVariables.canTakeItem && PlayerController.TakeItemPress)
        {
            PlayerStatusVariables.isTakingItem = true;
        }

        if (PlayerStatusVariables.canInteractWithScenery && PlayerController.InteractWithSceneryPress)
        {
            PlayerStatusVariables.isInteractingWithScenery = true;
        }

        if (PlayerController.ZoomCameraPress)
        {
            miscellaneousPressMovementState = MiscellaneousPressMovementState.ZoomCamera;
            PlayerStatusVariables.isCameraZoomed = !PlayerStatusVariables.isCameraZoomed;
        }
        else if (PlayerStatusVariables.isTakingItem)
        {
            miscellaneousPressMovementState = MiscellaneousPressMovementState.TakeItem;
        }
        else if (PlayerStatusVariables.isInteractingWithScenery)
        {
            miscellaneousPressMovementState = MiscellaneousPressMovementState.InteractWithScenery;
        }
    }

    public void PressMovementHandler()
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
                ZoomCamera(PlayerStatusVariables.isCameraZoomed);
                break;
            default:
                Debug.Log("Error");
                break;
        }
        miscellaneousPressMovementState = MiscellaneousPressMovementState.None;
    }

    public void ReadNotes()
    {
    }

    private void InteractWithScenery()
    {
        if (sceneryInteractionEvent == null) return;
        sceneryInteractionEvent();
        PlayerStatusVariables.isInteractingWithScenery = false;
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
        PlayerStatusVariables.isTakingItem = false;
    }
}