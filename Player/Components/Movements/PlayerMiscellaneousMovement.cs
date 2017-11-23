using UnityEngine;
using UnityEngine.UI;

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
    private Diary diary;
    private InGameMenuController inGameMenuController;


    public PlayerMiscellaneousMovement(MonoBehaviour monoBehaviour, float cameraZoomSize,
        BasicCollisionHandler playerCollisionHandler,
        PlayerController playerController, PlayerStatusVariables playerStatusVariables, Inventory inventory,
        Diary diary,
        InGameMenuController inGameMenuController) : base(
        monoBehaviour)
    {
        this.playerController = playerController;
        this.playerCollisionHandler = playerCollisionHandler;
        this.playerStatusVariables = playerStatusVariables;
        this.inventory = inventory;
        this.diary = diary;
        this.inGameMenuController = inGameMenuController;
        this.cameraZoomSize = cameraZoomSize;
    }

    public override void StartMovement()
    {
        playerController.CheckForMiscellaneousInput();

        if (playerStatusVariables.isInGameMenuOpen)
        {
            playerController.RevokeControl(true, ControlTypeToRevoke.CombatMovement);
        }
        else
        {
            if (inGameMenuController.gameObject.activeSelf)
            {
                OpenCloseInGameMenu();
            }
        }

        if (playerController.MenuPress)
        {
            playerStatusVariables.isInGameMenuOpen = !playerStatusVariables.isInGameMenuOpen;
            if (inGameMenuController.gameObject.activeSelf)
            {
                playerController.RevokeControl(false, ControlTypeToRevoke.CombatMovement);
            }
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

        if (playerStatusVariables.canTakeWeapon && playerController.TakeItemPress)
        {
            playerStatusVariables.isTakingWeapon = true;
        }
        else if (!playerStatusVariables.canTakeWeapon && playerStatusVariables.isTakingWeapon)
        {
            playerStatusVariables.isTakingWeapon = false;
        }

        if (playerStatusVariables.canTakeNote && playerController.TakeItemPress)
        {
            playerStatusVariables.isTakingNote = true;
        }
        else if (!playerStatusVariables.canTakeNote && playerStatusVariables.isTakingNote)
        {
            playerStatusVariables.isTakingNote = false;
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
        else if (playerStatusVariables.isTakingWeapon)
        {
            MiscellaneousPressMovementState = MiscellaneousPressMovementState.TakeWeapon;
        }
        else if (playerStatusVariables.isTakingNote)
        {
            MiscellaneousPressMovementState = MiscellaneousPressMovementState.TakeNote;
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
                
                playerController.RevokeControl(0.5f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
                playerController.RevokeControl(false, ControlTypeToRevoke.MiscellaneousMovement);
                break;
            case MiscellaneousPressMovementState.TakeWeapon:
                inventory.TakeWeapon(TakeWeapon());

                playerController.RevokeControl(0.5f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
                playerController.RevokeControl(false, ControlTypeToRevoke.MiscellaneousMovement);
                break;
            case MiscellaneousPressMovementState.TakeNote:
                diary.TakeNote(TakeNote());
                
                playerController.RevokeControl(0.5f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
                playerController.RevokeControl(false, ControlTypeToRevoke.MiscellaneousMovement);
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

    private CollectibleNote TakeNote()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Collectible Note"),
        };
        var note = new Collider2D[1];

        capsuleCollider2D.GetContacts(contactFilter2D, note);
        var collectibleNote = note[0] != null ? note[0].GetComponent<CollectibleNote>() : null;

        if (collectibleNote != null)
        {
            collectibleNote.DestroyItem();
            return collectibleNote;
        }
        return null;
    }

    private CollectibleWeapon TakeWeapon()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Collectible Weapon"),
        };

        var weapon = new Collider2D[1];
        capsuleCollider2D.GetContacts(contactFilter2D, weapon);
        var collectibleWeapon = weapon[0] != null ? weapon[0].GetComponent<CollectibleWeapon>() : null;

        if (collectibleWeapon != null)
        {
            collectibleWeapon.DestroyWeapon();
            return collectibleWeapon;
        }
        return null;
    }

    private void OpenCloseInGameMenu()
    {
        inGameMenuController.OpenCloseInGameMenu();
    }
}