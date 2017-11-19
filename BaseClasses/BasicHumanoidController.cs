using System.Collections;
using UnityEngine;

public abstract class BasicHumanoidController
{
    protected class RevokeControlVariables
    {
        public bool horizontalMovementControl;
        public bool verticalMovementControl;
        public bool ladderMovementControl;
        public bool miscellaneousMovementControl;
        public bool combatMovementControl;
        public bool allMovementControl;
    }

    public abstract void CheckForHorizontalInput();
    public abstract void CheckForVerticalInput();
    public abstract void CheckForMiscellaneousInput();

    protected virtual void RevokeControl(float timeToRevoke, bool revoke,
        ControlTypeToRevoke controlTypeToRevoke, RevokeControlVariables revokeControlVariables,
        MonoBehaviour monoBehaviour)
    {
        var coroutine = CoroutineManager.FindCoroutine("RevokeControlCoroutine");
        if (coroutine == null)
        {
            CoroutineManager.AddCoroutine(
                RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, revokeControlVariables, true),
                "RevokeControlCoroutine");
        }
        else if (!coroutine.IsRunning)
        {
            CoroutineManager.DeleteCoroutine("RevokeControlCoroutine");
            CoroutineManager.AddCoroutine(
                RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, revokeControlVariables, true),
                "RevokeControlCoroutine");
        }
    }

    protected virtual void RevokeControl(bool revoke, ControlTypeToRevoke controlTypeToRevoke,
        RevokeControlVariables revokeControlVariables)
    {
        RevokeControlSelection(revoke, controlTypeToRevoke, revokeControlVariables, false);
    }

    protected static IEnumerator RevokeControlCoroutine(float time, ControlTypeToRevoke controlTypeToRevoke,
        RevokeControlVariables revokeControlVariables,
        bool revoke)
    {
        for (var i = 0; i < 1; i++)
        {
            RevokeControlSelection(revoke, controlTypeToRevoke, revokeControlVariables, false);
            yield return new WaitForSeconds(time);
        }
        RevokeControlSelection(revoke, controlTypeToRevoke, revokeControlVariables, true);
        CoroutineManager.FindCoroutine("RevokeControlCoroutine").IsRunning = false;
    }


    protected static void RevokeControlSelection(bool revoke, ControlTypeToRevoke controlTypeToRevoke,
        RevokeControlVariables revokeControlVariables, bool negate)
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
            case ControlTypeToRevoke.CombatMovement:
                revokeControlVariables.combatMovementControl = revoke;
                break;
            case ControlTypeToRevoke.AllMovement:
                revokeControlVariables.horizontalMovementControl = revoke;
                revokeControlVariables.verticalMovementControl = revoke;
                revokeControlVariables.ladderMovementControl = revoke;
                revokeControlVariables.miscellaneousMovementControl = revoke;
                revokeControlVariables.combatMovementControl = revoke;
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }
}