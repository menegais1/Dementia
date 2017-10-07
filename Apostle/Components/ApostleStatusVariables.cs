using UnityEngine;

public class ApostleStatusVariables
{
    public bool isJogging;

    public bool isOnAir;

    public bool canJump;

    public FacingDirection facingDirection;


    public ApostleStatusVariables()
    {
        facingDirection = FacingDirection.Right;
    }


    public bool CheckIsOnAir()
    {
        return !canJump || isOnAir;
    }
}