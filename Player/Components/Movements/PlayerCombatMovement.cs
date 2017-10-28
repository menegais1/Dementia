using UnityEngine;

public class PlayerCombatMovement : BasicPhysicsMovement
{
    public CombatMovementState CombatMovementState { get; private set; }
    public CombatPressMovementState CombatPressMovementState { get; private set; }

    private PlayerController playerController;
    private BasicCollisionHandler playerCollisionHandler;
    private PlayerStatusVariables playerStatusVariables;

    private Weapon currentWeapon;
    private float cqcDistance;

    public PlayerCombatMovement(MonoBehaviour monoBehaviour,
        BasicCollisionHandler playerCollisionHandler,
        PlayerController playerController, PlayerStatusVariables playerStatusVariables, Weapon currentWeapon,
        float cqcDistance) : base(
        monoBehaviour)
    {
        this.playerStatusVariables = playerStatusVariables;
        this.monoBehaviour = monoBehaviour;
        this.playerController = playerController;
        this.playerCollisionHandler = playerCollisionHandler;
        this.currentWeapon = currentWeapon;
        this.cqcDistance = cqcDistance;
    }


    public override void StartMovement()
    {
        playerController.CheckForCombatInput(currentWeapon.Automatic);

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

    public void UseItem()
    {
    }

    public void TakeDamage()
    {
    }

    public void Shoot()
    {
        currentWeapon.Shoot(playerController.AimDirection);
    }

    public void ReloadWeapon()
    {
        currentWeapon.Reload();
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