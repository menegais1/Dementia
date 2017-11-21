using System;
using UnityEngine;

public class PlayerHorizontalMovement : BasicPhysicsMovement
{
    public HorizontalMovementState HorizontalMovementState { get; private set; }
    public HorizontalPressMovementState HorizontalPressMovementState { get; private set; }


    private float maxSpeed;
    private float acceleration;
    private float dodgeForce;
    private float crouchingSpeed;
    private float characterHeight;

    private Vector2 forceApplied;

    private PlayerController playerController;
    private BasicCollisionHandler playerCollisionHandler;
    private PlayerStatusVariables playerStatusVariables;
    private PlayerGeneralController player;

    public PlayerHorizontalMovement(MonoBehaviour monoBehaviour,
        float maxSpeed, float acceleration,
        float dodgeForce, float crouchingSpeed, BasicCollisionHandler playerCollisionHandler,
        PlayerController playerController, PlayerStatusVariables playerStatusVariables,
        PlayerGeneralController player) : base(
        monoBehaviour)
    {
        this.playerStatusVariables = playerStatusVariables;
        this.monoBehaviour = monoBehaviour;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.dodgeForce = dodgeForce;
        this.crouchingSpeed = crouchingSpeed;
        this.characterHeight = capsuleCollider2D.bounds.size.y;
        this.playerController = playerController;
        this.playerCollisionHandler = playerCollisionHandler;
        this.player = player;
    }

    public override void StartMovement()
    {
        playerController.CheckForHorizontalInput();
        CheckFacingDirection();

        if (playerController.Jog)
        {
            playerStatusVariables.isJoggingActive = !playerStatusVariables.isJoggingActive;
        }

        if (playerController.Crouch)
        {
            Crouch();
        }
        else if (playerStatusVariables.isCrouching)
        {
            Raise();
        }

        playerStatusVariables.isDodging = playerController.Dodge && player.CheckStamina(30, true);
        playerStatusVariables.isRunning = false;
        playerStatusVariables.isJogging = false;
        
        if (!MathHelpers.Approximately(playerController.HorizontalMove, 0, float.Epsilon) &&
            !playerStatusVariables.isDodging)
        {
            if (playerStatusVariables.isCrouching)
            {
                HorizontalMovementState = HorizontalMovementState.CrouchWalking;
            }
            else if (playerController.Run && player.CheckStamina(10, false))
            {
                playerStatusVariables.isRunning = true;
                HorizontalMovementState = HorizontalMovementState.Running;
            }
            else if (playerStatusVariables.isJoggingActive && player.CheckStamina(5, false))
            {
                HorizontalMovementState = HorizontalMovementState.Jogging;
                playerStatusVariables.isJogging = true;
            }
            else
            {
                HorizontalMovementState = HorizontalMovementState.Walking;
            }
        }
        else if (playerStatusVariables.isDodging)
        {
            HorizontalPressMovementState = HorizontalPressMovementState.Dodge;
            HorizontalMovementState = HorizontalMovementState.Idle;
        }
        else if (playerStatusVariables.isCrouching)
        {
            HorizontalMovementState = HorizontalMovementState.CrouchIdle;
        }
        else
        {
            HorizontalMovementState = HorizontalMovementState.Idle;
        }
    }

    public override void PressMovementHandler()
    {
        switch (HorizontalPressMovementState)
        {
            case HorizontalPressMovementState.Dodge:
                player.SpendStamina(30, true);
                forceApplied = Dodge();
                break;
            case HorizontalPressMovementState.None:
                break;
            default:
                Debug.Log("Error");
                break;
        }

        HorizontalPressMovementState = HorizontalPressMovementState.None;
    }

