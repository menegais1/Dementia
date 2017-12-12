using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public struct PatrolPointInfo
{
    public Transform transform;
    public Collider2D floorCollider2D;
    public Floor currentFloor;
    public TransitionFloor currentTransitionFloor;
    public TransitionFloorType transitionFloorType;

    public PatrolPointInfo(Transform transform, Collider2D collider2D, TransitionFloorType type, Navigation navigation)
    {
        this.transform = transform;
        this.floorCollider2D = collider2D;
        this.transitionFloorType = type;
        this.currentFloor = new Floor();
        this.currentTransitionFloor = new TransitionFloor();
        navigation.CheckForCurrentFloor(transform, floorCollider2D, ref currentFloor, ref currentTransitionFloor);
        navigation.CheckForCurrentTransitionFloor(transform,
            ref currentFloor, ref currentTransitionFloor, type);
    }
}

public struct NavigationNode
{
    public Transform transform;
    public TransitionFloorType type;
    public List<Floor> floors;

    public NavigationNode(Transform transform, TransitionFloorType type, List<Floor> floors)
    {
        this.transform = transform;
        this.type = type;
        this.floors = floors;
    }
}

public class ApostleInputHandler : MonoBehaviour
{
    [SerializeField] private float aggroTime;
    [SerializeField] private Transform startPointTransform;
    [SerializeField] private Collider2D startPointFloorCollider;
    [SerializeField] private TransitionFloorType startPointTransitionFloorType;
    [SerializeField] private Transform endPointTransform;
    [SerializeField] private Collider2D endPointFloorCollider;
    [SerializeField] private TransitionFloorType endPointTransitionFloorType;

    private PatrolPointInfo startPointPatrolInfo;
    private PatrolPointInfo endPointPatrolInfo;
    private List<TransitionFloor> patrolTransitionFloorList;

    private float currentAggroTime;
    private NavigationNode currentAimNode;
    private List<NavigationNode> navigationNodes;

    private Floor currentFloor;
    private TransitionFloor currentTransitionFloor;

    private BoxCollider2D triggerArea;
    private Navigation navigation;
    private BasicCollisionHandler apostleCollisionHandler;
    private ApostleStatusVariables apostleStatusVariables;
    private MonoBehaviour monoBehaviour;

    private float movementDirectionValue;
    private bool climbObstacleValue;

    public bool ClimbObstacleValue
    {
        get { return climbObstacleValue; }
    }

    public float MovementDirectionValue
    {
        get { return movementDirectionValue; }
    }

    private void Start()
    {
        var apostleManager = GetComponent<ApostleManager>();
        this.apostleCollisionHandler = apostleManager.ApostleCollisionHandler;
        this.apostleStatusVariables = apostleManager.ApostleStatusVariables;
        if (GameManager.instance.NavigationAcessor == null)
        {
            GameManager.instance.NavigationAcessor =
                GameObject.FindGameObjectWithTag("Navigation").GetComponent<Navigation>();
        }
        navigation = GameManager.instance.NavigationAcessor;
        CreateTriggerArea();

        startPointPatrolInfo =
            new PatrolPointInfo(startPointTransform, startPointFloorCollider, startPointTransitionFloorType,
                navigation);

        endPointPatrolInfo =
            new PatrolPointInfo(endPointTransform, endPointFloorCollider, endPointTransitionFloorType, navigation);

        patrolTransitionFloorList = new List<TransitionFloor>();


        navigation.CalculatePath(startPointPatrolInfo.currentFloor, endPointPatrolInfo.currentFloor,
            patrolTransitionFloorList);

        currentAimNode = new NavigationNode(startPointTransform, TransitionFloorType.None, null);

        navigationNodes = new List<NavigationNode>
        {
            new NavigationNode(startPointTransform, TransitionFloorType.None, null),
            new NavigationNode(endPointTransform, TransitionFloorType.None, null),
        };

        foreach (var transitionFloor in patrolTransitionFloorList)
        {
            if (transitionFloor.transform == null) continue;

            navigationNodes.Insert(
                navigationNodes.FindIndex(lambdaExpression => lambdaExpression.transform == endPointTransform),
                new NavigationNode(transitionFloor.transform, transitionFloor.type, transitionFloor.floors));
        }
    }

