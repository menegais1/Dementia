using System;
using UnityEngine;

public class ApostleHorizontalMovement : BasicPhysicsMovement
{
    public HorizontalMovementState horizontalMovementState;

    private static ApostleHorizontalMovement instance;

    private float maxSpeed;
    private float acceleration;
    private float dodgeForce;
    private float crouchingSpeed;
    private float characterHeight;

    private Vector2 forceApplied;

    private ApostleController apostleController;
    private BasicCollisionHandler basicCollisionHandler;
    private ApostleStatusVariables apostleStatusVariables;

    public static ApostleHorizontalMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new ApostleHorizontalMovement();
        }

        return instance;
    }

    private ApostleHorizontalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour,
        float maxSpeed, float acceleration, BasicCollisionHandler basicCollisionHandler,
        ApostleController apostleController)
    {
        FillInstance(monoBehaviour);

        this.apostleStatusVariables = ApostleStatusVariables.GetInstance();
        this.monoBehaviour = monoBehaviour;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.characterHeight = capsuleCollider2D.bounds.size.y;
        this.apostleController = apostleController;
        this.basicCollisionHandler = basicCollisionHandler;
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
                horizontalMovementState = HorizontalMovementState.Running;
            }
            else if (apostleStatusVariables.isJogging)
            {
                horizontalMovementState = HorizontalMovementState.Jogging;
            }
            else
            {
                horizontalMovementState = HorizontalMovementState.Walking;
            }
        }
        else
        {
            horizontalMovementState = HorizontalMovementState.Idle;
        }
    }

    public override void PressMovementHandler()
    {
    }

    public override void HoldMovementHandler()
    {
        switch (horizontalMovementState)
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
            PhysicsHelpers.PreventSlideOnSlopes(basicCollisionHandler.SurfaceAngle, basicCollisionHandler.SurfaceNormal,
                horizontalMovementState == HorizontalMovementState.Idle,
                rigidbody2D);
        }
    }

    public override void ResolvePendencies()
    {
    }

    public bool CheckForPreventSlideOnSlopes()
    {
        return !MathHelpers.Approximately(basicCollisionHandler.SurfaceAngle, 0, float.Epsilon) &&
               (horizontalMovementState == HorizontalMovementState.Idle || rigidbody2D.velocity.y < 0) &&
               !apostleStatusVariables.isOnAir;
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


    public Vector2 PreventSlide(Vector2 forceApplied)
    {
        return PhysicsHelpers.PreventSlide(forceApplied, basicCollisionHandler.SurfaceAngle, rigidbody2D);
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