    public override void HoldMovementHandler()
    {
        switch (HorizontalMovementState)
        {
            case HorizontalMovementState.Idle:
                if (!playerStatusVariables.isOnAir)
                {
                    PreventSlide(forceApplied);
                }
                break;
            case HorizontalMovementState.Walking:
                forceApplied = Walk(playerController.HorizontalMove, 1);
                break;
            case HorizontalMovementState.Jogging:
                player.SpendStamina(5f, false);
                forceApplied = Jog(playerController.HorizontalMove, 1.5f);
                break;
            case HorizontalMovementState.Running:
                player.SpendStamina(10f, false);
                forceApplied = Run(playerController.HorizontalMove, 2f);
                break;
            case HorizontalMovementState.CrouchIdle:
                if (!playerStatusVariables.isOnAir)
                {
                    PreventSlide(forceApplied);
                }
                break;
            case HorizontalMovementState.CrouchWalking:
                forceApplied = CrouchWalk(playerController.HorizontalMove, 1f);
                break;
            default:
                Debug.Log("ERRO");
                break;
        }

        if (CheckForPreventSlideOnSlopes())
        {
            PhysicsHelpers.PreventSlideOnSlopes(playerCollisionHandler.SurfaceAngle,
                playerCollisionHandler.SurfaceNormal,
                (HorizontalMovementState == HorizontalMovementState.Idle ||
                 HorizontalMovementState == HorizontalMovementState.CrouchIdle),
                rigidbody2D);
        }
    }

    public override void ResolvePendencies()
    {
    }

    public bool CheckForPreventSlideOnSlopes()
    {
        return !MathHelpers.Approximately(playerCollisionHandler.SurfaceAngle, 0, float.Epsilon) &&
               (HorizontalMovementState == HorizontalMovementState.Idle ||
                HorizontalMovementState == HorizontalMovementState.CrouchIdle || rigidbody2D.velocity.y < 0) &&
               !playerStatusVariables.isOnAir;
    }


    public Vector2 Walk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisionHandler.SurfaceAngle, playerCollisionHandler.SurfaceNormal);
    }

    public Vector2 Jog(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisionHandler.SurfaceAngle, playerCollisionHandler.SurfaceNormal);
    }

    public Vector2 Run(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisionHandler.SurfaceAngle, playerCollisionHandler.SurfaceNormal);
    }

    public Vector2 CrouchWalk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisionHandler.SurfaceAngle, playerCollisionHandler.SurfaceNormal);
    }

    public Vector2 PreventSlide(Vector2 forceApplied)
    {
        return PhysicsHelpers.PreventSlide(forceApplied, playerCollisionHandler.SurfaceAngle, rigidbody2D);
    }

    public Vector2 Dodge()
    {
        //Adicionar a colisão com inimigos depois
        PhysicsHelpers.ResetVelocityX(rigidbody2D);
        PhysicsHelpers.ResetVelocityY(rigidbody2D);
        var forceApplied =
            PhysicsHelpers.AddImpulseForce(dodgeForce, playerCollisionHandler.SurfaceAngle,
                playerCollisionHandler.SurfaceNormal,
                rigidbody2D, playerStatusVariables.facingDirection);
        playerStatusVariables.isDodging = false;
        playerController.RevokeControl(0.6f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
        PhysicsHelpers.IgnoreLayerCollision(rigidbody2D.gameObject.layer, LayerMask.NameToLayer("Enemy"),
            true, 0.6f);

        return forceApplied;
    }

    public void Crouch()
    {
        if (capsuleCollider2D.size.y > characterHeight / 2)
        {
            capsuleCollider2D.size = new Vector2(capsuleCollider2D.size.x, capsuleCollider2D.size.y - crouchingSpeed);
            capsuleCollider2D.offset =
                new Vector2(capsuleCollider2D.offset.x, capsuleCollider2D.offset.y - (crouchingSpeed / 2));

            playerStatusVariables.isCrouching = true;
        }
    }

    public void Raise()
    {
        if (capsuleCollider2D.size.y < characterHeight)
        {
            capsuleCollider2D.size = new Vector2(capsuleCollider2D.size.x, capsuleCollider2D.size.y + crouchingSpeed);
            capsuleCollider2D.offset =
                new Vector2(capsuleCollider2D.offset.x, capsuleCollider2D.offset.y + (crouchingSpeed / 2));
        }
        else
        {
            playerStatusVariables.isCrouching = false;
        }
    }


    public void CheckFacingDirection()
    {
        if (MathHelpers.Approximately(playerController.HorizontalMove, 1, float.Epsilon) &&
            !playerStatusVariables.isAiming)
        {
            playerStatusVariables.facingDirection = FacingDirection.Right;
        }
        else if (MathHelpers.Approximately(playerController.HorizontalMove, -1, float.Epsilon) &&
                 !playerStatusVariables.isAiming)
        {
            playerStatusVariables.facingDirection = FacingDirection.Left;
        }

        spriteRenderer.flipX = (playerStatusVariables.facingDirection != FacingDirection.Right);
    }
}