using System;
using UnityEngine;

public class HorizontalMovement
{
    public HorizontalMovementState horizontalMovementState;
    public HorizontalPressMovementState horizontalPressMovementState;

    private static HorizontalMovement instance;

    private float maxSpeed;
    private float acceleration;
    private float dodgeForce;
    private float crouchingSpeed;
    private float characterHeight;

    private MonoBehaviour monoBehaviour;
    private Rigidbody2D rigidbody2D;
    private CapsuleCollider2D capsuleCollider2D;
    private PlayerCollisions playerCollisions;

    private SpriteRenderer spriteRenderer;
    //private CoroutineManager coroutineManager;


    public static HorizontalMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new HorizontalMovement();
        }

        return instance;
    }

    private HorizontalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour,
        float maxSpeed, float acceleration,
        float dodgeForce, float crouchingSpeed)
    {
        playerCollisions = PlayerCollisions.GetInstance();
        this.monoBehaviour = monoBehaviour;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.dodgeForce = dodgeForce;
        this.crouchingSpeed = crouchingSpeed;
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.capsuleCollider2D = monoBehaviour.GetComponent<CapsuleCollider2D>();
        this.characterHeight = capsuleCollider2D.bounds.size.y;
        PlayerStatusVariables.facingDirection = FacingDirection.Right;
        this.spriteRenderer = monoBehaviour.GetComponent<SpriteRenderer>();
    }

    public void StartHorizontalMovement()
    {
        PlayerController.CheckForHorizontalPlayerInput();
        CheckFacingDirection();

        if (PlayerController.Jog)
        {
            PlayerStatusVariables.isJogging = !PlayerStatusVariables.isJogging;
        }

        if (PlayerController.Crouch)
        {
            Crouch();
        }
        else if (PlayerStatusVariables.isCrouching)
        {
            Raise();
        }

        PlayerStatusVariables.isDodging = PlayerController.Dodge;

        if (!MathHelpers.Approximately(PlayerController.HorizontalMove, 0, float.Epsilon) &&
            !PlayerStatusVariables.isDodging)
        {
            if (PlayerStatusVariables.isCrouching)
            {
                horizontalMovementState = HorizontalMovementState.CrouchWalking;
            }
            else if (PlayerController.Run)
            {
                horizontalMovementState = HorizontalMovementState.Running;
            }
            else if (PlayerStatusVariables.isJogging)
            {
                horizontalMovementState = HorizontalMovementState.Jogging;
            }
            else
            {
                horizontalMovementState = HorizontalMovementState.Walking;
            }
        }
        else if (PlayerStatusVariables.isDodging)
        {
            horizontalPressMovementState = HorizontalPressMovementState.Dodge;
            horizontalMovementState = HorizontalMovementState.Idle;
        }
        else if (PlayerStatusVariables.isCrouching)
        {
            horizontalMovementState = HorizontalMovementState.CrouchIdle;
        }
        else
        {
            horizontalMovementState = HorizontalMovementState.Idle;
        }
    }

    public void PressMovementHandler(ref Vector2 forceApplied)
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

    public void HoldMovementHandler(ref Vector2 forceApplied)
    {
        switch (horizontalMovementState)
        {
            case HorizontalMovementState.Idle:
                if (!PlayerStatusVariables.isOnAir)
                {
                    PreventSlide(forceApplied);
                }
                break;
            case HorizontalMovementState.Walking:
                forceApplied = Walk(PlayerController.HorizontalMove, 1);
                break;
            case HorizontalMovementState.Jogging:
                forceApplied = Jog(PlayerController.HorizontalMove, 1.5f);
                break;
            case HorizontalMovementState.Running:
                forceApplied = Run(PlayerController.HorizontalMove, 2f);
                break;
            case HorizontalMovementState.CrouchIdle:
                if (!PlayerStatusVariables.isOnAir)
                {
                    PreventSlide(forceApplied);
                }
                break;
            case HorizontalMovementState.CrouchWalking:
                forceApplied = CrouchWalk(PlayerController.HorizontalMove, 1f);
                break;
            default:
                Debug.Log("ERRO");
                break;
        }

        if (CheckForPreventSlideOnSlopes())
        {
            PhysicsHelpers.PreventSlideOnSlopes(playerCollisions.SurfaceAngle, playerCollisions.SurfaceNormal,
                (horizontalMovementState == HorizontalMovementState.Idle ||
                 horizontalMovementState == HorizontalMovementState.CrouchIdle),
                rigidbody2D);
        }
    }

    public bool CheckForPreventSlideOnSlopes()
    {
        return !MathHelpers.Approximately(playerCollisions.SurfaceAngle, 0, float.Epsilon) &&
               (horizontalMovementState == HorizontalMovementState.Idle ||
                horizontalMovementState == HorizontalMovementState.CrouchIdle || rigidbody2D.velocity.y < 0) &&
               !PlayerStatusVariables.isOnAir;
    }


    public Vector2 Walk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisions.SurfaceAngle, playerCollisions.SurfaceNormal);
    }

    public Vector2 Jog(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisions.SurfaceAngle, playerCollisions.SurfaceNormal);
    }

    public Vector2 Run(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisions.SurfaceAngle, playerCollisions.SurfaceNormal);
    }

    public Vector2 CrouchWalk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            playerCollisions.SurfaceAngle, playerCollisions.SurfaceNormal);
    }

    public Vector2 PreventSlide(Vector2 forceApplied)
    {
        return PhysicsHelpers.PreventSlide(forceApplied, playerCollisions.SurfaceAngle, rigidbody2D);
    }

    public Vector2 Dodge()
    {
        //Adicionar a colisão com inimigos depois
        ResetVelocityX();
        ResetVelocityY();
        var forceApplied =
            PhysicsHelpers.AddImpulseForce(dodgeForce, playerCollisions.SurfaceAngle, playerCollisions.SurfaceNormal,
                rigidbody2D, PlayerStatusVariables.facingDirection);
        PlayerStatusVariables.isDodging = false;
        PlayerController.RevokePlayerControl(0.6f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
        return forceApplied;
    }

    public void Crouch()
    {
        if (capsuleCollider2D.size.y > characterHeight / 2)
        {
            capsuleCollider2D.size = new Vector2(capsuleCollider2D.size.x, capsuleCollider2D.size.y - crouchingSpeed);
            capsuleCollider2D.offset =
                new Vector2(capsuleCollider2D.offset.x, capsuleCollider2D.offset.y - (crouchingSpeed / 2));
            PlayerStatusVariables.isCrouching = true;
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
            PlayerStatusVariables.isCrouching = false;
        }
    }

    public void ResetVelocityX()
    {
        rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
    }

    public void ResetVelocityY()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
    }

    public void CheckFacingDirection()
    {
        if (MathHelpers.Approximately(PlayerController.HorizontalMove, 1, float.Epsilon))
        {
            PlayerStatusVariables.facingDirection = FacingDirection.Right;
        }
        else if (MathHelpers.Approximately(PlayerController.HorizontalMove, -1, float.Epsilon))
        {
            PlayerStatusVariables.facingDirection = FacingDirection.Left;
        }


        spriteRenderer.flipX = (PlayerStatusVariables.facingDirection != FacingDirection.Right);
    }
}