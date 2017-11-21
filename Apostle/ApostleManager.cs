using UnityEngine;

public class ApostleManager : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxAngle;
    [SerializeField] private LayerMask layerMaskForCollisions;

    public ApostleHorizontalMovement HorizontalMovement { get; private set; }
    public ApostleVerticalMovement VerticalMovement { get; private set; }
    public ApostleController ApostleController { get; private set; }
    public BasicCollisionHandler ApostleCollisionHandler { get; private set; }
    public ApostleStatusVariables ApostleStatusVariables { get; private set; }
    public ApostleInputHandler ApostleInputHandler { get; private set; }
    public ApostleGeneralController ApostleGeneralController { get; private set; }


    void Start()
    {
        ApostleStatusVariables = new ApostleStatusVariables();
        ApostleGeneralController = GetComponent<ApostleGeneralController>();
        
        ApostleCollisionHandler = new BasicCollisionHandler(this, maxAngle, layerMaskForCollisions);

        ApostleInputHandler = new ApostleInputHandler(this, ApostleCollisionHandler, ApostleStatusVariables);

        ApostleController = new ApostleController(this, ApostleInputHandler);

        HorizontalMovement = new ApostleHorizontalMovement(this, maxSpeed, acceleration, ApostleCollisionHandler,
            ApostleController,
            ApostleStatusVariables);

        VerticalMovement =
            new ApostleVerticalMovement(this, ApostleCollisionHandler, ApostleController, ApostleStatusVariables);
    }

    void Update()
    {
        ApostleCollisionHandler.StartCollisions(HorizontalMovement.HorizontalMovementState);
        HorizontalMovement.StartMovement();
        HorizontalMovement.PressMovementHandler();
        VerticalMovement.StartMovement();
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