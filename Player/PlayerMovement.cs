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
    [SerializeField] private GameObject bulletEffect;

    public PlayerHorizontalMovement HorizontalMovement { get; private set; }
    public PlayerVerticalMovement VerticalMovement { get; private set; }
    public PlayerMiscellaneousMovement MiscellaneousMovement { get; private set; }
    public PlayerCombatMovement CombatMovement { get; private set; }

    public PlayerController PlayerController { get; private set; }
    public BasicCollisionHandler PlayerCollisionHandler { get; private set; }
    public PlayerStatusVariables PlayerStatusVariables { get; private set; }

    void Start()
    {
        //PlayerStatusVariables = new PlayerStatusVariables();
        PlayerStatusVariables = GetComponent<PlayerStatusVariables>();
        
        PlayerCollisionHandler = new BasicCollisionHandler();
        PlayerCollisionHandler.InitializeCollisions(this, maxAngle, layerMaskForCollisions);

        PlayerController = new PlayerController();

        HorizontalMovement = new PlayerHorizontalMovement();
        HorizontalMovement.FillInstance(this, maxSpeed, acceleration,
            dodgeForce, crouchingSpeed, PlayerCollisionHandler, PlayerController, PlayerStatusVariables);

        VerticalMovement = new PlayerVerticalMovement();
        VerticalMovement.FillInstance(this, jumpForce, climbingLadderSmoothness,
            climbingObstacleSmoothness, climbLadderVelocity, PlayerCollisionHandler, PlayerController,
            PlayerStatusVariables);

        MiscellaneousMovement = new PlayerMiscellaneousMovement();
        MiscellaneousMovement.FillInstance(this, cameraZoomSize, PlayerCollisionHandler, PlayerController,
            PlayerStatusVariables);

        CombatMovement = new PlayerCombatMovement();
        CombatMovement.FillInstance(this, bulletEffect, PlayerCollisionHandler, PlayerController,
            PlayerStatusVariables);
    }

    void Update()
    {
        PlayerCollisionHandler.StartCollisions(HorizontalMovement.HorizontalMovementState);
        HorizontalMovement.StartMovement();
        HorizontalMovement.PressMovementHandler();
        VerticalMovement.StartMovement();
        VerticalMovement.PressMovementHandler();
        MiscellaneousMovement.StartMovement();
        MiscellaneousMovement.PressMovementHandler();
        CombatMovement.StartMovement();
        CombatMovement.PressMovementHandler();
    }

    void FixedUpdate()
    {
        HorizontalMovement.HoldMovementHandler();
        VerticalMovement.HoldMovementHandler();
        CombatMovement.HoldMovementHandler();
    }

    private void LateUpdate()
    {
        VerticalMovement.ResolvePendencies();
    }
}