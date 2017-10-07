using System;
using UnityEngine;

public class ApostleHorizontalMovement : BasicPhysicsMovement
{
    public HorizontalMovementState HorizontalMovementState { get; private set; }

    private float maxSpeed;
    private float acceleration;
    private float dodgeForce;
    private float crouchingSpeed;
    private float characterHeight;

    private Vector2 forceApplied;

    private ApostleController apostleController;
    private BasicCollisionHandler apostleCollisionHandler;
    private ApostleStatusVariables apostleStatusVariables;


    public ApostleHorizontalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour,
        float maxSpeed, float acceleration, BasicCollisionHandler apostleCollisionHandler,
        ApostleController apostleController, ApostleStatusVariables apostleStatusVariables)
    {
        FillInstance(monoBehaviour);

        this.apostleStatusVariables = apostleStatusVariables;
        this.monoBehaviour = monoBehaviour;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.characterHeight = capsuleCollider2D.bounds.size.y;
        this.apostleController = apostleController;
        this.apostleCollisionHandler = apostleCollisionHandler;
    }

    public override void StartMovement()
    {
        apostleController.CheckForHorizontalInput();
        CheckFacingDirection();

        if (apostleController.Jog)
        {
            apostleStatusVariables.isJogging = !apostleStatusVariables.isJogging;
        }

        if (!MathHelpers.Approximately(apostleController.HorizontalMove, 0, float.Epsilon))
        {
            if (apostleController.Run)
            {
                HorizontalMovementState = HorizontalMovementState.Running;
            }
            else if (apostleStatusVariables.isJogging)
            {
                HorizontalMovementState = HorizontalMovementState.Jogging;
            }
            else
            {
                HorizontalMovementState = HorizontalMovementState.Walking;
            }
        }
        else
        {
            HorizontalMovementState = HorizontalMovementState.Idle;
        }
    }

    public override void PressMovementHandler()
    {
    }

    public override void HoldMovementHandler()
    {
        switch (HorizontalMovementState)
        {
            case HorizontalMovementState.Idle:
                if (!apostleStatusVariables.isOnAir)
                {
                    PreventSlide(forceApplied);
                }
                break;
            case HorizontalMovementState.Walking:
                forceApplied = Walk(apostleController.HorizontalMove, 1);
                break;
            case HorizontalMovementState.Jogging:
                forceApplied = Jog(apostleController.HorizontalMove, 1.5f);
                break;
            case HorizontalMovementState.Running:
                forceApplied = Run(apostleController.HorizontalMove, 2f);
                break;
            default:
                Debug.Log("ERRO");
                break;
        }

        if (CheckForPreventSlideOnSlopes())
        {
            PhysicsHelpers.PreventSlideOnSlopes(apostleCollisionHandler.SurfaceAngle,
                apostleCollisionHandler.SurfaceNormal,
                HorizontalMovementState == HorizontalMovementState.Idle,
                rigidbody2D);
        }
    }

    public override void ResolvePendencies()
    {
    }

    public bool CheckForPreventSlideOnSlopes()
    {
        return !MathHelpers.Approximately(apostleCollisionHandler.SurfaceAngle, 0, float.Epsilon) &&
               (HorizontalMovementState == HorizontalMovementState.Idle || rigidbody2D.velocity.y < 0) &&
               !apostleStatusVariables.isOnAir;
    }


    public Vector2 Walk(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            apostleCollisionHandler.SurfaceAngle, apostleCollisionHandler.SurfaceNormal);
    }

    public Vector2 Jog(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            apostleCollisionHandler.SurfaceAngle, apostleCollisionHandler.SurfaceNormal);
    }

    public Vector2 Run(float horizontalMove, float constant)
    {
        return PhysicsHelpers.HorizontalMovementByForce(acceleration, constant, maxSpeed, horizontalMove, rigidbody2D,
            apostleCollisionHandler.SurfaceAngle, apostleCollisionHandler.SurfaceNormal);
    }


    public Vector2 PreventSlide(Vector2 forceApplied)
    {
        return PhysicsHelpers.PreventSlide(forceApplied, apostleCollisionHandler.SurfaceAngle, rigidbody2D);
    }

    public void CheckFacingDirection()
    {
        if (MathHelpers.Approximately(apostleController.HorizontalMove, 1, float.Epsilon))
        {
            apostleStatusVariables.facingDirection = FacingDirection.Right;
        }
        else if (MathHelpers.Approximately(apostleController.HorizontalMove, -1, float.Epsilon))
        {
            apostleStatusVariables.facingDirection = FacingDirection.Left;
        }

        spriteRenderer.flipX = (apostleStatusVariables.facingDirection != FacingDirection.Right);
    }
}