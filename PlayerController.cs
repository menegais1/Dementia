using System;
using System.Collections;
using UnityEngine;

public static class PlayerController
{
    private struct RevokeControlVariables
    {
        public bool horizontalMovementControl;
        public bool verticalMovementControl;
        public bool miscellaneousMovementControl;
        public bool allMovementControl;
    }

    public static float HorizontalMove { get; private set; }
    public static bool Jog { get; private set; }
    public static bool Run { get; private set; }
    public static bool Crouch { get; private set; }
    public static bool Dodge { get; private set; }
    public static float ClimbLadderMovement { get; private set; }
    public static bool Jump { get; private set; }
    public static bool ClimbObstacles { get; private set; }
    public static bool ClimbLadderPress { get; private set; }


    private static RevokeControlVariables revokeControlVariables;


/*    public static bool takeOfCamera;
    public static bool interactWithScenery;
    public static bool takeItem;
    public static bool onStairsControl;
    public static bool runningCoroutine;*/

    public static void CheckForHorizontalPlayerInput()
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

    public static void CheckForVerticalPlayerInput()
    {
        if (!revokeControlVariables.verticalMovementControl)
        {
            Jump = Input.GetButtonDown("Jump");
            ClimbObstacles = Input.GetButtonDown("Climb Obstacles");
            ClimbLadderPress = Input.GetButtonDown("Climb Ladder");
            ClimbLadderMovement = GetClimbStairsMovement();
        }
        else
        {
            Jump = false;
            ClimbObstacles = false;
            ClimbLadderPress = false;
            ClimbLadderMovement = 0;
        }
    }

    //Para não poluir o código - Revisar isso mais tarde
    private static float GetClimbStairsMovement()
    {
        if ((Input.GetKey("w") && Input.GetKey("s")) || (!Input.GetKey("w") && !Input.GetKey("s")))
        {
            return 0;
        }
        if (Input.GetKey("w"))
        {
            return !MathHelpers.Approximately(ClimbLadderMovement, -1, float.Epsilon) ? 1 : 0;
        }
        if (Input.GetKey("s"))
        {
            return !MathHelpers.Approximately(ClimbLadderMovement, 1, float.Epsilon) ? -1 : 0;
        }
        return 0;
    }

    public static void RevokePlayerControl(float timeToRevoke,
        ControlTypeToRevoke controlTypeToRevoke, MonoBehaviour monoBehaviour)
    {
        var coroutine = CoroutineManager.findCoroutine("RevokeControlCoroutine");
        if (coroutine == null)
        {
            CoroutineManager.insertNewCoroutine(RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, true),
                "RevokeControlCoroutine");
        }
        else if (!coroutine.getIsRunning())
        {
            CoroutineManager.deleteCoroutine("RevokeControlCoroutine");
            CoroutineManager.insertNewCoroutine(RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, true),
                "RevokeControlCoroutine");
        }
    }

    /* public void revokeMovementPlayerControl()
     {
         revokeControl = true;
     }
 
     public void changeMovementPlayerControl(bool onStairs)
     {
         onStairsControl = onStairs;
     }
 
     public void giveMovementPlayerControl()
     {
         revokeControl = false;
     }
 
     public void giveMovementPlayerControlWithCooldown(float timeToCooldown, MonoBehaviour monoBehaviour)
     {
         revokeControl = false;
         horizontalMove = 0;
         if (!runningCoroutine)
             monoBehaviour.StartCoroutine(waitForSeconds(timeToCooldown, false));
     }
 
     private IEnumerator waitForSeconds(float time, bool revoke)
     {
         runningCoroutine = true;
         for (var i = 0; i <= 1; i++)
         {
             revokeControl = !revokeControl;
             yield return new WaitForSeconds(time);
         }
         runningCoroutine = false;
     }*/

    private static IEnumerator RevokeControlCoroutine(float time, ControlTypeToRevoke controlTypeToRevoke,
        bool revoke)
    {
        for (var i = 0; i < 1; i++)
        {
            revokeControlSelection(revoke, controlTypeToRevoke, false);
            yield return new WaitForSeconds(time);
        }
        revokeControlSelection(revoke, controlTypeToRevoke, true);
        CoroutineManager.findCoroutine("RevokeControlCoroutine").setIsRunning(false);
    }


    private static void revokeControlSelection(bool revoke, ControlTypeToRevoke controlTypeToRevoke, bool negate)
    {
        if (negate)
        {
            revoke = !revoke;
        }

        switch (controlTypeToRevoke)
        {
            case ControlTypeToRevoke.HorizontalMovement:
                revokeControlVariables.horizontalMovementControl = revoke;
                break;
            case ControlTypeToRevoke.VerticalMovement:
                revokeControlVariables.verticalMovementControl = revoke;
                break;
            case ControlTypeToRevoke.MiscellaneousMovement:
                revokeControlVariables.miscellaneousMovementControl = revoke;
                break;
            case ControlTypeToRevoke.AllMovement:
                revokeControlVariables.horizontalMovementControl = revoke;
                revokeControlVariables.verticalMovementControl = revoke;
                revokeControlVariables.miscellaneousMovementControl = revoke;
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }
}