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
    private BoxCollider2D boxCollider2D;
    private PlayerCollisions playerCollisions;
    private VerticalMovement verticalMovement;

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
        verticalMovement = VerticalMovement.GetInstance();
        this.monoBehaviour = monoBehaviour;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.dodgeForce = dodgeForce;
        this.crouchingSpeed = crouchingSpeed;
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.boxCollider2D = monoBehaviour.GetComponent<BoxCollider2D>();
        this.characterHeight = boxCollider2D.bounds.size.y;
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

    public void PressMovementHandler()
    {
        switch (horizontalPressMovementState)
        {
            case HorizontalPressMovementState.Dodge:
                Dodge();
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
                PreventSlide(forceApplied);
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
                PreventSlide(forceApplied);
                break;
            case HorizontalMovementState.CrouchWalking:
                forceApplied = CrouchWalk(PlayerController.HorizontalMove, 1f);
                break;
            default:
                Debug.Log("ERRO");
                break;
        }
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
        return PhysicsHelpers.PreventSlide(forceApplied, rigidbody2D);
    }

    public Vector2 Dodge()
    {
        //Adicionar a colisão com inimigos depois
        ResetVelocityX();

        var forceApplied =
            PhysicsHelpers.AddImpulseForce(dodgeForce, rigidbody2D, PlayerStatusVariables.facingDirection);
        PlayerStatusVariables.isDodging = false;
        PlayerController.RevokePlayerControl(0.6f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
        return forceApplied;
    }

    public void Crouch()
    {
        if (boxCollider2D.size.y > characterHeight / 2)
        {
            boxCollider2D.size = new Vector2(boxCollider2D.size.x, boxCollider2D.size.y - crouchingSpeed);
            boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y - (crouchingSpeed / 2));
            PlayerStatusVariables.isCrouching = true;
        }
    }

    public void Raise()
    {
        if (boxCollider2D.size.y < characterHeight)
        {
            boxCollider2D.size = new Vector2(boxCollider2D.size.x, boxCollider2D.size.y + crouchingSpeed);
            boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y + (crouchingSpeed / 2));
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