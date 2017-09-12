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

        //Checar amanha
        if (PlayerStatusVariables.canClimbLadder && PlayerController.ClimbLadderPress)
        {
            PlayerStatusVariables.isClimbingLadder =
                PlayerStatusVariables.canClimbLadder && PlayerController.ClimbLadderPress ||
                PlayerStatusVariables.isClimbingLadder;
        }

        if (PlayerStatusVariables.canClimbObstacle && PlayerController.ClimbObstaclePress)
        {
            PlayerStatusVariables.isClimbingObstacle =
                PlayerStatusVariables.canClimbObstacle && PlayerController.ClimbObstaclePress ||
                PlayerStatusVariables.isClimbingObstacle;
        }

        PlayerStatusVariables.isJumping = PlayerStatusVariables.canJump && PlayerController.Jump &&
                                          !PlayerStatusVariables.isClimbingObstacle;


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

        if (PlayerStatusVariables.isClimbingObstacle && PlayerStatusVariables.canJump)
        {
            var coroutine = CoroutineManager.findCoroutine("ClimbOntoObstacleCoroutine");
            if (coroutine != null && !coroutine.getIsRunning())
            {
                PlayerStatusVariables.isClimbingObstacle = false;
                PlayerController.RevokePlayerControl(0.3f, false, ControlTypeToRevoke.AllMovement, monoBehaviour);
                SwitchGravity(true);
                CoroutineManager.deleteCoroutine("ClimbOntoObstacleCoroutine");
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
                ClimbOntoObstacle(GetObstaclePosition());
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
            case VerticalMovementState.OnAir:
                PlayerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);
                break;
            case VerticalMovementState.ClimbingLadder:
                ClimbLadder(PlayerController.ClimbLadderMovement);
                break;
            case VerticalMovementState.Grounded:
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

        var ladderColliders = new Collider2D[1];
        boxCollider2D.GetContacts(contactFilter2D, ladderColliders);
        return ladderColliders[0].transform;
    }

    private Vector2 GetObstaclePosition()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Obstacle Trigger"),
        };

        var obstacleColliders = new Collider2D[1];
        boxCollider2D.GetContacts(contactFilter2D, obstacleColliders);

        var parentCollider = obstacleColliders[0].transform.parent.GetComponent<BoxCollider2D>();
        var position = obstacleColliders[0].transform.position.x > parentCollider.transform.position.x
            ? new Vector2(parentCollider.bounds.max.x, parentCollider.bounds.max.y)
            : new Vector2(parentCollider.bounds.min.x, parentCollider.bounds.max.y);

        return position;
    }


    public void Jump()
    {
        PhysicsHelpers.Jump(jumpForce, rigidbody2D);
    }

    public void ClimbOntoObstacle(Vector2 position)
    {
        SwitchGravity(false);
        ResetVelocityX();
        ResetVelocityY();
        PlayerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);

        var coroutine = CoroutineManager.findCoroutine("ClimbOntoObstacleCoroutine");
        if (coroutine == null)
        {
            CoroutineManager.insertNewCoroutine(ClimbOntoObstacleCoroutine(position),
                "ClimbOntoObstacleCoroutine");
        }
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
        /*  Mathf.Abs(rigidbody2D.position.x - position.x) >= 0.01 ||
              Mathf.Abs(rigidbody2D.position.y - position.y) >= 0.01)*/
        while (!MathHelpers.Approximately(rigidbody2D.position.x, position.x, 0.01f) ||
               !MathHelpers.Approximately(rigidbody2D.position.y, position.y, 0.01f))
        {
            rigidbody2D.position =
                Vector2.Lerp(rigidbody2D.position, new Vector2(position.x, position.y),
                    climbingLadderSmoothness);
            yield return new WaitForEndOfFrame();
        }

        CoroutineManager.findCoroutine("ClimbOntoLadderCoroutine").setIsRunning(false);
        PlayerController.RevokePlayerControl(false, ControlTypeToRevoke.StairsMovement);
    }

    private IEnumerator ClimbOntoObstacleCoroutine(Vector2 position)
    {
        var playerSizeX = (position.x > rigidbody2D.position.x) ? boxCollider2D.size.x / 2 : -boxCollider2D.size.x / 2;
        var playerSizeY = boxCollider2D.size.y / 2;
        var desiredPositionX = position.x + playerSizeX;
        var desiredPositionY = position.y + playerSizeY;

        /* Mathf.Abs(rigidbody2D.position.x - (position.x + playerSizeX)) >= 0.01 ||
           Mathf.Abs(rigidbody2D.position.y - (position.y + playerSizeY)) >= 0.01)*/
        while (!MathHelpers.Approximately(rigidbody2D.position.x, desiredPositionX, 0.01f) ||
               !MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, 0.01f))
        {
            rigidbody2D.position = Vector2.Lerp(rigidbody2D.position,
                !MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, 0.01f)
                    ? new Vector2(rigidbody2D.position.x, desiredPositionY)
                    : new Vector2(desiredPositionX, rigidbody2D.position.y),
                climbingObstacleSmoothness);
            yield return new WaitForEndOfFrame();
        }
        CoroutineManager.findCoroutine("ClimbOntoObstacleCoroutine").setIsRunning(false);
        PlayerController.RevokePlayerControl(false, ControlTypeToRevoke.AllMovement);
    }

    public void IgnoreCollision(Collider2D other, bool ignore)
    {
        Physics2D.IgnoreCollision(boxCollider2D, other, ignore);
    }

    public void ClimbLadder(float climbLadderMovement)
    {
        PhysicsHelpers.ClimbLadder(climbLadderVelocity, climbLadderMovement, rigidbody2D);
    }


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