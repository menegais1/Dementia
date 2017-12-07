using System.Collections;
using UnityEngine;

public abstract class BasicController
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
        var coroutine = CoroutineManager.FindCoroutine("RevokeControlCoroutine", monoBehaviour);
        if (coroutine == null)
        {
            CoroutineManager.AddCoroutine(
                RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, revokeControlVariables, true, monoBehaviour),
                "RevokeControlCoroutine", monoBehaviour);
        }
        else if (!coroutine.IsRunning)
        {
            CoroutineManager.DeleteCoroutine("RevokeControlCoroutine", monoBehaviour);
            CoroutineManager.AddCoroutine(
                RevokeControlCoroutine(timeToRevoke, controlTypeToRevoke, revokeControlVariables, true, monoBehaviour),
                "RevokeControlCoroutine", monoBehaviour);
        }
    }

    protected virtual void RevokeControl(bool revoke, ControlTypeToRevoke controlTypeToRevoke,
        RevokeControlVariables revokeControlVariables)
    {
        RevokeControlSelection(revoke, controlTypeToRevoke, revokeControlVariables, false);
    }

    protected static IEnumerator RevokeControlCoroutine(float time, ControlTypeToRevoke controlTypeToRevoke,
        RevokeControlVariables revokeControlVariables,
        bool revoke, MonoBehaviour monoBehaviour)
    {
        for (var i = 0; i < 1; i++)
        {
            RevokeControlSelection(revoke, controlTypeToRevoke, revokeControlVariables, false);
            yield return new WaitForSeconds(time);
        }
        RevokeControlSelection(revoke, controlTypeToRevoke, revokeControlVariables, true);
        CoroutineManager.FindCoroutine("RevokeControlCoroutine", monoBehaviour).IsRunning = false;
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