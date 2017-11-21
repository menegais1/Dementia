using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float dodgeForce;
    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbingLadderSmoothness;
    [SerializeField] private float climbingObstacleSmoothness;
    [SerializeField] private float climbLadderVelocity;
    [SerializeField] private float minimumFallingDistanceForDamage;
    [SerializeField] private float minimumDamageForFalling;
    [SerializeField] private float cameraZoomSize;
    [SerializeField] private float maxAngle;
    [SerializeField] private float cqcDistance;
    [SerializeField] private LayerMask layerMaskForCollisions;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Diary diary;
    [SerializeField] private InGameMenuController inGameMenuController;


    public PlayerHorizontalMovement HorizontalMovement { get; private set; }
    public PlayerVerticalMovement VerticalMovement { get; private set; }
    public PlayerMiscellaneousMovement MiscellaneousMovement { get; private set; }
    public PlayerCombatMovement CombatMovement { get; private set; }
    public PlayerGeneralController Player { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public BasicCollisionHandler PlayerCollisionHandler { get; private set; }
    public PlayerStatusVariables PlayerStatusVariables { get; private set; }

    public Inventory Inventory
    {
        get { return inventory; }
    }

    public InGameMenuController InGameMenuController
    {
        get { return inGameMenuController; }
    }

    void Start()
    {
        //PlayerStatusVariables = new PlayerStatusVariables();
        PlayerStatusVariables = GetComponent<PlayerStatusVariables>();
        Player = GetComponent<PlayerGeneralController>();

        PlayerCollisionHandler = new BasicCollisionHandler(this, maxAngle, layerMaskForCollisions);

        PlayerController = new PlayerController(transform);

        HorizontalMovement = new PlayerHorizontalMovement(this, maxSpeed, acceleration,
            dodgeForce, crouchingSpeed, PlayerCollisionHandler, PlayerController, PlayerStatusVariables, Player);

        VerticalMovement = new PlayerVerticalMovement(this, jumpForce, climbingLadderSmoothness,
            climbingObstacleSmoothness, climbLadderVelocity, minimumFallingDistanceForDamage, minimumDamageForFalling,
            PlayerCollisionHandler,
            PlayerController,
            PlayerStatusVariables, Player);

        MiscellaneousMovement = new PlayerMiscellaneousMovement(this, cameraZoomSize, PlayerCollisionHandler,
            PlayerController,
            PlayerStatusVariables, Inventory, diary, InGameMenuController);

        CombatMovement = new PlayerCombatMovement(this, PlayerCollisionHandler, PlayerController,
            PlayerStatusVariables, Inventory, cqcDistance);
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