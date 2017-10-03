using UnityEngine;

public class ApostleMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxAngle;
    [SerializeField] private LayerMask layerMaskForCollisions;

    private ApostleHorizontalMovement horizontalMovement;
    private ApostleVerticalMovement verticalMovement;
    private ApostleController apostleController;
    private BasicCollisionHandler apostleCollisions;

    void Start()
    {
        apostleCollisions = new BasicCollisionHandler();
        apostleCollisions.InitializeCollisions(this, maxAngle, layerMaskForCollisions);

        apostleController = ApostleController.GetInstance();

        horizontalMovement = ApostleHorizontalMovement.GetInstance();
        horizontalMovement.FillInstance(this, maxSpeed, acceleration, apostleCollisions, apostleController);

        verticalMovement = ApostleVerticalMovement.GetInstance();
        verticalMovement.FillInstance(this, apostleCollisions, apostleController);
    }

    void Update()
    {
        apostleCollisions.StartCollisions();
        horizontalMovement.StartMovement();
        horizontalMovement.PressMovementHandler();
        verticalMovement.StartMovement();
    }

    void FixedUpdate()
    {
        horizontalMovement.HoldMovementHandler();
        verticalMovement.HoldMovementHandler();
    }

    private void LateUpdate()
    {
        verticalMovement.ResolvePendencies();
    }
}