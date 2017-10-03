using System;
using System.Collections;
using UnityEngine;

public sealed class ApostleController : BasicHumanoidController
{
    public float HorizontalMove { get; private set; }
    public bool Jog { get; private set; }
    public bool Run { get; private set; }

    private static ApostleController instance;
    private RevokeControlVariables revokeControlVariables;
    
    public static ApostleController GetInstance()
    {
        if (instance == null)
        {
            instance = new ApostleController();
        }

        return instance;
    }

    private ApostleController()
    {
        revokeControlVariables = new RevokeControlVariables();
    }

    public override void CheckForHorizontalInput()
    {
        if (!revokeControlVariables.horizontalMovementControl)
        {
            HorizontalMove = Input.GetAxisRaw("Horizontal");
            Run = Input.GetButton("Running");
            Jog = Input.GetButtonDown("Jogging");
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
    }

    public override void CheckForMiscellaneousInput()
    {
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