using UnityEngine;

public class ApostleStatusVariables : MonoBehaviour
{
    public bool isJogging;

    public bool isOnAir;

    public bool canJump;

    public bool isClimbingLadder;

    public bool isClimbingObstacle;

    public bool canClimbLadder;

    public bool canClimbStairs;

    public bool canClimbObstacle;

    public bool isClimbingStairs;

    public bool isAggroed;

    public bool inAggroRange;

    public bool isPatrolling;

    public FacingDirection facingDirection;


    public ApostleStatusVariables()
    {
        facingDirection = FacingDirection.Right;
    }

    public bool CheckIsOnAir()
    {
        return !isClimbingObstacle && !isClimbingLadder && !canJump || isOnAir;
    }
}