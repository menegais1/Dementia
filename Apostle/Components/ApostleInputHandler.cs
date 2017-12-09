using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using UnityEngine;


public struct NavigationNode
{
    public Vector3 position;
    public Transform transform;
    public NavigationNodeType type;

    public NavigationNode(Vector3 position, Transform transform, NavigationNodeType type)
    {
        this.position = position;
        this.transform = transform;
        this.type = type;
    }
}

public class ApostleInputHandler : MonoBehaviour
{
    [SerializeField] private float aggroTime;
    [SerializeField] private Transform startPointTransform;
    [SerializeField] private Transform endPointTransform;

    private float currentAggroTime;

    private BoxCollider2D triggerArea;
    private NavigationNode currentAimNode;
    private List<NavigationInfo> worldNavigationInfo;

    private List<NavigationNode> navigationNodes;
    private List<NavigationNode> patrollingNavigationNodes;
    private List<NavigationNode> currentAimNodesHistory;
    private BasicCollisionHandler apostleCollisionHandler;

    private ApostleStatusVariables apostleStatusVariables;
    private MonoBehaviour monoBehaviour;

    private bool isOnFirstUpdate;
    private float timeForCheckNavigation;
    private float currentTimeForCheckNavigation;

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
        worldNavigationInfo = GameManager.instance.NavigationAcessor.WorldNavigationInfo;


        CreateTriggerArea();

