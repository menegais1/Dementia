using UnityEngine;

public class ApostleStatusVariables
{
    private static ApostleStatusVariables instance;

    public bool isJogging;

    public bool isOnAir;

    public bool canJump;

    public FacingDirection facingDirection;

    public static ApostleStatusVariables GetInstance()
    {
        if (instance == null)
        {
            instance = new ApostleStatusVariables();
        }

        return instance;
    }

    private ApostleStatusVariables()
    {
        facingDirection = FacingDirection.Right;
    }


    public bool CheckIsOnAir()
    {
        return !canJump || isOnAir;
    }
}