using UnityEngine;

public class PlayerStatusVariables
{
    public bool isJogging;

    public bool isOnAir;

    public bool isJumping;

    public bool isClimbingLadder;

    public bool isClimbingObstacle;

    public bool isCrouching;

    public bool isDodging;

    public bool isOnStairs;

    public bool isCameraZoomed;

    public bool canJump;

    public bool canClimbLadder;

    public bool canClimbStairs;

    public bool canClimbObstacle;

    public bool canTakeItem;

    public bool canInteractWithScenery;

    public bool isInteractingWithScenery;

    public bool isTakingItem;

    public bool isClimbingStairs;

    public FacingDirection facingDirection;


    public PlayerStatusVariables()
    {
        facingDirection = FacingDirection.Right;
    }

    public bool CheckIsOnAir()
    {
        return !isClimbingObstacle && !isClimbingLadder && !canJump || isOnAir;
    }
}