        patrollingNavigationNodes = new List<NavigationNode>
        {
            new NavigationNode(startPointTransform.position, startPointTransform, NavigationNodeType.None),
            new NavigationNode(endPointTransform.position, endPointTransform, NavigationNodeType.None)
        };
        currentAimNode = new NavigationNode(startPointTransform.position, startPointTransform, NavigationNodeType.None);
        isOnFirstUpdate = true;
    }

    private void Update()
    {
        if (isOnFirstUpdate)
        {
            CheckForNavigationNodes(endPointTransform.position, ref patrollingNavigationNodes);

            foreach (var patrollingNavigationNode in patrollingNavigationNodes)
            {
                Debug.Log(patrollingNavigationNode.transform.name);
            }
            isOnFirstUpdate = false;
        }

        SetTriggerAreaDirection();
        if (Time.time >= currentAggroTime && apostleStatusVariables.isAggroed && !apostleStatusVariables.inAggroRange)
        {
            currentAimNode =
                new NavigationNode(startPointTransform.position, startPointTransform, NavigationNodeType.None);
            apostleStatusVariables.isAggroed = false;
        }

        apostleStatusVariables.isPatrolling = !apostleStatusVariables.isAggroed;

        if (apostleStatusVariables.isPatrolling)
        {
            foreach (var patrollingNavigationNode in patrollingNavigationNodes)
            {
                if (currentAimNode.position == patrollingNavigationNode.position &&
                    MathHelpers.Approximately(transform.position, patrollingNavigationNode.position,
                        apostleCollisionHandler.CapsuleCollider2D.size.y / 2))
                {

                    var index = patrollingNavigationNodes.FindIndex(lambdaExpression =>
                        lambdaExpression.Equals(patrollingNavigationNode));
                    if (patrollingNavigationNodes.Count == index + 1)
                    {
                        patrollingNavigationNodes.Reverse();
                        break;
                    }
                    climbObstacleValue = patrollingNavigationNode.type == NavigationNodeType.Obstacle;


                    currentAimNode = patrollingNavigationNodes[index + 1];

                    if (currentAimNode.type == NavigationNodeType.Obstacle &&
                        currentAimNode.transform == patrollingNavigationNode.transform ||
                        currentAimNode.position.y - transform.position.y <=
                        -apostleCollisionHandler.CapsuleCollider2D.size.y / 2)
                    {
                        currentAimNode = patrollingNavigationNodes[index + 2];
                    }
                }
            }
        }

        if (!currentAimNode.Equals(null))
            movementDirectionValue =
                MovementDirection(currentAimNode.position);

        //Debug.Log(!currentAimNode.Equals(null) ? currentAimNode.transform.name : "");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerManager = other.GetComponent<PlayerManager>();
        if (CheckIfPositionIsOnSight(playerManager.transform.position))
        {
            currentAimNode = new NavigationNode(playerManager.transform.position, playerManager.transform,
                NavigationNodeType.Player);

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
            currentAimNode = new NavigationNode(playerManager.transform.position, playerManager.transform,
                NavigationNodeType.Player);
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

        if (!currentAimNode.Equals(null))
        {
            currentAggroTime = Time.time + aggroTime;
            apostleStatusVariables.inAggroRange = false;
        }
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


    public void CheckForNavigationNodes(Vector3 endPointPosition, ref List<NavigationNode> navigationNodes)
    {
        var startingPoint = endPointPosition.x > transform.position.x
            ? apostleCollisionHandler.BoxColliderBounds.topRight
            : apostleCollisionHandler.BoxColliderBounds.topLeft;
        var xDirection = endPointPosition.x - startingPoint.x;
        var yDirection = endPointPosition.y - startingPoint.y;
        var raycastHit2D = Physics2D.RaycastAll(startingPoint, new Vector3(xDirection, yDirection, 0),
            Mathf.Abs(endPointPosition.x - startingPoint.x), LayerMask.GetMask("Ground"));

        if (!MathHelpers.Approximately(endPointPosition.y,
            transform.position.y, apostleCollisionHandler.CapsuleCollider2D.size.y / 2))
        {
            var possibleNavigationNodes = new List<NavigationInfo>();

            foreach (var navigationInfo in worldNavigationInfo)
            {
                if (MathHelpers.Approximately(navigationInfo.colliderBounds.topMid.y, endPointPosition.y,
                    apostleCollisionHandler.CapsuleCollider2D.size.y / 2))
                {
                    possibleNavigationNodes.Add(navigationInfo);
                }
            }

            if (possibleNavigationNodes.Count <= 0) return;

            possibleNavigationNodes = possibleNavigationNodes.OrderBy(lambdaExpression =>
                Mathf.Abs(lambdaExpression.transform.position.x - endPointPosition.x)).ToList();


            var navigationNode = InsertNewHeightNavigationNode(possibleNavigationNodes);
            if (!navigationNodes.Exists(lambdaExpression => lambdaExpression.transform == navigationNode.transform))
            {
                navigationNodes.Insert(navigationNodes.FindIndex(lambdaExpression =>
                    lambdaExpression.position == endPointPosition), navigationNode);

                CheckForNavigationNodes(navigationNode.position, ref navigationNodes);
            }
        }
        else
        {
            foreach (var hit2D in raycastHit2D)
            {
                if (hit2D.collider != null && hit2D.collider.CompareTag("Obstacle") &&
                    !navigationNodes.Exists(lambdaExpression => lambdaExpression.transform == hit2D.transform))
                {
                    var possibleNavigationNodes = new List<NavigationInfo>();
                    foreach (var navigationInfo in worldNavigationInfo)
                    {
                        if (navigationInfo.transform == hit2D.transform)
                        {
                            possibleNavigationNodes.Add(navigationInfo);
                        }
                    }
                    if (possibleNavigationNodes.Count <= 0) return;

                    possibleNavigationNodes = possibleNavigationNodes.OrderBy(lambdaExpression =>
                        Mathf.Abs(lambdaExpression.transform.position.x - endPointPosition.x)).ToList();

                    var navigationNode = InsertNewObstacleNavigationNode(possibleNavigationNodes);
                    navigationNodes.InsertRange(navigationNodes.FindIndex(lambdaExpression =>
                        lambdaExpression.position == endPointPosition), navigationNode);

                    CheckForNavigationNodes(navigationNode[0].position, ref navigationNodes);
                }
            }
        }
    }

    private NavigationNode InsertNewHeightNavigationNode(List<NavigationInfo> possibleNavigationNodes)
    {
        Vector3 position = Vector3.zero;
        if (possibleNavigationNodes[0].type == NavigationNodeType.Obstacle)
        {
            if (possibleNavigationNodes[0].transform.position.x > transform.position.x)
            {
                position =
                    possibleNavigationNodes[0].colliderBounds.bottomLeft;
                position = new Vector2(position.x - apostleCollisionHandler.CapsuleCollider2D.size.x / 2,
                    position.y + apostleCollisionHandler.CapsuleCollider2D.size.y / 2);
            }
            else
            {
                position =
                    possibleNavigationNodes[0].colliderBounds.bottomRight;
                position = new Vector2(position.x + apostleCollisionHandler.CapsuleCollider2D.size.x / 2,
                    position.y + apostleCollisionHandler.CapsuleCollider2D.size.y / 2);
            }
        }
        return new NavigationNode(position, possibleNavigationNodes[0].transform, possibleNavigationNodes[0].type);
    }

    private List<NavigationNode> InsertNewObstacleNavigationNode(List<NavigationInfo> possibleNavigationNodes)
    {
        Vector3 firstPosition = Vector3.zero;
        Vector3 secondPosition = Vector3.zero;
        if (possibleNavigationNodes[0].type == NavigationNodeType.Obstacle)
        {
            if (possibleNavigationNodes[0].transform.position.x > transform.position.x)
            {
                firstPosition =
                    possibleNavigationNodes[0].colliderBounds.bottomLeft;
                firstPosition = new Vector2(firstPosition.x - apostleCollisionHandler.CapsuleCollider2D.size.x / 2,
                    firstPosition.y + apostleCollisionHandler.CapsuleCollider2D.size.y / 2);
                secondPosition = possibleNavigationNodes[0].colliderBounds.bottomRight;
                secondPosition = new Vector2(secondPosition.x + apostleCollisionHandler.CapsuleCollider2D.size.x / 2,
                    secondPosition.y + apostleCollisionHandler.CapsuleCollider2D.size.y / 2);
            }
            else
            {
                firstPosition =
                    possibleNavigationNodes[0].colliderBounds.bottomRight;
                firstPosition = new Vector2(firstPosition.x + apostleCollisionHandler.CapsuleCollider2D.size.x / 2,
                    firstPosition.y + apostleCollisionHandler.CapsuleCollider2D.size.y / 2);
                secondPosition = possibleNavigationNodes[0].colliderBounds.bottomLeft;
                secondPosition = new Vector2(secondPosition.x - apostleCollisionHandler.CapsuleCollider2D.size.x / 2,
                    secondPosition.y + apostleCollisionHandler.CapsuleCollider2D.size.y / 2);
            }
        }
        return new List<NavigationNode>()
        {
            new NavigationNode(firstPosition, possibleNavigationNodes[0].transform,
                possibleNavigationNodes[0].type),
            new NavigationNode(secondPosition, possibleNavigationNodes[0].transform,
                possibleNavigationNodes[0].type)
        };
    }
}