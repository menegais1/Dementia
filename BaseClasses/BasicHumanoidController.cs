using System.Collections;
using UnityEngine;

public abstract class BasicHumanoidController
{
    protected struct RevokeControlVariables
    {
        public bool horizontalMovementControl;
        public bool verticalMovementControl;
        public bool ladderMovementControl;
        public bool miscellaneousMovementControl;
        public bool allMovementControl;
    }

    protected static RevokeControlVariables revokeControlVariables;

    public abstract void CheckForHorizontalInput();
    public abstract void CheckForVerticalInput();
    public abstract void CheckForMiscellaneousInput();

    public void RevokePlayerControl(float timeToRevoke, bool revoke,
        ControlTypeToRevoke controlTypeToRevoke, MonoBehaviour monoBehaviour)
    {
        var coroutine = CoroutineManager.FindCoroutine("RevokeControlCoroutine");
        if (coroutine == null)
        {
            CoroutineManager.AddCoroutine(RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, true),
                "RevokeControlCoroutine");
        }
        else if (!coroutine.IsRunning)
        {
            CoroutineManager.DeleteCoroutine("RevokeControlCoroutine");
            CoroutineManager.AddCoroutine(RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, true),
                "RevokeControlCoroutine");
        }
    }

    public void RevokePlayerControl(bool revoke, ControlTypeToRevoke controlTypeToRevoke)
    {
        RevokeControlSelection(revoke, controlTypeToRevoke, false);
    }

    protected static IEnumerator RevokeControlCoroutine(float time, ControlTypeToRevoke controlTypeToRevoke,
        bool revoke)
    {
        for (var i = 0; i < 1; i++)
        {
            RevokeControlSelection(revoke, controlTypeToRevoke, false);
            yield return new WaitForSeconds(time);
        }
        RevokeControlSelection(revoke, controlTypeToRevoke, true);
        CoroutineManager.FindCoroutine("RevokeControlCoroutine").IsRunning = false;
    }


    protected static void RevokeControlSelection(bool revoke, ControlTypeToRevoke controlTypeToRevoke, bool negate)
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
            case ControlTypeToRevoke.LadderMovement:
                revokeControlVariables.ladderMovementControl = revoke;
                break;
            case ControlTypeToRevoke.MiscellaneousMovement:
                revokeControlVariables.miscellaneousMovementControl = revoke;
                break;
            case ControlTypeToRevoke.AllMovement:
                revokeControlVariables.horizontalMovementControl = revoke;
                revokeControlVariables.verticalMovementControl = revoke;
                revokeControlVariables.ladderMovementControl = revoke;
                revokeControlVariables.miscellaneousMovementControl = revoke;
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }
}