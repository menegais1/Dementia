using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCollisions
{
    private struct ColliderBounds
    {
        public Vector2 bottomRight, bottomLeft;
        public Vector2 topRight, topLeft;
    }

    private struct RaycastHit2DPoints
    {
        public RaycastHit2D bottomLeftRay;
        public RaycastHit2D bottomRightRay;
        public RaycastHit2D bottomMidRay;
    }

    private static PlayerCollisions instance;
    private RaycastHit2DPoints raycastHit2DPoints;
    private ColliderBounds boxColliderBounds;

    private BoxCollider2D boxCollider2D;

    public static PlayerCollisions GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerCollisions();
        }

        return instance;
    }

    private PlayerCollisions()
    {
    }

    public void InitializeCollisions(MonoBehaviour monoBehaviour)
    {
        this.boxCollider2D = monoBehaviour.GetComponent<BoxCollider2D>();
    }

    public void StartCollisions(LayerMask layerMask)
    {
        UpdateColliderBounds();
        CastRays(layerMask);
    }

    private void UpdateColliderBounds()
    {
        var bounds = boxCollider2D.bounds;

        boxColliderBounds.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        boxColliderBounds.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        boxColliderBounds.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        boxColliderBounds.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CastRays(LayerMask layerMask)
    {
        raycastHit2DPoints.bottomLeftRay =
            Physics2D.Raycast(boxColliderBounds.bottomLeft, Vector2.down, 1f, layerMask.value);
        raycastHit2DPoints.bottomRightRay =
            Physics2D.Raycast(boxColliderBounds.bottomRight, Vector2.down, 1f, layerMask.value);
        raycastHit2DPoints.bottomMidRay = Physics2D.Raycast(
            boxColliderBounds.bottomLeft + (Vector2.right * boxCollider2D.bounds.size.x / 2), Vector2.down, 1f,
            layerMask.value);

        Debug.DrawRay(boxColliderBounds.bottomLeft, Vector2.down, Color.red);
        Debug.DrawRay(boxColliderBounds.bottomLeft + (Vector2.right * boxCollider2D.bounds.size.x / 2), Vector2.down,
            Color.green);
        Debug.DrawRay(boxColliderBounds.bottomRight, Vector2.down, Color.blue);
    }

    public bool CheckGroundForJump(float distance)
    {
        return raycastHit2DPoints.bottomMidRay.collider != null &&
               raycastHit2DPoints.bottomMidRay.distance <= distance;
    }
}