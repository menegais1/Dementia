using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class ApostleVerticalMovement : BasicPhysicsMovement
{
    public VerticalMovementState verticalMovementState;

    private static ApostleVerticalMovement instance;


    private BasicCollisionHandler basicCollisionHandler;
    private ApostleController apostleController;
    private ApostleStatusVariables apostleStatusVariables;

    public static ApostleVerticalMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new ApostleVerticalMovement();
        }

        return instance;
    }

    private ApostleVerticalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour, BasicCollisionHandler basicCollisionHandler,
        ApostleController apostleController)
    {
        FillInstance(monoBehaviour);

        this.apostleStatusVariables = ApostleStatusVariables.GetInstance();
        this.basicCollisionHandler = basicCollisionHandler;
        this.apostleController = apostleController;
    }


    public override void StartMovement()
    {
        apostleStatusVariables.canJump = CheckGroundForJump();
        apostleStatusVariables.isOnAir = apostleStatusVariables.CheckIsOnAir();


        if (apostleStatusVariables.isOnAir)
        {
            verticalMovementState = VerticalMovementState.OnAir;
        }
        else
        {
            verticalMovementState = VerticalMovementState.Grounded;
        }
    }


    public override void PressMovementHandler()
    {
    }

    public override void HoldMovementHandler()
    {
        switch (verticalMovementState)
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
        return basicCollisionHandler.CheckGroundForJump(0.1f);
    }
}