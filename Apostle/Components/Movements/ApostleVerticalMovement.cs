using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ApostleVerticalMovement : BasicPhysicsMovement
{
    public VerticalMovementState VerticalMovementState { get; private set; }

    private BasicCollisionHandler apostleCollisionHandler;
    private ApostleController apostleController;
    private ApostleStatusVariables apostleStatusVariables;


    public ApostleVerticalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour, BasicCollisionHandler apostleCollisionHandler,
        ApostleController apostleController, ApostleStatusVariables apostleStatusVariables)
    {
        FillInstance(monoBehaviour);

        this.apostleStatusVariables = apostleStatusVariables;
        this.apostleCollisionHandler = apostleCollisionHandler;
        this.apostleController = apostleController;
    }


    public override void StartMovement()
    {
        apostleStatusVariables.canJump = CheckGroundForJump();
        apostleStatusVariables.isOnAir = apostleStatusVariables.CheckIsOnAir();


        if (apostleStatusVariables.isOnAir)
        {
            VerticalMovementState = VerticalMovementState.OnAir;
        }
        else
        {
            VerticalMovementState = VerticalMovementState.Grounded;
        }
    }


    public override void PressMovementHandler()
    {
    }

    public override void HoldMovementHandler()
    {
        switch (VerticalMovementState)
        {
            case VerticalMovementState.OnAir:
                apostleController.RevokeControl(true, ControlTypeToRevoke.AllMovement);
                break;
            case VerticalMovementState.Grounded:
                break;
            default:
                Debug.Log("ERRO");
                break;
        }
    }

    public override void ResolvePendencies()
    {
        if (apostleStatusVariables.isOnAir &&
            apostleStatusVariables.canJump &&
            (rigidbody2D.velocity.y < 0 || MathHelpers.Approximately(rigidbody2D.velocity.y, 0, float.Epsilon)))
        {
            apostleStatusVariables.isOnAir = false;
            apostleController.RevokeControl(0.3f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
        }
    }

    private bool CheckGroundForJump()
    {
        return apostleCollisionHandler.CheckGroundForJump(0.1f);
    }
}