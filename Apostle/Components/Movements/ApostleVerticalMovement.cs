using System.Collections;
using UnityEngine;

public class ApostleVerticalMovement : BasicPhysicsMovement
{
    public VerticalMovementState VerticalMovementState { get; private set; }
    public VerticalPressMovementState VerticalPressMovementState { get; private set; }

    private BasicCollisionHandler apostleCollisionHandler;
    private ApostleController apostleController;
    private ApostleStatusVariables apostleStatusVariables;
    private Enemy apostle;

    private float climbingLadderSmoothness;
    private float climbingObstacleSmoothness;
    private float climbLadderVelocity;
    private float currentGravityScale;
    private float minimumFallingDistanceForDamage;
    private float minimumDamageForFalling;

    private float distanceWhileFalling;
    private float lastFramePositionWhileFalling;

    public ApostleVerticalMovement(MonoBehaviour monoBehaviour,
        float climbingLadderSmoothness,
        float climbingObstacleSmoothness, float climbLadderVelocity, float minimumFallingDistanceForDamage
        , float minimumDamageForFalling,
        BasicCollisionHandler apostleCollisionHandler,
        ApostleController apostleController, ApostleStatusVariables apostleStatusVariables,
        Enemy apostle) : base(
        monoBehaviour)
    {
        this.apostleStatusVariables = apostleStatusVariables;
        this.apostleCollisionHandler = apostleCollisionHandler;
        this.apostleController = apostleController;
        this.climbingLadderSmoothness = climbingLadderSmoothness;
        this.climbingObstacleSmoothness = climbingObstacleSmoothness;
        this.minimumFallingDistanceForDamage = minimumFallingDistanceForDamage;
        this.minimumDamageForFalling = minimumDamageForFalling;
        this.currentGravityScale = rigidbody2D.gravityScale;
        this.climbLadderVelocity = climbLadderVelocity;
        this.apostle = apostle;
    }


    public override void StartMovement()
    {
        apostleController.CheckForVerticalInput();
        apostleStatusVariables.canJump = CheckGroundForJump();

        apostleStatusVariables.isOnAir = apostleStatusVariables.CheckIsOnAir();

        if (apostleStatusVariables.canClimbLadder && apostleController.ClimbLadderPress)
        {
            apostleStatusVariables.isClimbingLadder =
                apostleStatusVariables.canClimbLadder && apostleController.ClimbLadderPress ||
                apostleStatusVariables.isClimbingLadder;
        }

        if (apostleStatusVariables.canClimbObstacle && apostleController.ClimbObstaclePress)
        {
            apostleStatusVariables.isClimbingObstacle =
                apostleStatusVariables.canClimbObstacle && apostleController.ClimbObstaclePress ||
                apostleStatusVariables.isClimbingObstacle;
        }


        //Para velocidades ridiculamente altas, vai bugar
        if (apostleStatusVariables.isClimbingLadder && apostleStatusVariables.canJump)
        {
            var coroutine = CoroutineManager.FindCoroutine("ClimbOntoLadderCoroutine", this);
            if (coroutine != null && !coroutine.IsRunning)
            {
                PhysicsHelpers.IgnoreCollision(capsuleCollider2D,
                    GetLadderPosition().GetComponent<LadderController>().adjacentCollider, false);
                apostleStatusVariables.isClimbingLadder = false;
                apostleController.RevokeControl(0.3f, false, ControlTypeToRevoke.AllMovement, monoBehaviour);
                PhysicsHelpers.SwitchGravity(rigidbody2D, true, currentGravityScale);
                PhysicsHelpers.ResetVelocityY(rigidbody2D);
                rigidbody2D.isKinematic = false;
                CoroutineManager.DeleteCoroutine("ClimbOntoLadderCoroutine", this);
            }
        }

        if (apostleStatusVariables.isClimbingObstacle && apostleStatusVariables.canJump)
        {
            var coroutine = CoroutineManager.FindCoroutine("ClimbOntoObstacleCoroutine", this);
            if (coroutine != null && !coroutine.IsRunning)
            {
                apostleStatusVariables.isClimbingObstacle = false;
                apostleController.RevokeControl(0.3f, false, ControlTypeToRevoke.AllMovement, monoBehaviour);
                PhysicsHelpers.SwitchGravity(rigidbody2D, true, currentGravityScale);
                rigidbody2D.isKinematic = false;

                CoroutineManager.DeleteCoroutine("ClimbOntoObstacleCoroutine", this);
            }
        }

        CheckForClimbingStairs();

        if (apostleStatusVariables.isOnAir)
        {
            VerticalMovementState = VerticalMovementState.OnAir;
        }
        else
        {
            if (apostleStatusVariables.canClimbLadder && apostleController.ClimbLadderPress)
            {
                VerticalPressMovementState = VerticalPressMovementState.ClimbLadder;
            }
            else if (apostleStatusVariables.canClimbObstacle && apostleController.ClimbObstaclePress)
            {
                VerticalPressMovementState = VerticalPressMovementState.ClimbObstacle;
            }

            if (apostleStatusVariables.isClimbingLadder &&
                !MathHelpers.Approximately(apostleController.VerticalMovement, 0, float.Epsilon))
            {
                VerticalMovementState = VerticalMovementState.ClimbingLadder;
            }
            else if (apostleStatusVariables.isClimbingObstacle)
            {
                VerticalMovementState = VerticalMovementState.ClimbingObstacle;
            }
            else
            {
                VerticalMovementState = VerticalMovementState.Grounded;
            }
        }
    }


