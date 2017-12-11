using UnityEngine;

public struct PatrolPointInfo
{
    public Transform transform;
    public Collider2D floorCollider2D;
    public Floor currentFloor;
    public TransitionFloor currentTransitionFloor;
    public TransitionFloorType transitionFloorType;

    public PatrolPointInfo(Transform transform, Collider2D collider2D, TransitionFloorType type)
    {
        this.transform = transform;
        this.floorCollider2D = collider2D;
        this.transitionFloorType = type;
        this.currentFloor = new Floor();
        this.currentTransitionFloor = new TransitionFloor();
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

    private float currentAggroTime;
    private Transform currentAim;
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
        currentAim = endPointTransform;

        if (GameManager.instance.NavigationAcessor == null)
        {
            GameManager.instance.NavigationAcessor =
                GameObject.FindGameObjectWithTag("Navigation").GetComponent<Navigation>();
        }
        navigation = GameManager.instance.NavigationAcessor;
        CreateTriggerArea();

        startPointPatrolInfo =
            new PatrolPointInfo(startPointTransform, startPointFloorCollider, startPointTransitionFloorType);

        endPointPatrolInfo =
            new PatrolPointInfo(endPointTransform, endPointFloorCollider, endPointTransitionFloorType);
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
            currentAim = startPointTransform;
            apostleStatusVariables.isAggroed = false;
        }

        apostleStatusVariables.isPatrolling = !apostleStatusVariables.isAggroed;

        if (apostleStatusVariables.isPatrolling)
        {
            if (MathHelpers.Approximately(transform.position.x, startPointTransform.position.x, 0.3f))
            {
                currentAim = endPointTransform;
            }
            else if (MathHelpers.Approximately(transform.position.x, endPointTransform.position.x, 0.3f))
            {
                currentAim = startPointTransform;
            }
        }

        if (!currentAim.Equals(null))
            movementDirectionValue =
                MovementDirection(currentAim.position);

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
            currentAim = playerManager.transform;

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
            currentAim = playerManager.transform;

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

        if (!currentAim.Equals(null))
        {
            currentAggroTime = Time.time + aggroTime;
            apostleStatusVariables.inAggroRange = false;
        }
    }
}