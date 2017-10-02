using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerVerticalMovement : BasicPhysicsMovement
{
    public VerticalMovementState verticalMovementState;
    public VerticalPressMovementState verticalPressMovementState;

    private static PlayerVerticalMovement instance;

    private float jumpForce;
    private float climbingLadderSmoothness;
    private float climbingObstacleSmoothness;
    private float climbLadderVelocity;
    private float currentGravityScale;

    private BasicCollisionHandler basicCollisionHandler;
    private PlayerController playerController;
    private PlayerStatusVariables playerStatusVariables;

    public static PlayerVerticalMovement GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerVerticalMovement();
        }

        return instance;
    }

    private PlayerVerticalMovement()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour,
        float jumpForce, float climbingLadderSmoothness,
        float climbingObstacleSmoothness, float climbLadderVelocity, BasicCollisionHandler basicCollisionHandler,
        PlayerController playerController)
    {
        FillInstance(monoBehaviour);

        this.playerStatusVariables = PlayerStatusVariables.GetInstance();
        this.basicCollisionHandler = basicCollisionHandler;
        this.playerController = playerController;
        this.jumpForce = jumpForce;
        this.climbingLadderSmoothness = climbingLadderSmoothness;
        this.climbingObstacleSmoothness = climbingObstacleSmoothness;
        this.currentGravityScale = rigidbody2D.gravityScale;
        this.climbLadderVelocity = climbLadderVelocity;
    }


    public override void StartMovement()
    {
        playerController.CheckForVerticalInput();

        playerStatusVariables.canJump = CheckGroundForJump();
        playerStatusVariables.isOnAir = playerStatusVariables.CheckIsOnAir();

        if (playerStatusVariables.canClimbLadder && playerController.ClimbLadderPress)
        {
            playerStatusVariables.isClimbingLadder =
                playerStatusVariables.canClimbLadder && playerController.ClimbLadderPress ||
                playerStatusVariables.isClimbingLadder;
        }

        if (playerStatusVariables.canClimbObstacle && playerController.ClimbObstaclePress)
        {
            playerStatusVariables.isClimbingObstacle =
                playerStatusVariables.canClimbObstacle && playerController.ClimbObstaclePress ||
                playerStatusVariables.isClimbingObstacle;
        }

        playerStatusVariables.isJumping = playerStatusVariables.canJump && playerController.Jump &&
                                          !playerStatusVariables.isClimbingObstacle;


        //Para velocidades ridiculamente altas, vai bugar
        if (playerStatusVariables.isClimbingLadder && playerStatusVariables.canJump)
        {
            var coroutine = CoroutineManager.FindCoroutine("ClimbOntoLadderCoroutine");
            if (coroutine != null && !coroutine.IsRunning)
            {
                IgnoreCollision(GetLadderPosition().GetComponent<LadderController>().adjacentCollider, false);
                playerStatusVariables.isClimbingLadder = false;
                playerController.RevokePlayerControl(0.3f, false, ControlTypeToRevoke.AllMovement, monoBehaviour);
                SwitchGravity(true);
                PhysicsHelpers.ResetVelocityY(rigidbody2D);
                CoroutineManager.DeleteCoroutine("ClimbOntoLadderCoroutine");
            }
        }

        if (playerStatusVariables.isClimbingObstacle && playerStatusVariables.canJump)
        {
            var coroutine = CoroutineManager.FindCoroutine("ClimbOntoObstacleCoroutine");
            if (coroutine != null && !coroutine.IsRunning)
            {
                playerStatusVariables.isClimbingObstacle = false;
                playerController.RevokePlayerControl(0.3f, false, ControlTypeToRevoke.AllMovement, monoBehaviour);
                SwitchGravity(true);
                CoroutineManager.DeleteCoroutine("ClimbOntoObstacleCoroutine");
            }
        }

        if (playerStatusVariables.canClimbStairs && !playerStatusVariables.isClimbingStairs)
        {
            var collider = GetStairsTrigger();

            if (collider != null)
            {
                var stairsController = collider.GetComponent<StairsController>();


                if ((CheckIfObjectIsRight(stairsController.stairsCollider.transform.position)
                        ? playerController.HorizontalMove > 0
                        : playerController.HorizontalMove < 0) &&
                    (stairsController.stairsTriggerType == StairsTriggerType.TopTrigger
                        ? playerController.VerticalMovement < 0
                        : playerController.VerticalMovement > 0))
                {
                    SetOnStairsColliders(stairsController);
                }
            }
        }
        else if (playerStatusVariables.isClimbingStairs &&
                 (basicCollisionHandler.CheckForLayerCollision(LayerMask.GetMask("Ground"), 0.1f) ||
                  playerStatusVariables.isOnAir) &&
                 playerStatusVariables.canClimbStairs)
        {
            var collider = GetStairsTrigger();

            if (collider != null)
            {
                var stairsController = collider.GetComponent<StairsController>();

                var stairsCollider = stairsController.stairsCollider.GetComponent<BoxCollider2D>();

                //A normal é sempre perpendicular ao plano, porém é necessário manter a rotação entre 29 e -29
                //var normal = stairsCollider.transform.TransformDirection(new Vector2(center.x, center.y + size.y / 2));
                var normal = stairsCollider.transform.up;
                if (CheckIfObjectIsRight(stairsController.stairsCollider.transform.position)
                    ? PhysicsHelpers.SlopeInclinationRight(normal)
                        ? rigidbody2D.velocity.y < 0
                        : rigidbody2D.velocity.y > 0
                    : PhysicsHelpers.SlopeInclinationRight(normal)
                        ? rigidbody2D.velocity.y > 0
                        : rigidbody2D.velocity.y < 0)
                {
                    playerStatusVariables.isClimbingStairs = false;

                    IgnoreCollision(stairsController.adjacentCollider, false);
                    stairsController.adjacentCollider.gameObject.layer = LayerMask.NameToLayer("Ground");
                }
            }
        }


        if (playerStatusVariables.isOnAir)
        {
            verticalMovementState = VerticalMovementState.OnAir;
        }
        else
        {
            if (playerStatusVariables.isJumping)
            {
                verticalPressMovementState = VerticalPressMovementState.Jump;
            }
            else if (playerStatusVariables.canClimbLadder && playerController.ClimbLadderPress)
            {
                verticalPressMovementState = VerticalPressMovementState.ClimbLadder;
            }
            else if (playerStatusVariables.canClimbObstacle && playerController.ClimbObstaclePress)
            {
                verticalPressMovementState = VerticalPressMovementState.ClimbObstacle;
            }

            if (playerStatusVariables.isClimbingLadder &&
                !MathHelpers.Approximately(playerController.VerticalMovement, 0, float.Epsilon))
            {
                verticalMovementState = VerticalMovementState.ClimbingLadder;
            }
            else if (playerStatusVariables.isClimbingObstacle)
            {
                verticalMovementState = VerticalMovementState.ClimbingObstacle;
            }
            else if (playerStatusVariables.isJumping)
            {
                verticalMovementState = VerticalMovementState.OnAir;
            }
            else
            {
                verticalMovementState = VerticalMovementState.Grounded;
            }
        }
    }

    public override void PressMovementHandler()
    {
        switch (verticalPressMovementState)
        {
            case VerticalPressMovementState.Jump:
                Jump();
                //Para motivos de segurança, caso o fixed update demorar para executar
                playerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);
                //Demora de alguns frames para modificar o tipo de VerticalMovementState
                playerStatusVariables.isOnAir = true;
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

    public override void HoldMovementHandler()
    {
        switch (verticalMovementState)
        {
            case VerticalMovementState.OnAir:
                playerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);
                break;
            case VerticalMovementState.ClimbingLadder:
                ClimbLadder(playerController.VerticalMovement);
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

    public override void ResolvePendencies()
    {
        if (playerStatusVariables.isOnAir && playerStatusVariables.canJump && rigidbody2D.velocity.y <= 0)
        {
            playerStatusVariables.isOnAir = false;
            playerController.RevokePlayerControl(0.3f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
        }

        if (!MathHelpers.Approximately(rigidbody2D.velocity.y, 0, float.Epsilon) &&
            MathHelpers.Approximately(playerController.VerticalMovement, 0, float.Epsilon) &&
            playerStatusVariables.isClimbingLadder)
        {
            PhysicsHelpers.ResetVelocityY(rigidbody2D);
        }

        if (!playerStatusVariables.isClimbingStairs)
        {
            IgnoreLayerCollision(LayerMask.NameToLayer("Stairs Ground"), true);
            basicCollisionHandler.SetLayerForCollisions(new[] {"Ground"});
        }
    }

    private bool CheckGroundForJump()
    {
        return basicCollisionHandler.CheckGroundForJump(0.1f);
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
        capsuleCollider2D.GetContacts(contactFilter2D, ladderColliders);
        return ladderColliders[0].transform;
    }

    private Collider2D GetStairsTrigger()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Top Stairs Trigger", "Bottom Stairs Trigger")
        };

        var stairsTriggers = new Collider2D[1];
        capsuleCollider2D.GetContacts(contactFilter2D, stairsTriggers);
        return stairsTriggers[0];
    }

    private bool CheckIfObjectIsRight(Vector3 position)
    {
        return position.x > rigidbody2D.transform.position.x;
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
        capsuleCollider2D.GetContacts(contactFilter2D, obstacleColliders);

        var parentCollider = obstacleColliders[0].transform.parent.GetComponent<BoxCollider2D>();
        var position = obstacleColliders[0].transform.position.x > parentCollider.transform.position.x
            ? new Vector2(parentCollider.bounds.max.x, parentCollider.bounds.max.y)
            : new Vector2(parentCollider.bounds.min.x, parentCollider.bounds.max.y);

        return position;
    }


    public void Jump()
    {
        PhysicsHelpers.ResetVelocityY(rigidbody2D);
        PhysicsHelpers.Jump(jumpForce, rigidbody2D);
    }

    private void SetOnStairsColliders(StairsController stairsController)
    {
        playerStatusVariables.isClimbingStairs = true;
        IgnoreCollision(stairsController.adjacentCollider, true);
        stairsController.adjacentCollider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        IgnoreLayerCollision(LayerMask.NameToLayer("Stairs Ground"), false);
        basicCollisionHandler.SetLayerForCollisions(new[] {"Ground", "Stairs Ground"});
    }

    public void ClimbOntoObstacle(Vector2 position)
    {
        SwitchGravity(false);
        PhysicsHelpers.ResetVelocityX(rigidbody2D);
        PhysicsHelpers.ResetVelocityY(rigidbody2D);
        playerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);

        var coroutine = CoroutineManager.FindCoroutine("ClimbOntoObstacleCoroutine");
        if (coroutine == null)
        {
            CoroutineManager.AddCoroutine(ClimbOntoObstacleCoroutine(position, climbingObstacleSmoothness),
                "ClimbOntoObstacleCoroutine");
        }
    }

    public void ClimbOntoLadder(Transform ladderTransform)
    {
        IgnoreCollision(ladderTransform.GetComponent<LadderController>().adjacentCollider, true);
        SwitchGravity(false);
        PhysicsHelpers.ResetVelocityX(rigidbody2D);
        PhysicsHelpers.ResetVelocityY(rigidbody2D);
        playerController.RevokePlayerControl(true, ControlTypeToRevoke.AllMovement);

        var coroutine = CoroutineManager.FindCoroutine("ClimbOntoLadderCoroutine");
        if (coroutine == null)
        {
            CoroutineManager.AddCoroutine(
                ladderTransform.gameObject.layer != LayerMask.NameToLayer("Top Ladder")
                    ? ClimbOntoLadderCoroutine(ladderTransform.GetChild(0).position, climbingLadderSmoothness)
                    : ClimbOntoLadderCoroutine(ladderTransform.GetChild(0).position, climbingLadderSmoothness * 0.50f),
                "ClimbOntoLadderCoroutine");
        }
    }

    private IEnumerator ClimbOntoLadderCoroutine(Vector2 position, float changeRate)
    {
        /*  Mathf.Abs(rigidbody2D.position.x - position.x) >= 0.01 ||
              Mathf.Abs(rigidbody2D.position.y - position.y) >= 0.01)*/
        var f = 0.0f;
        var initialPosition = rigidbody2D.position;
        while (!MathHelpers.Approximately(rigidbody2D.position.x, position.x, 0.01f) ||
               !MathHelpers.Approximately(rigidbody2D.position.y, position.y, 0.01f))
        {
            f += changeRate;
            /*rigidbody2D.MovePosition(Vector2.Lerp(rigidbody2D.position, new Vector2(position.x, position.y),
                climbingLadderSmoothness));*/
            rigidbody2D.MovePosition(Vector2.Lerp(initialPosition, new Vector2(position.x, position.y), f));
            yield return new WaitForFixedUpdate();
        }

        CoroutineManager.FindCoroutine("ClimbOntoLadderCoroutine").IsRunning = false;
        playerController.RevokePlayerControl(false, ControlTypeToRevoke.LadderMovement);
    }

    private IEnumerator ClimbOntoObstacleCoroutine(Vector2 position, float changeRate)
    {
        var playerSizeX = (position.x > rigidbody2D.position.x)
            ? capsuleCollider2D.size.x / 2
            : -capsuleCollider2D.size.x / 2;
        var playerSizeY = capsuleCollider2D.size.y / 2;
        var desiredPositionX = position.x + playerSizeX;
        var desiredPositionY = position.y + playerSizeY;
        var f = 0.0f;
        var initialPositionForY = rigidbody2D.position;
        var initialPositionForX = new Vector2(rigidbody2D.position.x, desiredPositionY);

        /* Mathf.Abs(rigidbody2D.position.x - (position.x + playerSizeX)) >= 0.01 ||
           Mathf.Abs(rigidbody2D.position.y - (position.y + playerSizeY)) >= 0.01)*/
        while (!MathHelpers.Approximately(rigidbody2D.position.x, desiredPositionX, 0.01f) ||
               !MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, 0.01f))
        {
            /*rigidbody2D.MovePosition(Vector2.Lerp(rigidbody2D.position,
                !MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, 0.01f)
                    ? new Vector2(rigidbody2D.position.x, desiredPositionY)
                    : new Vector2(desiredPositionX, rigidbody2D.position.y),
                climbingObstacleSmoothness));*/

            if (!MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, float.Epsilon))
            {
                f += changeRate;
                rigidbody2D.MovePosition(Vector2.Lerp(initialPositionForY,
                    new Vector2(rigidbody2D.position.x, desiredPositionY), f));
            }
            else
            {
                if (MathHelpers.Approximately(rigidbody2D.position.x, initialPositionForX.x, 0.01f) &&
                    MathHelpers.Approximately(rigidbody2D.position.y, initialPositionForX.y, 0.01f))
                {
                    f = 0;
                }
                f += changeRate;
                rigidbody2D.MovePosition(Vector2.Lerp(initialPositionForX,
                    new Vector2(desiredPositionX, rigidbody2D.position.y), f));
            }


            yield return new WaitForFixedUpdate();
        }
        CoroutineManager.FindCoroutine("ClimbOntoObstacleCoroutine").IsRunning = false;
        playerController.RevokePlayerControl(false, ControlTypeToRevoke.AllMovement);
    }

    private void IgnoreCollision(Collider2D other, bool ignore)
    {
        Physics2D.IgnoreCollision(capsuleCollider2D, other, ignore);
    }

    private void IgnoreLayerCollision(LayerMask layerMask, bool ignore)
    {
        Physics2D.IgnoreLayerCollision(rigidbody2D.gameObject.layer, layerMask.value, ignore);
    }

    private void ClimbLadder(float climbLadderMovement)
    {
        PhysicsHelpers.ClimbLadder(climbLadderVelocity, climbLadderMovement, rigidbody2D);
    }

    /* private void AddDownwardForce(float force, float surfaceAngle, Vector2 surfaceNormal)
     {
         PhysicsHelpers.AddDownwardForce(force, surfaceAngle, surfaceNormal, rigidbody2D);
     }*/

    private void SwitchGravity(bool on)
    {
        rigidbody2D.gravityScale = on ? currentGravityScale : 0;
    }
}