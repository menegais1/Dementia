using UnityEngine;

public class PlayerMiscellaneousMovement : BasicPhysicsMovement
{
    public MiscellaneousPressMovementState MiscellaneousPressMovementState { get; private set; }

    public delegate void SceneryInteractionDelegate();

    private event SceneryInteractionDelegate sceneryInteractionEvent;

    public delegate void ItemEffectDelegate();

    private float cameraZoomSize;

    private BasicCollisionHandler playerCollisionHandler;
    private PlayerController playerController;
    private PlayerStatusVariables playerStatusVariables;
    private Inventory inventory;
    private InGameMenuController inGameMenuController;


    public PlayerMiscellaneousMovement(MonoBehaviour monoBehaviour, float cameraZoomSize,
        BasicCollisionHandler playerCollisionHandler,
        PlayerController playerController, PlayerStatusVariables playerStatusVariables, Inventory inventory,
        InGameMenuController inGameMenuController) : base(
        monoBehaviour)
    {
        this.playerController = playerController;
        this.playerCollisionHandler = playerCollisionHandler;
        this.playerStatusVariables = playerStatusVariables;
        this.inventory = inventory;
        this.inGameMenuController = inGameMenuController;
        this.cameraZoomSize = cameraZoomSize;
    }

    public override void StartMovement()
    {
        playerController.CheckForMiscellaneousInput();

        if (playerStatusVariables.isInGameMenuOpen)
        {
            playerController.RevokeControl(0.1f, true, ControlTypeToRevoke.CombatMovement, monoBehaviour);
        }
        else
        {
            if (inGameMenuController.gameObject.activeSelf)
            {
                OpenCloseInGameMenu();
            }
        }

        if (playerController.InGameMenuOpenClose)
        {
            playerStatusVariables.isInGameMenuOpen = !playerStatusVariables.isInGameMenuOpen;
            OpenCloseInGameMenu();
        }

        if (playerStatusVariables.canTakeItem && playerController.TakeItemPress)
        {
            playerStatusVariables.isTakingItem = true;
        }
        else if (!playerStatusVariables.canTakeItem && playerStatusVariables.isTakingItem)
        {
            playerStatusVariables.isTakingItem = false;
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
                inventory.TakeItem(TakeItem());
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

    private CollectibleItem TakeItem()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Collectible Item"),
        };

        var item = new Collider2D[1];
        capsuleCollider2D.GetContacts(contactFilter2D, item);
        var collectibleItem = item[0] != null ? item[0].GetComponent<CollectibleItem>() : null;

        if (collectibleItem != null)
        {
            collectibleItem.DestroyItem();
            return collectibleItem;
        }
        return null;
    }

    private void OpenCloseInGameMenu()
    {
        inGameMenuController.OpenCloseInGameMenu();
    }
}