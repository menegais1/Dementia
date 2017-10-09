using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerController : BasicHumanoidController
{
    public float HorizontalMove { get; private set; }
    public bool Jog { get; private set; }
    public bool Run { get; private set; }
    public bool Crouch { get; private set; }
    public bool Dodge { get; private set; }

    public float VerticalMovement { get; private set; }
    public bool Jump { get; private set; }
    public bool ClimbObstaclePress { get; private set; }
    public bool ClimbLadderPress { get; private set; }

    public bool TakeItemPress { get; private set; }
    public bool InteractWithSceneryPress { get; private set; }
    public bool ZoomCameraPress { get; private set; }

    public bool AimHold { get; private set; }
    public bool ShootPress { get; private set; }
    public float VerticalAim { get; private set; }
    public float HorizontalAim { get; private set; }

    private RevokeControlVariables revokeControlVariables;


    public PlayerController()
    {
        revokeControlVariables = new RevokeControlVariables();
    }

    public override void CheckForHorizontalInput()
    {
        if (!revokeControlVariables.horizontalMovementControl)
        {
            HorizontalMove = Input.GetAxisRaw("Horizontal");
            Dodge = Input.GetButtonDown("Dodge");
            Run = Input.GetButton("Running");
            Jog = Input.GetButtonDown("Jogging");
            Crouch = Input.GetButton("Crouching");
        }
        else
        {
            HorizontalMove = 0;
            Dodge = false;
            Run = false;
            Jog = false;
            Crouch = false;
        }
    }

    public override void CheckForVerticalInput()
    {
        if (!revokeControlVariables.verticalMovementControl)
        {
            Jump = Input.GetButtonDown("Jump");
            ClimbObstaclePress = Input.GetButtonDown("Climb Obstacles");
            ClimbLadderPress = Input.GetButtonDown("Climb Ladder");
            VerticalMovement = GetClimbLadderMovement();
        }
        else
        {
            Jump = false;
            ClimbObstaclePress = false;
            ClimbLadderPress = false;
            VerticalMovement = !revokeControlVariables.ladderMovementControl ? GetClimbLadderMovement() : 0;
        }
    }

    public override void CheckForMiscellaneousInput()
    {
        if (!revokeControlVariables.miscellaneousMovementControl)
        {
            TakeItemPress = Input.GetButtonDown("Take Item");
            InteractWithSceneryPress = Input.GetButtonDown("Interact With Scenery");
            ZoomCameraPress = Input.GetButtonDown("Zoom Camera");
        }
        else
        {
            TakeItemPress = false;
            InteractWithSceneryPress = false;
            ZoomCameraPress = false;
        }
    }

    public void CheckForCombatInput()
    {
        if (!revokeControlVariables.combatMovementControl)
        {
            AimHold = Input.GetButton("Aim");
            ShootPress = Input.GetButtonDown("Shoot");
            VerticalAim = Input.GetAxisRaw("Vertical Aim");
            HorizontalAim = Input.GetAxisRaw("Horizontal Aim");
        }
        else
        {
            AimHold = false;
            ShootPress = false;
            VerticalAim = 0;
            HorizontalAim = 0;
        }
    }

    //Para não poluir o código - Revisar isso mais tarde
    private float GetClimbLadderMovement()
    {
        if ((Input.GetKey("w") && Input.GetKey("s")) || (!Input.GetKey("w") && !Input.GetKey("s")))
        {
            return 0;
        }
        if (Input.GetKey("w"))
        {
            return !MathHelpers.Approximately(VerticalMovement, -1, float.Epsilon) ? 1 : 0;
        }
        if (Input.GetKey("s"))
        {
            return !MathHelpers.Approximately(VerticalMovement, 1, float.Epsilon) ? -1 : 0;
        }
        return 0;
    }

    public void RevokeControl(bool revoke, ControlTypeToRevoke controlTypeToRevoke)
    {
        base.RevokeControl(revoke, controlTypeToRevoke, revokeControlVariables);
    }

    public void RevokeControl(float timeToRevoke, bool revoke, ControlTypeToRevoke controlTypeToRevoke,
        MonoBehaviour monoBehaviour)
    {
        base.RevokeControl(timeToRevoke, revoke, controlTypeToRevoke, revokeControlVariables, monoBehaviour);
    }
}