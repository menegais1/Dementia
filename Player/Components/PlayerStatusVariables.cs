using UnityEngine;

public class PlayerStatusVariables : MonoBehaviour
{
    public bool isJoggingActive;

    public bool isJogging;

    public bool isRunning;

    public bool isOnAir;

    public bool isJumping;

    public bool isClimbingLadder;

    public bool isClimbingObstacle;

    public bool isCrouching;

    public bool isDodging;

    public bool isCameraZoomed;

    public bool canJump;

    public bool canClimbLadder;

    public bool canClimbStairs;

    public bool canClimbObstacle;

    public bool canTakeItem;

    public bool canTakeNote;

    public bool canTakeWeapon;

    public bool canInteractWithScenery;

    public bool isInteractingWithScenery;

    public bool isTakingItem;

    public bool isTakingNote;

    public bool isTakingWeapon;

    public bool isClimbingStairs;

    public bool isAiming;

    public bool canAim;

    public bool isInGameMenuOpen;

    public bool isMorphinActive;

    public bool isAdrenalineActive;

    public bool isAdrenalineAfterEffectActive;

    public FacingDirection facingDirection;

    public PlayerStatusVariables()
    {
        facingDirection = FacingDirection.Right;
        isInGameMenuOpen = false;
    }

    public bool CheckIsOnAir()
    {
        return !isClimbingObstacle && !isClimbingLadder && !canJump || isOnAir;
    }

    public bool CheckCanAim()
    {
        return !isClimbingObstacle && !isClimbingLadder && !isOnAir && !isDodging && !isJogging && !isRunning;
    }
}