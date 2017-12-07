using UnityEngine;

public class ApostleManager : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxAngle;
    [SerializeField] private LayerMask layerMaskForCollisions;
    [SerializeField] private float climbingLadderSmoothness;
    [SerializeField] private float climbingObstacleSmoothness;
    [SerializeField] private float climbLadderVelocity;
    [SerializeField] private float minimumFallingDistanceForDamage;
    [SerializeField] private float minimumDamageForFalling;

    public ApostleHorizontalMovement HorizontalMovement { get; private set; }
    public ApostleVerticalMovement VerticalMovement { get; private set; }
    public ApostleController ApostleController { get; private set; }
    public BasicCollisionHandler ApostleCollisionHandler { get; private set; }
    public ApostleStatusVariables ApostleStatusVariables { get; private set; }
    public ApostleInputHandler ApostleInputHandler { get; private set; }
    public Enemy Apostle { get; private set; }


    void Start()
    {
        ApostleStatusVariables = GetComponent<ApostleStatusVariables>();
        Apostle = GetComponent<Enemy>();
        ApostleInputHandler = GetComponent<ApostleInputHandler>();

        ApostleCollisionHandler =
            new BasicCollisionHandler(this, maxAngle, layerMaskForCollisions);


        ApostleController = new ApostleController(this, ApostleInputHandler);

        HorizontalMovement = new ApostleHorizontalMovement(this, maxSpeed, acceleration, ApostleCollisionHandler,
            ApostleController,
            ApostleStatusVariables);

        VerticalMovement =
            new ApostleVerticalMovement(this, climbingLadderSmoothness, climbingObstacleSmoothness, climbLadderVelocity,
                minimumFallingDistanceForDamage, minimumDamageForFalling, ApostleCollisionHandler, ApostleController,
                ApostleStatusVariables, Apostle);
    }

    void Update()
    {
        ApostleCollisionHandler.StartCollisions(HorizontalMovement.HorizontalMovementState);
        HorizontalMovement.StartMovement();
        HorizontalMovement.PressMovementHandler();
        VerticalMovement.StartMovement();
        VerticalMovement.PressMovementHandler();

    }

    void FixedUpdate()
    {
        HorizontalMovement.HoldMovementHandler();
        VerticalMovement.HoldMovementHandler();
    }

    private void LateUpdate()
    {
        VerticalMovement.ResolvePendencies();
    }
}