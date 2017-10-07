using UnityEngine;

public class ApostleInputHandler
{
    private BasicCollisionHandler apostleCollisionHandler;
    private ApostleStatusVariables apostleStatusVariables;
    private MonoBehaviour monoBehaviour;

    private float horizontalMovement;

    public ApostleInputHandler()
    {
    }

    public void FillInstance(MonoBehaviour monoBehaviour, BasicCollisionHandler apostleCollisionHandler,
        ApostleStatusVariables apostleStatusVariables)
    {
        this.monoBehaviour = monoBehaviour;
        this.apostleCollisionHandler = apostleCollisionHandler;
        this.apostleStatusVariables = apostleStatusVariables;
        this.horizontalMovement = 1f;
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