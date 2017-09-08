using System.Collections;
using UnityEngine;

public class VerticalMovement
{
    public VerticalMovementState verticalMovementState;
    public VerticalPressMovementState verticalPressMovementState;

    private static VerticalMovement instance;

    private float jumpForce;
    private float climbingLadderSmoothness;
    private float climbingObstacleSmoothness;
    private float climbLadderVelocity;
    private float currentGravityScale;

    private MonoBehaviour monoBehaviour;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;
    private PlayerCollisions playerCollisions;

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
        float climbingObstacleSmoothness, float climbLadderVelocity)
    {
        playerCollisions = PlayerCollisions.GetInstance();
        this.monoBehaviour = monoBehaviour;
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.boxCollider2D = monoBehaviour.GetComponent<BoxCollider2D>();
        this.jumpForce = jumpForce;
        this.climbingLadderSmoothness = climbingLadderSmoothness;
        this.climbingObstacleSmoothness = climbingObstacleSmoothness;
        this.currentGravityScale = rigidbody2D.gravityScale;
        this.climbLadderVelocity = climbLadderVelocity;
    }

    public void StartVerticalMovement()
    {
        PlayerController.CheckForVerticalPlayerInput();
        PlayerStatusVariables.canJump = CheckGroundForJump();
        PlayerStatusVariables.isOnAir = PlayerStatusVariables.CheckIsOnAir();

        if (PlayerStatusVariables.canClimbLadder && PlayerController.ClimbLadderPress)
        {
            PlayerStatusVariables.isClimbingLadder = true;
        }

        if (PlayerStatusVariables.canClimbObstacle && PlayerController.ClimbObstaclePress)
        {
            PlayerStatusVariables.isClimbingObstacle = true;
        }

        PlayerStatusVariables.isJumping = PlayerStatusVariables.canJump && PlayerController.Jump;


        //Para velocidades ridiculamente altas, vai bugar
        if (PlayerStatusVariables.isClimbingLadder && PlayerStatusVariables.canJump)
        {
            var coroutine = CoroutineManager.findCoroutine("ClimbOntoLadderCoroutine");
            if (coroutine != null && !coroutine.getIsRunning())
            {
                IgnoreCollision(GetLadderPosition().GetComponent<LadderController>().adjacentCollider, false);
                PlayerStatusVariables.isClimbingLadder = false;
                PlayerController.RevokePlayerControl(0.3f, false, ControlTypeToRevoke.AllMovement, monoBehaviour);
                SwitchGravity(true);
                ResetVelocityY();
                CoroutineManager.deleteCoroutine("ClimbOntoLadderCoroutine");
            }
        }

        if (PlayerStatusVariables.isOnAir)
        {
            verticalMovementState = VerticalMovementState.OnAir;
        }
        else
        {
            if (PlayerStatusVariables.isJumping)
            {
                verticalPressMovementState = VerticalPressMovementState.Jump;
            }
            else if (PlayerStatusVariables.canClimbLadder && PlayerController.ClimbLadderPress)
            {
                verticalPressMovementState = VerticalPressMovementState.ClimbLadder;
            }
            else if (PlayerStatusVariables.canClimbObstacle && PlayerController.ClimbObstaclePress)
            {
                verticalPressMovementState = VerticalPressMovementState.ClimbObstacle;
            }

            if (PlayerStatusVariables.isClimbingLadder &&
                !MathHelpers.Approximately(PlayerController.ClimbLadderMovement, 0, float.Epsilon))
            {
                verticalMovementState = VerticalMovementState.ClimbingLadder;
            }
            else if (PlayerStatusVariables.isClimbingObstacle)
            {
                verticalMovementState = VerticalMovementState.ClimbingObstacle;
            }
            else if (PlayerStatusVariables.isJumping)
            {
                verticalMovementState = VerticalMovementState.OnAir;
            }
            else
            {
                verticalMovementState = VerticalMovementState.Grounded;
            }
        }
    }

    public void PressMovementHandler()
    {
        switch (verticalPressMovementState)
        {
            case VerticalPressMovementState.Jump:
                Jump();
                //Para motivos de segurança, caso o fixed update demorar para executar
                PlayerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);
                break;
            case VerticalPressMovementState.ClimbLadder:
                ClimbOntoLadder(GetLadderPosition());
                break;
            case VerticalPressMovementState.ClimbObstacle:

                break;
            case VerticalPressMovementState.None:

                break;
            default:
                Debug.Log("Error");
                break;
        }

        verticalPressMovementState = VerticalPressMovementState.None;
    }

    public void HoldMovementHandler()
    {
        switch (verticalMovementState)
        {
            case VerticalMovementState.Grounded:
                break;
            case VerticalMovementState.OnAir:
                PlayerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);
                break;
            case VerticalMovementState.ClimbingLadder:
                ClimbLadder(PlayerController.ClimbLadderMovement);
                break;
            case VerticalMovementState.ClimbingObstacle:
                break;
            default:
                Debug.Log("ERRO");
                break;
        }
    }

    public void ResolvePendencies()
    {
        if (MathHelpers.Approximately(rigidbody2D.velocity.y, 0, float.Epsilon) && PlayerStatusVariables.isOnAir &&
            PlayerStatusVariables.canJump)
        {
            PlayerStatusVariables.isOnAir = false;
            PlayerController.RevokePlayerControl(0.3f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
        }

        if (!MathHelpers.Approximately(rigidbody2D.velocity.y, 0, float.Epsilon) &&
            MathHelpers.Approximately(PlayerController.ClimbLadderMovement, 0, float.Epsilon) &&
            PlayerStatusVariables.isClimbingLadder)
        {
            ResetVelocityY();
        }
    }

    private bool CheckGroundForJump()
    {
        return playerCollisions.CheckGroundForJump(0.1f);
        /*&& !PlayerStatusVariables.isClimbingLadder &&
        !PlayerStatusVariables.isClimbingObstacle*/
    }

    private Transform GetLadderPosition()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Bottom Ladder", "Top Ladder")
        };

        var stairColliders = new Collider2D[1];
        boxCollider2D.GetContacts(contactFilter2D, stairColliders);
        return stairColliders[0].transform;
    }


    public void Jump()
    {
        PhysicsHelpers.Jump(jumpForce, rigidbody2D);
    }

    public void ClimbOntoLadder(Transform ladderTransform)
    {
        IgnoreCollision(ladderTransform.GetComponent<LadderController>().adjacentCollider, true);
        SwitchGravity(false);
        ResetVelocityX();
        ResetVelocityY();
        PlayerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);

        var coroutine = CoroutineManager.findCoroutine("ClimbOntoLadderCoroutine");
        if (coroutine == null)
        {
            CoroutineManager.insertNewCoroutine(ClimbOntoLadderCoroutine(ladderTransform.GetChild(0).position),
                "ClimbOntoLadderCoroutine");
        }
    }

    private IEnumerator ClimbOntoLadderCoroutine(Vector2 position)
    {
        while (Mathf.Abs(rigidbody2D.position.x - position.x) >= 0.01 ||
               Mathf.Abs(rigidbody2D.position.y - position.y) >= 0.01)
        {
            rigidbody2D.position =
                Vector2.Lerp(rigidbody2D.position, new Vector2(position.x, position.y), climbingLadderSmoothness);
            yield return new WaitForEndOfFrame();
        }
        CoroutineManager.findCoroutine("ClimbOntoLadderCoroutine").setIsRunning(false);
        PlayerController.RevokePlayerControl(false, ControlTypeToRevoke.StairsMovement);
    }

    public void IgnoreCollision(Collider2D other, bool ignore)
    {
        Physics2D.IgnoreCollision(boxCollider2D, other, ignore);
    }

    public void ClimbLadder(float climbLadderMovement)
    {
        PhysicsHelpers.ClimbLadder(climbLadderVelocity, climbLadderMovement, rigidbody2D);
    }

