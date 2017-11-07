using UnityEngine;

public class PlayerStatusVariables : MonoBehaviour
{
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

    public bool canTakeWeapon;

    public bool canInteractWithScenery;

    public bool isInteractingWithScenery;

    public bool isTakingItem;

    public bool isTakingWeapon;

    public bool isClimbingStairs;

    public bool isAiming;

    public bool canAim;

    public bool canUseItem;

    public bool canShoot;

    public bool canReloadWeapon;

    public bool canExecuteCQC;

    public bool isInGameMenuOpen;

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