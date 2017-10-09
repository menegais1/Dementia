using System;
using UnityEngine;

public class PlayerCombatMovement : BasicPhysicsMovement
{
    public CombatMovementState CombatMovementState { get; private set; }
    public CombatPressMovementState CombatPressMovementState { get; private set; }


    public AimDirection AimDirection;

    private GameObject bulletEffect;

    private PlayerController playerController;
    private BasicCollisionHandler playerCollisionHandler;
    private PlayerStatusVariables playerStatusVariables;


    public PlayerCombatMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour, GameObject bulletEffect,
        BasicCollisionHandler playerCollisionHandler,
        PlayerController playerController, PlayerStatusVariables playerStatusVariables)
    {
        FillInstance(monoBehaviour);

        this.playerStatusVariables = playerStatusVariables;
        this.monoBehaviour = monoBehaviour;
        this.playerController = playerController;
        this.bulletEffect = bulletEffect;
        this.playerCollisionHandler = playerCollisionHandler;
    }

    public override void StartMovement()
    {
        playerController.CheckForCombatInput();

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
            AimDirection = CheckAimDirection();

            if (playerController.ShootPress)
            {
                CombatPressMovementState = CombatPressMovementState.Shoot;
            }
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
                Aim();
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

    public void Aim()
    {
        switch (AimDirection)
        {
            case AimDirection.Right:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.right);

                break;
            case AimDirection.Up:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.up);

                break;
            case AimDirection.Down:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.up * -1);

                break;
            case AimDirection.Left:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.right * -1);

                break;
            case AimDirection.UpRight:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    (monoBehaviour.transform.up + monoBehaviour.transform.right).normalized);

                break;
            case AimDirection.UpLeft:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    (monoBehaviour.transform.up + (monoBehaviour.transform.right * -1)).normalized);

                break;
            case AimDirection.DownRight:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    ((monoBehaviour.transform.up * -1) + monoBehaviour.transform.right).normalized);

                break;
            case AimDirection.DownLeft:
                Debug.DrawRay(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    ((monoBehaviour.transform.up * -1) + (monoBehaviour.transform.right * -1)).normalized);

                break;
            default:
                Debug.Log("Error");
                break;
        }
    }

    public void UseItem()
    {
    }

    public void TakeDamage()
    {
    }

    public void Die()
    {
    }

    public AimDirection CheckAimDirection()
    {
        if (!MathHelpers.Approximately(playerController.HorizontalAim, 0, float.Epsilon))
        {
            if (!MathHelpers.Approximately(playerController.VerticalAim, 0, float.Epsilon))
            {
                if (MathHelpers.Approximately(playerController.HorizontalAim, 1, float.Epsilon)
                    && MathHelpers.Approximately(playerController.VerticalAim, 1, float.Epsilon))
                {
                    return AimDirection.UpRight;
                }
                else if (MathHelpers.Approximately(playerController.HorizontalAim, 1, float.Epsilon)
                         && MathHelpers.Approximately(playerController.VerticalAim, -1, float.Epsilon))
                {
                    return AimDirection.DownRight;
                }
                else if (MathHelpers.Approximately(playerController.HorizontalAim, -1, float.Epsilon)
                         && MathHelpers.Approximately(playerController.VerticalAim, 1, float.Epsilon))
                {
                    return AimDirection.UpLeft;
                }
                else if (MathHelpers.Approximately(playerController.HorizontalAim, -1, float.Epsilon)
                         && MathHelpers.Approximately(playerController.VerticalAim, -1, float.Epsilon))
                {
                    return AimDirection.DownLeft;
                }
            }
            else
            {
                if (MathHelpers.Approximately(playerController.HorizontalAim, -1, float.Epsilon))
                {
                    return AimDirection.Left;
                }
                else
                {
                    return AimDirection.Right;
                }
            }
        }
        else if (MathHelpers.Approximately(playerController.VerticalAim, 1, float.Epsilon))
        {
            return AimDirection.Up;
        }
        else if (MathHelpers.Approximately(playerController.VerticalAim, -1, float.Epsilon))
        {
            return AimDirection.Down;
        }

        return playerStatusVariables.facingDirection == FacingDirection.Right
            ? AimDirection.Right
            : AimDirection.Left;
    }

    public void Shoot()
    {
        RaycastHit2D ray = new RaycastHit2D();
        Debug.Log(AimDirection);

        switch (AimDirection)
        {
            case AimDirection.Right:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.right);

                break;
            case AimDirection.Up:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.up);

                break;
            case AimDirection.Down:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.up * -1);

                break;
            case AimDirection.Left:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    monoBehaviour.transform.right * -1);

                break;
            case AimDirection.UpRight:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    (monoBehaviour.transform.up + monoBehaviour.transform.right).normalized);
                break;
            case AimDirection.UpLeft:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    (monoBehaviour.transform.up + (monoBehaviour.transform.right * -1)).normalized);
                break;
            case AimDirection.DownRight:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    ((monoBehaviour.transform.up * -1) + monoBehaviour.transform.right).normalized);

                break;
            case AimDirection.DownLeft:
                ray = Physics2D.Raycast(monoBehaviour.gameObject.transform.TransformPoint(capsuleCollider2D.offset),
                    ((monoBehaviour.transform.up * -1) + (monoBehaviour.transform.right * -1)).normalized);

                break;
            default:
                Debug.Log("Error");
                break;
        }

        if (ray.collider != null)
        {
            var bulletEffectObject = GameObject.Instantiate(bulletEffect);
            bulletEffectObject.transform.position = ray.point;
            GameObject.Destroy(bulletEffectObject, 1f);
        }
    }

    public void ReloadWeapon()
    {
    }

    public void Cqc()
    {
    }

    public void CheckBullets()
    {
    }

    public void CheckFacingDirection()
    {
        if (MathHelpers.Approximately(playerController.HorizontalAim, 1, float.Epsilon))
        {
            playerStatusVariables.facingDirection = FacingDirection.Right;
        }
        else if (MathHelpers.Approximately(playerController.HorizontalAim, -1, float.Epsilon))
        {
            playerStatusVariables.facingDirection = FacingDirection.Left;
        }
        spriteRenderer.flipX = (playerStatusVariables.facingDirection != FacingDirection.Right);
    }
}