/*
   

    public void snapToPositionObject(Vector2 position)
    {
        resetVelocityX();
        resetVelocityY();
        PlayerController.revokeMovementPlayerControl();
        StartCoroutine(snapToPositionObjectCoroutine(position));
        SnapToPositionRan = true;
    }
    
   

    private IEnumerator snapToPositionObjectCoroutine(Vector2 position)
    {
        float localScale = (position.x > RigidBody.position.x) ? +transform.localScale.x : -transform.localScale.x;
        while (Mathf.Abs(RigidBody.position.x - (position.x + localScale)) >= 0.01 ||
               Mathf.Abs(RigidBody.position.y - (position.y + transform.localScale.y)) >= 0.01)
        {
            if (Mathf.Abs(RigidBody.position.y - (position.y + transform.localScale.y)) >= 0.01)
            {
                RigidBody.position = Vector2.Lerp(RigidBody.position,
                    new Vector2(RigidBody.position.x, (position.y + transform.localScale.y)), StairsSmoothness);
            }
            else
            {
                RigidBody.position = Vector2.Lerp(RigidBody.position,
                    new Vector2(position.x + localScale, RigidBody.position.y), StairsSmoothness);
            }
            yield return new WaitForEndOfFrame();
        }
        CoroutineEndedRunning = true;
        PlayerController.giveMovementPlayerControl();
    }

   
  
    
    
    
    */


    public void SwitchGravity(bool on)
    {
        rigidbody2D.gravityScale = on ? currentGravityScale : 0;
    }

    public void ResetVelocityY()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
    }

    public void ResetVelocityX()
    {
        rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
    }
}