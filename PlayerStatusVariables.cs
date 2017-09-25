using System.Security.Policy;
using UnityEngine;

public static class PlayerStatusVariables
{
    public static bool isJogging;

    public static bool isOnAir;

    public static bool isJumping;

    public static bool isClimbingLadder;

    public static bool isClimbingObstacle;

    public static bool isCrouching;

    public static bool isDodging;

    public static bool isOnStairs;

    public static bool isCameraZoomed;

    public static bool canJump;

    public static bool canClimbLadder;

    public static bool canClimbStairs;

    public static bool canClimbObstacle;

    public static bool canTakeItem;

    public static bool canInteractWithScenery;

    public static bool isInteractingWithScenery;

    public static bool isTakingItem;

    public static bool isClimbingStairs;
    
    public static FacingDirection facingDirection;

    public static bool CheckIsOnAir()
    {
        return !isClimbingObstacle && !isClimbingLadder && !canJump || isOnAir;
    }

    public static void PrintStatus()
    {
        Debug.Log("IsJogging: " + isJogging + "\n IsOnAir: " + isOnAir + "\n IsJumping: " + isJumping +
                  "\n IsClimbingLadder: " + isClimbingLadder +
                  "\n IsClimbingObstacle: " + isClimbingObstacle + "\n IsCrouching: " + isCrouching + "\n IsDodging: " +
                  isDodging +
                  "\n IsOnStairs: " + isOnStairs + "\n CanJump: " + canJump + "\n CanClimbLadder: " +
                  canClimbLadder + "\n CanClimbObstacle: " + canClimbObstacle);
    }
}