    public override void PressMovementHandler()
    {
        switch (VerticalPressMovementState)
        {
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

        VerticalPressMovementState = VerticalPressMovementState.None;
    }

    public override void HoldMovementHandler()
    {
        switch (VerticalMovementState)
        {
            case VerticalMovementState.OnAir:
                apostleController.RevokeControl(0.3f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
                break;
            case VerticalMovementState.ClimbingLadder:
                ClimbLadder(apostleController.VerticalMovement);
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
        if (apostleStatusVariables.isOnAir)
        {
            if (lastFramePositionWhileFalling >= rigidbody2D.position.y && rigidbody2D.velocity.y < 0)
            {
                distanceWhileFalling += lastFramePositionWhileFalling - rigidbody2D.position.y;
            }

            if (apostleStatusVariables.canJump &&
                (rigidbody2D.velocity.y < 0 || MathHelpers.Approximately(rigidbody2D.velocity.y, 0, float.Epsilon)))
            {
                if (distanceWhileFalling >= minimumFallingDistanceForDamage)
                {
                    apostle.TakeDamage(minimumDamageForFalling * distanceWhileFalling /
                                       minimumFallingDistanceForDamage);
                }
                distanceWhileFalling = 0;
                apostleStatusVariables.isOnAir = false;
                apostleController.RevokeControl(0.1f, true, ControlTypeToRevoke.AllMovement, monoBehaviour);
            }

            lastFramePositionWhileFalling = rigidbody2D.position.y;
        }
        else
        {
            lastFramePositionWhileFalling = 0;
        }

        if (!MathHelpers.Approximately(rigidbody2D.velocity.y, 0, float.Epsilon) &&
            MathHelpers.Approximately(apostleController.VerticalMovement, 0, float.Epsilon) &&
            apostleStatusVariables.isClimbingLadder)
        {
            PhysicsHelpers.ResetVelocityY(rigidbody2D);
        }

        if (!apostleStatusVariables.isClimbingStairs)
        {
//            PhysicsHelpers.IgnoreLayerCollision(rigidbody2D.gameObject.layer, LayerMask.NameToLayer("Stairs Ground"),
//                true);
            apostleCollisionHandler.SetLayerForCollisions(new[] {"Ground", "Ground Ignore"});

            var leftRayCollider = apostleCollisionHandler.CastLeftwardRay(LayerMask.GetMask("Stairs Ground")).collider;
            var rightRayCollider =
                apostleCollisionHandler.CastRightwardRay(LayerMask.GetMask("Stairs Ground")).collider;

            if (leftRayCollider != null && !Physics2D.GetIgnoreCollision(capsuleCollider2D, leftRayCollider))
            {
                PhysicsHelpers.IgnoreCollision(capsuleCollider2D, leftRayCollider, true);
            }
            else if (rightRayCollider && !Physics2D.GetIgnoreCollision(capsuleCollider2D, rightRayCollider))
            {
                PhysicsHelpers.IgnoreCollision(capsuleCollider2D, rightRayCollider, true);
            }
        }
    }

    private bool CheckGroundForJump()
    {
        return apostleCollisionHandler.CheckGroundForJump(0.1f);
    }

    public void CheckForClimbingStairs()
    {
        if (apostleStatusVariables.canClimbStairs && !apostleStatusVariables.isClimbingStairs)
        {
            var collider = GetStairsTrigger();

            if (collider != null)
            {
                var stairsController = collider.GetComponent<StairsController>();


                if ((CheckIfObjectIsRight(stairsController.stairsCollider.transform.position)
                        ? apostleController.HorizontalMove > 0
                        : apostleController.HorizontalMove < 0) &&
                    (stairsController.stairsTriggerType == StairsTriggerType.TopTrigger
                        ? apostleController.VerticalMovement < 0
                        : apostleController.VerticalMovement > 0))
                {
                    SetOnStairsColliders(stairsController);
                }
            }
        }
        else if (apostleStatusVariables.isClimbingStairs &&
                 (apostleCollisionHandler.CheckForLayerCollision(LayerMask.GetMask("Ground"), 0.1f) ||
                  apostleStatusVariables.isOnAir) &&
                 apostleStatusVariables.canClimbStairs)
        {
            var collider = GetStairsTrigger();

            if (collider != null)
            {
                var stairsController = collider.GetComponent<StairsController>();

                var stairsCollider = stairsController.stairsCollider.GetComponent<BoxCollider2D>();

                //A normal é sempre perpendicular ao plano, porém é necessário manter a rotação entre 29 e -29
                var normal = stairsCollider.transform.up;
                if (CheckIfObjectIsRight(stairsController.stairsCollider.transform.position)
                    ? PhysicsHelpers.SlopeInclinationRight(normal)
                        ? rigidbody2D.velocity.y < 0
                        : rigidbody2D.velocity.y >= 0
                    : PhysicsHelpers.SlopeInclinationRight(normal)
                        ? rigidbody2D.velocity.y >= 0
                        : rigidbody2D.velocity.y < 0)
                {
                    apostleStatusVariables.isClimbingStairs = false;

                    PhysicsHelpers.IgnoreCollision(capsuleCollider2D, stairsController.adjacentCollider, false);
                    stairsController.adjacentCollider.gameObject.layer = LayerMask.NameToLayer("Ground");
                }
            }
        }
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

    private void SetOnStairsColliders(StairsController stairsController)
    {
        apostleStatusVariables.isClimbingStairs = true;
        PhysicsHelpers.IgnoreCollision(capsuleCollider2D, stairsController.adjacentCollider, true);
        stairsController.adjacentCollider.gameObject.layer = LayerMask.NameToLayer("Ground Ignore");
//        PhysicsHelpers.IgnoreLayerCollision(rigidbody2D.gameObject.layer, LayerMask.NameToLayer("Stairs Ground"),
//            false);
        PhysicsHelpers.IgnoreCollision(capsuleCollider2D, stairsController.stairsCollider, false);

        apostleCollisionHandler.SetLayerForCollisions(new[] {"Ground", "Stairs Ground"});
    }

    public void ClimbOntoObstacle(Vector2 position)
    {
        PhysicsHelpers.SwitchGravity(rigidbody2D, false, currentGravityScale);
        PhysicsHelpers.ResetVelocityX(rigidbody2D);
        PhysicsHelpers.ResetVelocityY(rigidbody2D);
        rigidbody2D.isKinematic = true;

        apostleController.RevokeControl(true, ControlTypeToRevoke.AllMovement);

        var coroutine = CoroutineManager.FindCoroutine("ClimbOntoObstacleCoroutine", this);
        if (coroutine == null)
        {
            CoroutineManager.AddCoroutine(ClimbOntoObstacleCoroutine(position, climbingObstacleSmoothness),
                "ClimbOntoObstacleCoroutine", this);
        }
    }

    public void ClimbOntoLadder(Transform ladderTransform)
    {
        PhysicsHelpers.IgnoreCollision(capsuleCollider2D,
            ladderTransform.GetComponent<LadderController>().adjacentCollider, true);
        PhysicsHelpers.SwitchGravity(rigidbody2D, false, currentGravityScale);
        PhysicsHelpers.ResetVelocityX(rigidbody2D);
        PhysicsHelpers.ResetVelocityY(rigidbody2D);
        rigidbody2D.isKinematic = true;
        apostleController.RevokeControl(true, ControlTypeToRevoke.AllMovement);

        var coroutine = CoroutineManager.FindCoroutine("ClimbOntoLadderCoroutine", this);
        if (coroutine == null)
        {
            CoroutineManager.AddCoroutine(
                ladderTransform.gameObject.layer != LayerMask.NameToLayer("Top Ladder")
                    ? ClimbOntoLadderCoroutine(ladderTransform.GetChild(0).position, climbingLadderSmoothness)
                    : ClimbOntoLadderCoroutine(ladderTransform.GetChild(0).position, climbingLadderSmoothness * 0.50f),
                "ClimbOntoLadderCoroutine", this);
        }
    }

    private IEnumerator ClimbOntoLadderCoroutine(Vector2 position, float changeRate)
    {
        var f = 0.0f;
        var initialPosition = rigidbody2D.position;
        while (!MathHelpers.Approximately(rigidbody2D.position.x, position.x, 0.01f) ||
               !MathHelpers.Approximately(rigidbody2D.position.y, position.y, 0.01f))
        {
            f += changeRate;

            rigidbody2D.MovePosition(Vector2.Lerp(initialPosition, new Vector2(position.x, position.y), f));
            yield return new WaitForFixedUpdate();
        }

        CoroutineManager.FindCoroutine("ClimbOntoLadderCoroutine", this).IsRunning = false;
        apostleController.RevokeControl(false, ControlTypeToRevoke.LadderMovement);
    }

    private IEnumerator ClimbOntoObstacleCoroutine(Vector2 position, float changeRate)
    {
        var apostleSizeX = (position.x > rigidbody2D.position.x)
            ? capsuleCollider2D.size.x / 2
            : -capsuleCollider2D.size.x / 2;
        var apostleSizeY = capsuleCollider2D.size.y / 2;
        var desiredPositionX = position.x + apostleSizeX;
        var desiredPositionY = position.y + apostleSizeY;
        var f = 0.0f;
        var initialPositionForY = rigidbody2D.position;
        var initialPositionForX = new Vector2(rigidbody2D.position.x, desiredPositionY);

        // Bug Corrigido, usar a posição do rigidBody no lerp era a causa, pode ser um quirck do unity
        while (!MathHelpers.Approximately(rigidbody2D.position.x, desiredPositionX, 0.01f) ||
               !MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, 0.01f))
        {
            if (!MathHelpers.Approximately(rigidbody2D.position.y, desiredPositionY, 0.001f))
            {
                f += changeRate;
                if (f >= 1)
                {
                    f = 1;
                }

                rigidbody2D.MovePosition(Vector2.Lerp(initialPositionForY,
                    new Vector2(initialPositionForY.x, desiredPositionY), f));
            }
            else
            {
                if (MathHelpers.Approximately(rigidbody2D.position.x, initialPositionForX.x, 0.01f) &&
                    MathHelpers.Approximately(rigidbody2D.position.y, initialPositionForX.y, 0.01f) &&
                    MathHelpers.Approximately(f, 1, 0.1f))
                {
                    f = 0;
                }
                f += changeRate;
                if (f >= 1)
                {
                    f = 1;
                }
                rigidbody2D.MovePosition(Vector2.Lerp(initialPositionForX,
                    new Vector2(desiredPositionX, initialPositionForX.y), f));
            }


            yield return new WaitForFixedUpdate();
        }
        CoroutineManager.FindCoroutine("ClimbOntoObstacleCoroutine", this).IsRunning = false;
        apostleController.RevokeControl(false, ControlTypeToRevoke.AllMovement);
    }

    private void ClimbLadder(float climbLadderMovement)
    {
        PhysicsHelpers.ClimbLadder(climbLadderVelocity, climbLadderMovement, rigidbody2D);
    }
}