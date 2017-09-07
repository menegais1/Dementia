using System.Runtime.InteropServices;
using UnityEngine;

public class VerticalMovement
{
    public VerticalMovementState verticalMovementState;

    private static VerticalMovement instance;

    private float jumpForce;
    private float climbingLadderSmoothness;
    private float climbingObstacleSmoothness;


    private MonoBehaviour monoBehaviour;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;

    public static VerticalMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new VerticalMovement();
        }

        return instance;
    }

    private VerticalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour,
        float jumpForce, float climbingLadderSmoothness,
        float climbingObstacleSmoothness)
    {
        this.monoBehaviour = monoBehaviour;
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.boxCollider2D = monoBehaviour.GetComponent<BoxCollider2D>();
        this.jumpForce = jumpForce;
        this.climbingLadderSmoothness = climbingLadderSmoothness;
        this.climbingObstacleSmoothness = climbingObstacleSmoothness;
    }

    public void StartVerticalMovement()
    {
        PlayerController.CheckForVerticalPlayerInput();
    }


    public void ResetVelocityY()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
    }
}