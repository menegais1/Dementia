using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float dodgeForce;
    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbingLadderSmoothness;
    [SerializeField] private float climbingObstacleSmoothness;
    [SerializeField] private float climbLadderVelocity;
    [SerializeField] private float cameraZoomSize;
    [SerializeField] private float maxAngle;
    [SerializeField] private LayerMask layerMaskForCollisions;

    private PlayerHorizontalMovement horizontalMovement;
    private PlayerVerticalMovement verticalMovement;
    private PlayerMiscellaneousMovement miscellaneousMovement;
    private PlayerController playerController;
    private BasicCollisionHandler playerCollisions;

    void Start()
    {
        playerCollisions = new BasicCollisionHandler();
        playerCollisions.InitializeCollisions(this, maxAngle, layerMaskForCollisions);

        playerController = PlayerController.GetInstance();

        horizontalMovement = PlayerHorizontalMovement.GetInstance();
        horizontalMovement.FillInstance(this, maxSpeed, acceleration,
            dodgeForce, crouchingSpeed, playerCollisions, playerController);

        verticalMovement = PlayerVerticalMovement.GetInstance();
        verticalMovement.FillInstance(this, jumpForce, climbingLadderSmoothness,
            climbingObstacleSmoothness, climbLadderVelocity, playerCollisions, playerController);

        miscellaneousMovement = PlayerMiscellaneousMovement.GetInstance();
        miscellaneousMovement.FillInstance(this, cameraZoomSize, playerCollisions, playerController);
    }

    void Update()
    {
        playerCollisions.StartCollisions();
        horizontalMovement.StartMovement();
        horizontalMovement.PressMovementHandler();
        verticalMovement.StartMovement();
        verticalMovement.PressMovementHandler();
        miscellaneousMovement.StartMovement();
        miscellaneousMovement.PressMovementHandler();
    }

    void FixedUpdate()
    {
        horizontalMovement.HoldMovementHandler();
        verticalMovement.HoldMovementHandler();
    }

    private void LateUpdate()
    {
        verticalMovement.ResolvePendencies();
    }
}