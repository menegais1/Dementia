﻿using System.Collections;
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
    private CapsuleCollider2D capsuleCollider2D;
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
        this.capsuleCollider2D = monoBehaviour.GetComponent<CapsuleCollider2D>();
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

        if (PlayerStatusVariables.canClimbStairs && !PlayerStatusVariables.isClimbingStairs)
        {
            var collider = GetStairsTrigger();

            if (collider != null)
            {
                var stairsController = collider.GetComponent<StairsController>();

                if (CheckIfObjectIsRight(stairsController.stairsCollider.transform.position))
                {
                    if (PlayerController.HorizontalMove > 0 &&
                        (stairsController.stairsTriggerType == StairsTriggerType.TopTrigger
                            ? PlayerController.VerticalMovement < 0
                            : PlayerController.VerticalMovement > 0))
                    {
                        PlayerStatusVariables.isClimbingStairs = true;
                        IgnoreCollision(stairsController.adjacentCollider, true);
                        IgnoreLayerCollision(LayerMask.NameToLayer("Stairs Ground"), false);
                    }
                }
                else
                {
                    if (PlayerController.HorizontalMove < 0 &&
                        (stairsController.stairsTriggerType == StairsTriggerType.TopTrigger
                            ? PlayerController.VerticalMovement < 0
                            : PlayerController.VerticalMovement > 0))
                    {
                        PlayerStatusVariables.isClimbingStairs = true;
                        IgnoreCollision(stairsController.adjacentCollider, true);
                        IgnoreLayerCollision(LayerMask.NameToLayer("Stairs Ground"), false);
                    }
                }
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
                !MathHelpers.Approximately(PlayerController.VerticalMovement, 0, float.Epsilon))
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
                //Demora de alguns frames para modificar o tipo de VerticalMovementState
                PlayerStatusVariables.isOnAir = true;
                break;
            case VerticalPressMovementState.ClimbLadder:
                ClimbOntoLadder(GetLadderPosition());
                break;
            case VerticalPressMovementState.ClimbObstacle:
                ClimbOntoObstacle(GetObstaclePosition());
                break;
            case VerticalPressMovementState.None:
                break;
            case VerticalPressMovementState.ClimbStairs:
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
                ClimbLadder(PlayerController.VerticalMovement);
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
        if (PlayerStatusVariables.isOnAir && PlayerStatusVariables.canJump && rigidbody2D.velocity.y <= 0)
        {
            PlayerStatusVariables.isOnAir = false;
            PlayerController.RevokePlayerControl(0.3f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
        }

        if (!MathHelpers.Approximately(rigidbody2D.velocity.y, 0, float.Epsilon) &&
            MathHelpers.Approximately(PlayerController.VerticalMovement, 0, float.Epsilon) &&
            PlayerStatusVariables.isClimbingLadder)
        {
            ResetVelocityY();
        }

        if (!PlayerStatusVariables.isClimbingStairs)
        {
            IgnoreLayerCollision(LayerMask.NameToLayer("Stairs Ground"), true);
        }
    }

    private bool CheckGroundForJump()
    {
        return playerCollisions.CheckGroundForJump(0.1f);
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
        ResetVelocityY();
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
            CoroutineManager.insertNewCoroutine(ClimbOntoObstacleCoroutine(position, climbingObstacleSmoothness),
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
            CoroutineManager.insertNewCoroutine(
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

        CoroutineManager.findCoroutine("ClimbOntoLadderCoroutine").setIsRunning(false);
        PlayerController.RevokePlayerControl(false, ControlTypeToRevoke.StairsMovement);
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

            if (!MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, 0f))
            {
                f += changeRate;
                rigidbody2D.MovePosition(Vector2.Lerp(initialPositionForY,
                    new Vector2(rigidbody2D.position.x, desiredPositionY), f));
            }
            else
            {
                if (rigidbody2D.position == initialPositionForX)
                {
                    f = 0;
                }
                f += changeRate;
                rigidbody2D.MovePosition(Vector2.Lerp(initialPositionForX,
                    new Vector2(desiredPositionX, rigidbody2D.position.y), f));
            }


            yield return new WaitForFixedUpdate();
        }
        CoroutineManager.findCoroutine("ClimbOntoObstacleCoroutine").setIsRunning(false);
        PlayerController.RevokePlayerControl(false, ControlTypeToRevoke.AllMovement);
    }

    public void IgnoreCollision(Collider2D other, bool ignore)
    {
        Physics2D.IgnoreCollision(capsuleCollider2D, other, ignore);
    }

    public void IgnoreLayerCollision(LayerMask layerMask, bool ignore)
    {
        Physics2D.IgnoreLayerCollision(rigidbody2D.gameObject.layer, layerMask.value, ignore);
    }

    public void ClimbLadder(float climbLadderMovement)
    {
        PhysicsHelpers.ClimbLadder(climbLadderVelocity, climbLadderMovement, rigidbody2D);
    }

    /* private void AddDownwardForce(float force, float surfaceAngle, Vector2 surfaceNormal)
     {
         PhysicsHelpers.AddDownwardForce(force, surfaceAngle, surfaceNormal, rigidbody2D);
     }*/

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