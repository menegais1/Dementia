using UnityEngine;

public sealed class ApostleController : BasicController
{
    public float HorizontalMove { get; private set; }
    public bool Jog { get; private set; }
    public bool Run { get; private set; }

    public float VerticalMovement { get; private set; }
    public bool ClimbObstaclePress { get; private set; }
    public bool ClimbLadderPress { get; private set; }

    public bool AttackPress { get; private set; }

    private RevokeControlVariables revokeControlVariables;
    private MonoBehaviour monoBehaviour;
    private ApostleInputHandler apostleInputHandler;

    public ApostleController(MonoBehaviour monoBehaviour, ApostleInputHandler apostleInputHandler)
    {
        this.monoBehaviour = monoBehaviour;
        this.apostleInputHandler = apostleInputHandler;
        revokeControlVariables = new RevokeControlVariables();
    }


    public override void CheckForHorizontalInput()
    {
        if (!revokeControlVariables.horizontalMovementControl)
        {
            // HorizontalMove = Input.GetAxisRaw("Horizontal");
            HorizontalMove = apostleInputHandler.MovementDirectionValue;
            Run = false;
            Jog = false;
        }
        else
        {
            HorizontalMove = 0;
            Run = false;
            Jog = false;
        }
    }

    public override void CheckForVerticalInput()
    {
        if (!revokeControlVariables.verticalMovementControl)
        {
            ClimbObstaclePress = apostleInputHandler.ClimbObstacleValue;
            ClimbLadderPress = Input.GetButtonDown("Climb Ladder");
            VerticalMovement = GetClimbLadderMovement();
        }
        else
        {
            ClimbObstaclePress = false;
            ClimbLadderPress = false;
            VerticalMovement = !revokeControlVariables.ladderMovementControl ? GetClimbLadderMovement() : 0;
        }
    }

    public override void CheckForMiscellaneousInput()
    {
    }

    public void CheckForCombatInput()
    {
        if (!revokeControlVariables.combatMovementControl)
        {
            AttackPress = apostleInputHandler.AttackPressValue;
        }
        else
        {
            AttackPress = false;
        }
    }

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