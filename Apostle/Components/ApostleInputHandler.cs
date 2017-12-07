using UnityEngine;

public class ApostleInputHandler : MonoBehaviour
{
    private BasicCollisionHandler apostleCollisionHandler;
    private ApostleStatusVariables apostleStatusVariables;
    private MonoBehaviour monoBehaviour;

    private float horizontalMovement;
    private BoxCollider2D triggerArea;
    private PlayerStatusController player;

    private void Start()
    {
        this.triggerArea = GetComponent<BoxCollider2D>();
        var mainCamera = Camera.main;
        var height = mainCamera.orthographicSize * 2;
        var width = mainCamera.aspect * height;
        triggerArea.size = new Vector2((width / 2) + (width / 12), height / 2);
        triggerArea.offset = new Vector2(width / 5, 0);

        var apostleManager = GetComponent<ApostleManager>();
        this.apostleCollisionHandler = apostleManager.ApostleCollisionHandler;
        this.apostleStatusVariables = apostleManager.ApostleStatusVariables;
        this.horizontalMovement = 1f;
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
    }

    public float GetHorizontalInput()
    {
        //Quando o Jogador estiver na escada, o adjacent collider é movido para a layer IGNORE RAYCAT, logo, o inimigo não detectará essa layer
        var raycastPoints = apostleCollisionHandler.RaycastHit2DPoints;

        if (raycastPoints.bottomMidRightwardRay.collider != null &&
            raycastPoints.bottomMidRightwardRay.distance < 1f &&
            MathHelpers.Approximately(raycastPoints.bottomMidRightwardRay.transform.eulerAngles.z, 0, float.Epsilon) &&
            apostleStatusVariables.facingDirection == FacingDirection.Right)
        {
            horizontalMovement = -1f;
        }
        else if (raycastPoints.bottomMidLeftwardRay.collider != null &&
                 raycastPoints.bottomMidLeftwardRay.distance < 1f &&
                 MathHelpers.Approximately(raycastPoints.bottomMidLeftwardRay.transform.eulerAngles.z, 0,
                     float.Epsilon) &&
                 apostleStatusVariables.facingDirection == FacingDirection.Left)
        {
            horizontalMovement = 1f;
        }
        return horizontalMovement;
    }
}