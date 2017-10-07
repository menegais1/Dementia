using UnityEngine;

public class ApostleMovement : MonoBehaviour
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

    void Start()
    {
        ApostleStatusVariables = new ApostleStatusVariables();

        ApostleCollisionHandler = new BasicCollisionHandler();
        ApostleCollisionHandler.InitializeCollisions(this, maxAngle, layerMaskForCollisions);

        ApostleInputHandler = new ApostleInputHandler();
        ApostleInputHandler.FillInstance(this, ApostleCollisionHandler, ApostleStatusVariables);

        ApostleController = new ApostleController();
        ApostleController.FillInstance(this, ApostleInputHandler);
        
        HorizontalMovement = new ApostleHorizontalMovement();
        HorizontalMovement.FillInstance(this, maxSpeed, acceleration, ApostleCollisionHandler, ApostleController,
            ApostleStatusVariables);

        VerticalMovement = new ApostleVerticalMovement();
        VerticalMovement.FillInstance(this, ApostleCollisionHandler, ApostleController, ApostleStatusVariables);
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