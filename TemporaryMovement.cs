using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float dodgeForce;
    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbingLadderSmoothness;
    [SerializeField] private float climbingObstacleSmoothness;

    private Vector2 forceApplied;

    private HorizontalMovement horizontalMovement;
    private VerticalMovement verticalMovement;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    void Start()
    {
        horizontalMovement = HorizontalMovement.GetInstance();
        horizontalMovement.FillInstance(this, maxSpeed, acceleration,
            dodgeForce, crouchingSpeed);

        verticalMovement = VerticalMovement.GetInstance();
        verticalMovement.FillInstance(this, jumpForce, climbingLadderSmoothness,
            climbingObstacleSmoothness);
    }

    void Update()
    {
        horizontalMovement.StartHorizontalMovement();
        verticalMovement.StartVerticalMovement();
        // print(horizontalMovement.horizontalMovementState);
    }

    void FixedUpdate()
    {
        switch (horizontalMovement.horizontalMovementState)
        {
            case HorizontalMovementState.Idle:
                horizontalMovement.PreventSlide(forceApplied);
                break;
            case HorizontalMovementState.Walking:
                forceApplied = horizontalMovement.Walk(PlayerController.HorizontalMove, 1);
                break;
            case HorizontalMovementState.Jogging:
                forceApplied = horizontalMovement.Jog(PlayerController.HorizontalMove, 1.5f);
                break;
            case HorizontalMovementState.Running:
                forceApplied = horizontalMovement.Run(PlayerController.HorizontalMove, 2f);
                break;
            case HorizontalMovementState.CrouchIdle:
                horizontalMovement.PreventSlide(forceApplied);
                break;
            case HorizontalMovementState.CrouchWalking:
                forceApplied = horizontalMovement.CrouchWalk(PlayerController.HorizontalMove, 1f);
                break;
            case HorizontalMovementState.Dodging:
                horizontalMovement.Dodge();
                break;
            default:
                Debug.Log("ERRO");
                break;
        }
    }
}