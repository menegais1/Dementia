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
    [SerializeField] private float climbLadderVelocity;
    [SerializeField] private LayerMask layerMaskForCollisions;

    private Vector2 forceApplied;

    private HorizontalMovement horizontalMovement;
    private VerticalMovement verticalMovement;
    private PlayerCollisions playerCollisions;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    void Start()
    {
        horizontalMovement = HorizontalMovement.GetInstance();
        horizontalMovement.FillInstance(this, maxSpeed, acceleration,
            dodgeForce, crouchingSpeed);

        verticalMovement = VerticalMovement.GetInstance();
        verticalMovement.FillInstance(this, jumpForce, climbingLadderSmoothness,
            climbingObstacleSmoothness, climbLadderVelocity);

        playerCollisions = PlayerCollisions.GetInstance();
        playerCollisions.InitializeCollisions(this);
    }

    void Update()
    {
        playerCollisions.StartCollisions(layerMaskForCollisions);
        horizontalMovement.StartHorizontalMovement();
        horizontalMovement.PressMovementHandler();
        verticalMovement.StartVerticalMovement();
        verticalMovement.PressMovementHandler();
        // print(horizontalMovement.horizontalMovementState);
    }

    void FixedUpdate()
    {
        horizontalMovement.HoldMovementHandler(ref forceApplied);
        verticalMovement.HoldMovementHandler();
    }

    private void LateUpdate()
    {
        verticalMovement.ResolvePendencies();
    }
}