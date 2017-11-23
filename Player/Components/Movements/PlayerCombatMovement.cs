using System;
using UnityEngine;

public class PlayerCombatMovement : BasicPhysicsMovement
{
    public CombatMovementState CombatMovementState { get; private set; }
    public CombatPressMovementState CombatPressMovementState { get; private set; }

    private PlayerController playerController;
    private BasicCollisionHandler playerCollisionHandler;
    private PlayerStatusVariables playerStatusVariables;
    private PlayerStatusController playerStatusController;
    private Inventory inventory;

    private Weapon currentWeapon;
    private ItemSlot currentItem;

    private float cqcDistance;

    public PlayerCombatMovement(MonoBehaviour monoBehaviour,
        BasicCollisionHandler playerCollisionHandler,
        PlayerController playerController, PlayerStatusVariables playerStatusVariables,
        PlayerStatusController playerStatusController,
        Inventory inventory,
        float cqcDistance) : base(
        monoBehaviour)
    {
        this.playerStatusVariables = playerStatusVariables;
        this.playerStatusController = playerStatusController;
        this.monoBehaviour = monoBehaviour;
        this.playerController = playerController;
        this.playerCollisionHandler = playerCollisionHandler;
        this.inventory = inventory;
        this.cqcDistance = cqcDistance;
    }


    public override void StartMovement()
    {
        CheckForEquipedWeapon();
        CheckForEquipedItem();

        playerController.CheckForCombatInput(
            currentWeapon != null && currentWeapon.Automatic);

        playerStatusVariables.canAim = playerStatusVariables.CheckCanAim();

        if (playerController.AimHold && playerStatusVariables.canAim)
        {
            playerStatusVariables.isAiming = true;
        }
        else
        {
            playerStatusVariables.isAiming = false;
        }


        if (playerStatusVariables.isAiming)
        {
            CheckFacingDirection();
            CombatMovementState = CombatMovementState.Aiming;

            if (playerController.ShootPress)
            {
                CombatPressMovementState = CombatPressMovementState.Shoot;
            }
            else if (playerController.UseItemPress)
            {
                CombatPressMovementState = CombatPressMovementState.UseItem;
            }
            else if (playerController.ReloadPress)
            {
                CombatPressMovementState = CombatPressMovementState.Reload;
            }
            else if (playerController.CqcPress)
            {
                CombatPressMovementState = CombatPressMovementState.Cqc;
            }
        }
        else if (playerController.CqcPress)
        {
            CombatPressMovementState = CombatPressMovementState.Cqc;
        }
        else
        {
            CombatMovementState = CombatMovementState.None;
        }
    }

    public override void PressMovementHandler()
    {
        switch (CombatPressMovementState)
        {
            case CombatPressMovementState.Shoot:
                Shoot();
                break;
            case CombatPressMovementState.Reload:
                ReloadWeapon();
                break;
            case CombatPressMovementState.Cqc:
                Cqc();
                break;
            case CombatPressMovementState.UseItem:
                UseItem();
                break;
            case CombatPressMovementState.None:
                break;
            default:
                Debug.Log("Error");
                break;
        }
        CombatPressMovementState = CombatPressMovementState.None;
    }

    public override void HoldMovementHandler()
    {
        switch (CombatMovementState)
        {
            case CombatMovementState.Aiming:
                break;
            case CombatMovementState.None:
                break;
            default:
                Debug.Log("ERRO");
                break;
        }
    }

    public override void ResolvePendencies()
    {
    }

    private void CheckForEquipedWeapon()
    {
        if (inventory.CurrentWeapon != null && currentWeapon != null &&
            currentWeapon.WeaponTypeId != inventory.CurrentWeapon.WeaponTypeId)
        {
            GameObject.Destroy(currentWeapon.gameObject);
            currentWeapon = null;
        }

        if (inventory.CurrentWeapon != null && currentWeapon == null)
        {
            currentWeapon = GameObject.Instantiate(inventory.CurrentWeapon.WeaponInstance,
                monoBehaviour.gameObject.transform).GetComponent<Weapon>();
            currentWeapon.CurrentMagazine = inventory.CurrentWeapon.Magazine;
            currentWeapon.CurrentAmmo = inventory.CurrentWeapon.Ammo;
        }
        else if (inventory.CurrentWeapon == null && currentWeapon != null)
        {
            GameObject.Destroy(currentWeapon.gameObject);
            currentWeapon = null;
        }
    }

    private void CheckForEquipedItem()
    {
        if ((inventory.CurrentItem != null && currentItem != null &&
             currentItem.Type != inventory.CurrentItem.Type) || (inventory.CurrentItem == null && currentItem != null))
        {
            currentItem = null;
        }

        if (inventory.CurrentItem != null && currentItem == null)
        {
            currentItem = inventory.CurrentItem;
        }
    }

    private void UseItem()
    {
        if (currentItem == null) return;

        Item item = null;

        bool itemUsed = false;
        switch (currentItem.Type)
        {
            case ItemType.Bandages:

                item = InstantiateItem();

                if (item == null) return;
                
                if (!playerStatusController.LifeIsFull())
                {
                    item.GetComponent<HealingItem>().Effect(playerStatusController);
                    itemUsed = true;
                }
                break;
            case ItemType.Molotov:
                break;
            case ItemType.Analgesics:
                break;
            default:
                Debug.Log("Error");
                break;
        }

        if (itemUsed)
            currentItem.UseItem();
        else if (item != null)
            GameObject.Destroy(item.gameObject);
    }

    public Item InstantiateItem()
    {
        return GameObject.Instantiate(currentItem.ItemInstance, monoBehaviour.gameObject.transform)
            .GetComponent<Item>();
    }

    public void TakeDamage()
    {
    }

    public void Shoot()
    {
        if (currentWeapon == null) return;
        currentWeapon.Shoot(playerController.AimDirection);
        if (inventory.CurrentWeapon != null)
        {
            inventory.CurrentWeapon.Ammo = currentWeapon.CurrentAmmo;
            inventory.CurrentWeapon.Magazine = currentWeapon.CurrentMagazine;
        }
    }

    public void ReloadWeapon()
    {
        if (currentWeapon == null) return;
        currentWeapon.Reload();
        if (inventory.CurrentWeapon != null)
        {
            inventory.CurrentWeapon.Ammo = currentWeapon.CurrentAmmo;
            inventory.CurrentWeapon.Magazine = currentWeapon.CurrentMagazine;
        }
    }

    public void Cqc()
    {
        RaycastHit2D ray = Physics2D.Raycast(
            monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
            playerStatusVariables.facingDirection == FacingDirection.Right ? Vector2.right : Vector2.right * -1,
            cqcDistance);

        Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
            playerStatusVariables.facingDirection == FacingDirection.Right ? Vector2.right : Vector2.right * -1,
            Color.black);
    }

    private void CheckFacingDirection()
    {
        if (playerController.AimDirection.x >= 0)
        {
            playerStatusVariables.facingDirection = FacingDirection.Right;
        }
        else if (playerController.AimDirection.x < 0)
        {
            playerStatusVariables.facingDirection = FacingDirection.Left;
        }
        spriteRenderer.flipX = (playerStatusVariables.facingDirection != FacingDirection.Right);
    }
}