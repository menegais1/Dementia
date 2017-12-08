using UnityEngine;

public class ApostleInputHandler : MonoBehaviour
{
    [SerializeField] private float aggroTime;
    [SerializeField] private Transform startPointTransform;
    [SerializeField] private Transform endPointTransform;

    private float currentAggroTime;

    private BoxCollider2D triggerArea;
    private Transform currentAim;

    private BasicCollisionHandler apostleCollisionHandler;
    private ApostleStatusVariables apostleStatusVariables;
    private MonoBehaviour monoBehaviour;


    private float movementDirectionValue;

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

        this.triggerArea = GetComponent<BoxCollider2D>();
        var mainCamera = Camera.main;
        var height = mainCamera.orthographicSize * 2;
        var width = mainCamera.aspect * height;
        triggerArea.size = new Vector2((width / 2) + (width / 12), height / 2);
        triggerArea.offset = new Vector2(width / 5, 0);
    }

    private void Update()
    {
        if (apostleStatusVariables.facingDirection == FacingDirection.Right && triggerArea.offset.x < 0)
        {
            triggerArea.offset = new Vector2(-triggerArea.offset.x, 0);
        }
        else if (apostleStatusVariables.facingDirection == FacingDirection.Left && triggerArea.offset.x > 0)
        {
            triggerArea.offset = new Vector2(-triggerArea.offset.x, 0);
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

        if (currentAim != null)
            movementDirectionValue =
                MovementDirection(currentAim.position);

        Debug.Log(currentAim != null ? currentAim.name : "");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var playerManager = other.GetComponent<PlayerManager>();
        if (CheckIfPlayerIsOnSight(playerManager.transform.position))
        {
            Debug.Log("teste");
            currentAim = playerManager.transform;
            apostleStatusVariables.isAggroed = true;
            apostleStatusVariables.inAggroRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;


        var playerManager = other.GetComponent<PlayerManager>();
        if (CheckIfPlayerIsOnSight(playerManager.transform.position))
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

        if (currentAim != null)
        {
            currentAggroTime = Time.time + aggroTime;
            apostleStatusVariables.inAggroRange = false;
        }
    }

    private bool CheckIfPlayerIsOnSight(Vector3 playerPosition)
    {
        var direction = playerPosition - transform.position;
        var startingPoint = playerPosition.x > transform.position.x
            ? apostleCollisionHandler.BoxColliderBounds.topRight
            : apostleCollisionHandler.BoxColliderBounds.topLeft;
        var ray = Physics2D.Raycast(startingPoint, new Vector2(direction.x, direction.y), triggerArea.size.x,
            LayerMask.GetMask("Ground", "Player"));
        return ray.collider != null && ray.collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }

    private float MovementDirection(Vector3 position)
    {
        return position.x > transform.position.x ? 1 : -1;
    }
}