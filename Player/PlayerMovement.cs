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
    [SerializeField] private float cqcDistance;
    [SerializeField] private LayerMask layerMaskForCollisions;
    [SerializeField] private Weapon weapon;


    public PlayerHorizontalMovement HorizontalMovement { get; private set; }
    public PlayerVerticalMovement VerticalMovement { get; private set; }
    public PlayerMiscellaneousMovement MiscellaneousMovement { get; private set; }
    public PlayerCombatMovement CombatMovement { get; private set; }
    public Player Player { get; private set; }

    public PlayerController PlayerController { get; private set; }
    public BasicCollisionHandler PlayerCollisionHandler { get; private set; }
    public PlayerStatusVariables PlayerStatusVariables { get; private set; }

    void Start()
    {
        //PlayerStatusVariables = new PlayerStatusVariables();
        PlayerStatusVariables = GetComponent<PlayerStatusVariables>();
        Player = GetComponent<Player>();
        
        PlayerCollisionHandler = new BasicCollisionHandler(this, maxAngle, layerMaskForCollisions);

        PlayerController = new PlayerController(transform);

        HorizontalMovement = new PlayerHorizontalMovement(this, maxSpeed, acceleration,
            dodgeForce, crouchingSpeed, PlayerCollisionHandler, PlayerController, PlayerStatusVariables, Player);

        VerticalMovement = new PlayerVerticalMovement(this, jumpForce, climbingLadderSmoothness,
            climbingObstacleSmoothness, climbLadderVelocity, PlayerCollisionHandler, PlayerController,
            PlayerStatusVariables, Player);

        MiscellaneousMovement = new PlayerMiscellaneousMovement(this, cameraZoomSize, PlayerCollisionHandler,
            PlayerController,
            PlayerStatusVariables);

        CombatMovement = new PlayerCombatMovement(this, PlayerCollisionHandler, PlayerController,
            PlayerStatusVariables, weapon, cqcDistance);
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