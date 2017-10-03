using System;
using UnityEngine;

public class PlayerHorizontalMovement : BasicPhysicsMovement
{
    public HorizontalMovementState horizontalMovementState;
    public HorizontalPressMovementState horizontalPressMovementState;

    private static PlayerHorizontalMovement instance;

    private float maxSpeed;
    private float acceleration;
    private float dodgeForce;
    private float crouchingSpeed;
    private float characterHeight;

    private Vector2 forceApplied;

    private PlayerController playerController;
    private BasicCollisionHandler basicCollisionHandler;
    private PlayerStatusVariables playerStatusVariables;

    public static PlayerHorizontalMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerHorizontalMovement();
        }

        return instance;
    }

    private PlayerHorizontalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour,
        float maxSpeed, float acceleration,
        float dodgeForce, float crouchingSpeed, BasicCollisionHandler basicCollisionHandler,
        PlayerController playerController)
    {
        FillInstance(monoBehaviour);

        this.playerStatusVariables = PlayerStatusVariables.GetInstance();
        this.monoBehaviour = monoBehaviour;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.dodgeForce = dodgeForce;
        this.crouchingSpeed = crouchingSpeed;
        this.characterHeight = capsuleCollider2D.bounds.size.y;
        this.playerController = playerController;
        this.basicCollisionHandler = basicCollisionHandler;
    }

    public override void StartMovement()
    {
        playerController.CheckForHorizontalInput();
        CheckFacingDirection();

        if (playerController.Jog)
        {
            playerStatusVariables.isJogging = !playerStatusVariables.isJogging;
        }

        if (playerController.Crouch)
        {
            Crouch();
        }
        else if (playerStatusVariables.isCrouching)
        {
            Raise();
        }

        playerStatusVariables.isDodging = playerController.Dodge;

        if (!MathHelpers.Approximately(playerController.HorizontalMove, 0, float.Epsilon) &&
            !playerStatusVariables.isDodging)
        {
            if (playerStatusVariables.isCrouching)
            {
                horizontalMovementState = HorizontalMovementState.CrouchWalking;
            }
            else if (playerController.Run)
            {
                horizontalMovementState = HorizontalMovementState.Running;
            }
            else if (playerStatusVariables.isJogging)
            {
                horizontalMovementState = HorizontalMovementState.Jogging;
            }
            else
            {
                horizontalMovementState = HorizontalMovementState.Walking;
            }
        }
        else if (playerStatusVariables.isDodging)
        {
            horizontalPressMovementState = HorizontalPressMovementState.Dodge;
            horizontalMovementState = HorizontalMovementState.Idle;
        }
        else if (playerStatusVariables.isCrouching)
        {
            horizontalMovementState = HorizontalMovementState.CrouchIdle;
        }
        else
        {
            horizontalMovementState = HorizontalMovementState.Idle;
        }
    }

    public override void PressMovementHandler()
    {
        switch (horizontalPressMovementState)
        {
            case HorizontalPressMovementState.Dodge:
                forceApplied = Dodge();
                break;
            case HorizontalPressMovementState.None:
                break;
            default:
                Debug.Log("Error");
                break;
        }

        horizontalPressMovementState = HorizontalPressMovementState.None;
    }

    public override void HoldMovementHandler()
    {
        switch (horizontalMovementState)
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
                forceApplied = Jog(playerController.HorizontalMove, 1.5f);
                break;
            case HorizontalMovementState.Running:
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
            PhysicsHelpers.PreventSlideOnSlopes(basicCollisionHandler.SurfaceAngle, basicCollisionHandler.SurfaceNormal,
                (horizontalMovementState == HorizontalMovementState.Idle ||
                 horizontalMovementState == HorizontalMovementState.CrouchIdle),
                rigidbody2D);
        }
    }

    public override void ResolvePendencies()
    {
    }

    public bool CheckForPreventSlideOnSlopes()
    {
        return !MathHelpers.Approximately(basicCollisionHandler.SurfaceAngle, 0, float.Epsilon) &&
               (horizontalMovementState == HorizontalMovementState.Idle ||
                horizontalMovementState == HorizontalMovementState.CrouchIdle || rigidbody2D.velocity.y < 0) &&
               !playerStatusVariables.isOnAir;
    }


    public Vector2 Walk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            basicCollisionHandler.SurfaceAngle, basicCollisionHandler.SurfaceNormal);
    }

    public Vector2 Jog(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            basicCollisionHandler.SurfaceAngle, basicCollisionHandler.SurfaceNormal);
    }

    public Vector2 Run(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            basicCollisionHandler.SurfaceAngle, basicCollisionHandler.SurfaceNormal);
    }

    public Vector2 CrouchWalk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            basicCollisionHandler.SurfaceAngle, basicCollisionHandler.SurfaceNormal);
    }

    public Vector2 PreventSlide(Vector2 forceApplied)
    {
        return PhysicsHelpers.PreventSlide(forceApplied, basicCollisionHandler.SurfaceAngle, rigidbody2D);
    }

    public Vector2 Dodge()
    {
        //Adicionar a colisão com inimigos depois
        PhysicsHelpers.ResetVelocityX(rigidbody2D);
        PhysicsHelpers.ResetVelocityY(rigidbody2D);
        var forceApplied =
            PhysicsHelpers.AddImpulseForce(dodgeForce, basicCollisionHandler.SurfaceAngle,
                basicCollisionHandler.SurfaceNormal,
                rigidbody2D, playerStatusVariables.facingDirection);
        playerStatusVariables.isDodging = false;
        playerController.RevokeControl(0.6f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
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
        if (MathHelpers.Approximately(playerController.HorizontalMove, 1, float.Epsilon))
        {
            playerStatusVariables.facingDirection = FacingDirection.Right;
        }
        else if (MathHelpers.Approximately(playerController.HorizontalMove, -1, float.Epsilon))
        {
            playerStatusVariables.facingDirection = FacingDirection.Left;
        }

        spriteRenderer.flipX = (playerStatusVariables.facingDirection != FacingDirection.Right);
    }
}