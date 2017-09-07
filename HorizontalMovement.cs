﻿using UnityEngine;

public class HorizontalMovement
{
    public HorizontalMovementState horizontalMovementState;
    public FacingDirection facingDirection;

    private static HorizontalMovement instance;

    private float maxSpeed;
    private float acceleration;
    private float dodgeForce;
    private float crouchingSpeed;
    private float characterHeight;

    private MonoBehaviour monoBehaviour;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

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
        this.monoBehaviour = monoBehaviour;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.dodgeForce = dodgeForce;
        this.crouchingSpeed = crouchingSpeed;
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.boxCollider2D = monoBehaviour.GetComponent<BoxCollider2D>();
        this.characterHeight = boxCollider2D.bounds.size.y;
        facingDirection = FacingDirection.Right;
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
            horizontalMovementState = HorizontalMovementState.Dodging;
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

    public Vector2 Walk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D);
    }

    public Vector2 Jog(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D);
    }

    public Vector2 Run(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D);
    }

    public Vector2 CrouchWalk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D);
    }

    public Vector2 PreventSlide(Vector2 forceApplied)
    {
        return PhysicsHelpers.PreventSlide(forceApplied, rigidbody2D);
    }

    public Vector2 Dodge()
    {
        //Adicionar a colisão com inimigos depois
        ResetVelocityX();

        var forceApplied = PhysicsHelpers.AddImpulseForce(dodgeForce, rigidbody2D, facingDirection);
        PlayerStatusVariables.isDodging = false;
        PlayerController.RevokePlayerControl(1f, ControlTypeToRevoke.AllMovement, monoBehaviour);
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
            facingDirection = FacingDirection.Right;
        }
        else if (MathHelpers.Approximately(PlayerController.HorizontalMove, -1, float.Epsilon))
        {
            facingDirection = FacingDirection.Left;
        }


        spriteRenderer.flipX = (facingDirection != FacingDirection.Right);
    }
}