    private void Update()
    {
        SetTriggerAreaDirection();

        if (!CheckIfOnTransitionFloor())
        {
            navigation.CheckForCurrentFloor(transform, apostleCollisionHandler.CapsuleCollider2D, ref currentFloor,
                ref currentTransitionFloor);
        }
        else
        {
            if (apostleStatusVariables.isClimbingObstacle)
                navigation.CheckForCurrentTransitionFloor(transform, apostleCollisionHandler.CapsuleCollider2D,
                    ref currentFloor,
                    ref currentTransitionFloor,
                    TransitionFloorType.Obstacle);
            else if (apostleStatusVariables.isClimbingLadder)
                navigation.CheckForCurrentTransitionFloor(transform, apostleCollisionHandler.CapsuleCollider2D,
                    ref currentFloor,
                    ref currentTransitionFloor,
                    TransitionFloorType.Ladder);
            else if (apostleStatusVariables.isClimbingStairs)
                navigation.CheckForCurrentTransitionFloor(transform, apostleCollisionHandler.CapsuleCollider2D,
                    ref currentFloor,
                    ref currentTransitionFloor,
                    TransitionFloorType.Stairs);
        }


        if (Time.time >= currentAggroTime && apostleStatusVariables.isAggroed && !apostleStatusVariables.inAggroRange)
        {
            currentAimNode = new NavigationNode(startPointTransform, TransitionFloorType.None, null);
            apostleStatusVariables.isAggroed = false;
        }

        apostleStatusVariables.isPatrolling = !apostleStatusVariables.isAggroed;

        if (apostleStatusVariables.isPatrolling)
        {
            foreach (var node in navigationNodes)
            {
                if (currentAimNode.transform == node.transform &&
                    MathHelpers.Approximately(transform.position, node.transform.position,
                        apostleCollisionHandler.CapsuleCollider2D.size.y / 2))
                {
                    var index = navigationNodes.FindIndex(lambdaExpression =>
                        lambdaExpression.Equals(node));

                    if (navigationNodes.Count == index + 1)
                    {
                        navigationNodes.Reverse();
                        break;
                    }


                    currentAimNode = navigationNodes[index + 1];
                    if (currentAimNode.type == TransitionFloorType.Obstacle)
                    {
                        if (currentAimNode.transform.position.y < transform.position.y)
                        {
                            currentAimNode = navigationNodes[index + 2];
                        }
                    }
                }
            }
        }


        if (currentAimNode.transform != null)
        {
            movementDirectionValue =
                MovementDirection(currentAimNode.transform.position);

            switch (currentAimNode.type)
            {
                case TransitionFloorType.None:
                    break;
                case TransitionFloorType.Ladder:
                    break;
                case TransitionFloorType.Obstacle:
                    climbObstacleValue =
                        MathHelpers.Approximately(transform.position, currentAimNode.transform.position,
                            apostleCollisionHandler.CapsuleCollider2D.size.y);
                    break;
                case TransitionFloorType.Stairs:
                    break;
                case TransitionFloorType.Consecutive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        //Debug.Log(!currentAimNode.Equals(null) ? currentAimNode.transform.name : "");
    }

    public bool CheckIfOnTransitionFloor()
    {
        return apostleStatusVariables.isClimbingLadder || apostleStatusVariables.isClimbingStairs ||
               apostleStatusVariables.isClimbingObstacle;
    }

    private bool CheckIfPositionIsOnSight(Vector3 position)
    {
        var startingPoint = position.x > transform.position.x
            ? apostleCollisionHandler.BoxColliderBounds.topRight
            : apostleCollisionHandler.BoxColliderBounds.topLeft;
        var xDirection = position.x - startingPoint.x;
        var yDirection = position.y - startingPoint.y;
        var ray = Physics2D.Raycast(startingPoint, new Vector2(xDirection, yDirection), triggerArea.size.x,
            LayerMask.GetMask("Ground", "Player"));
        return ray.collider != null && ray.collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }

    private float MovementDirection(Vector3 position)
    {
        return position.x > transform.position.x ? 1 : -1;
    }

    private void CreateTriggerArea()
    {
        this.triggerArea = GetComponent<BoxCollider2D>();
        var mainCamera = Camera.main;
        var height = mainCamera.orthographicSize * 2;
        var width = mainCamera.aspect * height;
        triggerArea.size = new Vector2((width / 2) + (width / 12), height / 2);
        triggerArea.offset = new Vector2(width / 5, 0);
    }

    private void SetTriggerAreaDirection()
    {
        if (apostleStatusVariables.facingDirection == FacingDirection.Right && triggerArea.offset.x < 0)
        {
            triggerArea.offset = new Vector2(-triggerArea.offset.x, 0);
        }
        else if (apostleStatusVariables.facingDirection == FacingDirection.Left && triggerArea.offset.x > 0)
        {
            triggerArea.offset = new Vector2(-triggerArea.offset.x, 0);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerManager = other.GetComponent<PlayerManager>();
        if (CheckIfPositionIsOnSight(playerManager.transform.position))
        {
            currentAimNode = new NavigationNode(playerManager.transform, TransitionFloorType.None, null);

            apostleStatusVariables.isAggroed = true;
            apostleStatusVariables.inAggroRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;


        var playerManager = other.GetComponent<PlayerManager>();
        if (CheckIfPositionIsOnSight(playerManager.transform.position))
        {
            currentAimNode = new NavigationNode(playerManager.transform, TransitionFloorType.None, null);

            apostleStatusVariables.isAggroed = true;
            apostleStatusVariables.inAggroRange = true;
        }
        else if (apostleStatusVariables.inAggroRange)
        {
            currentAggroTime = Time.time + aggroTime;
            apostleStatusVariables.inAggroRange = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (currentAimNode.transform != null)
        {
            currentAggroTime = Time.time + aggroTime;
            apostleStatusVariables.inAggroRange = false;
        }
    }
}