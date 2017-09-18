using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;

public class PlayerCollisions
{
    private struct ColliderBounds
    {
        public Vector2 bottomRight, bottomLeft, bottomMid;
        public Vector2 topRight, topLeft;
    }

    private struct RaycastHit2DPoints
    {
        public RaycastHit2D bottomLeftRay;
        public RaycastHit2D bottomRightRay;
        public RaycastHit2D bottomMidRay;
    }

    private float maxAngle;

    public float SurfaceAngle { get; private set; }
    public Vector2 SurfaceNormal { get; private set; }
    public float DistanceForJump { get; private set; }

    private static PlayerCollisions instance;
    private RaycastHit2DPoints raycastHit2DPoints;
    private ColliderBounds boxColliderBounds;

    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidbody2D;

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

    public void InitializeCollisions(MonoBehaviour monoBehaviour, float maxAngle)
    {
        this.maxAngle = maxAngle;
        this.boxCollider2D = monoBehaviour.GetComponent<BoxCollider2D>();
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.DistanceForJump = 0.1f;
    }

    public void StartCollisions(LayerMask layerMask)
    {
        UpdateColliderBounds();
        CastRays(layerMask);
        CheckGroundForSlopes();
        CheckGroundForFall();
    }

    private void UpdateColliderBounds()
    {
        /*var bounds = boxCollider2D.bounds;

        boxColliderBounds.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        boxColliderBounds.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        boxColliderBounds.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        boxColliderBounds.topRight = new Vector2(bounds.max.x, bounds.max.y)*/
        
        var center = boxCollider2D.offset;

        boxColliderBounds.bottomLeft =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x - boxCollider2D.size.x / 2,
                center.y - boxCollider2D.size.y / 2));
        boxColliderBounds.bottomRight =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x + boxCollider2D.size.x / 2,
                center.y - boxCollider2D.size.y / 2));
        boxColliderBounds.topLeft =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x - boxCollider2D.size.x / 2,
                center.y + boxCollider2D.size.y / 2));
        boxColliderBounds.topRight =
            boxCollider2D.transform.TransformPoint(new Vector2(center.x + boxCollider2D.size.x / 2,
                center.y + boxCollider2D.size.y / 2));
        boxColliderBounds.bottomMid = boxCollider2D.transform.TransformPoint(new Vector2(
            center.x,
            center.y - boxCollider2D.size.y / 2));
    }

    private void CastRays(LayerMask layerMask)
    {
        var direction = boxCollider2D.transform.up * -1;

        raycastHit2DPoints.bottomLeftRay =
            Physics2D.Raycast(boxColliderBounds.bottomLeft, direction, 1f, layerMask.value);
        raycastHit2DPoints.bottomRightRay =
            Physics2D.Raycast(boxColliderBounds.bottomRight, direction, 1f, layerMask.value);
        raycastHit2DPoints.bottomMidRay = Physics2D.Raycast(
            boxColliderBounds.bottomMid, direction, 1f,
            layerMask.value);

        Debug.DrawRay(boxColliderBounds.bottomLeft, direction, Color.red);
        Debug.DrawRay(boxColliderBounds.bottomMid, direction,
            Color.green);
        Debug.DrawRay(boxColliderBounds.bottomRight, direction, Color.blue);
    }

    public bool CheckGroundForJump(float distance)
    {
        if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
        {
            distance = raycastHit2DPoints.bottomMidRay.distance <= distance ||
                       raycastHit2DPoints.bottomMidRay.distance > 0.25f
                ? distance
                : raycastHit2DPoints.bottomMidRay.distance;
        }
        return raycastHit2DPoints.bottomMidRay.collider != null &&
               raycastHit2DPoints.bottomMidRay.distance <= distance;
    }

    public bool CheckGroundWithPerifericalRays(float distance, bool rightRay)
    {
        if (rightRay)
        {
            if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
            {
                distance = raycastHit2DPoints.bottomRightRay.distance <= distance ||
                           raycastHit2DPoints.bottomRightRay.distance > 0.25f
                    ? distance
                    : raycastHit2DPoints.bottomRightRay.distance;
            }
            return raycastHit2DPoints.bottomRightRay.collider != null &&
                   raycastHit2DPoints.bottomRightRay.distance <= distance;
        }

        if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
        {
            distance = raycastHit2DPoints.bottomLeftRay.distance <= distance ||
                       raycastHit2DPoints.bottomLeftRay.distance > 0.25f
                ? distance
                : raycastHit2DPoints.bottomLeftRay.distance;
        }
        return raycastHit2DPoints.bottomLeftRay.collider != null &&
               raycastHit2DPoints.bottomLeftRay.distance <= distance;
    }

    private void CheckGroundForSlopes()
    {
        if (PlayerStatusVariables.facingDirection == FacingDirection.Right)
        {
            if (raycastHit2DPoints.bottomRightRay.collider == null) return;
            var rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomRightRay.normal, Vector2.up);
            if (rotationAngle <= maxAngle)
            {
                SurfaceAngle = rotationAngle;
                SurfaceNormal = raycastHit2DPoints.bottomRightRay.normal;
            }
        }
        else
        {
            if (raycastHit2DPoints.bottomLeftRay.collider == null) return;
            var rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomLeftRay.normal, Vector2.up);
            if (rotationAngle <= maxAngle)
            {
                SurfaceAngle = rotationAngle;
                SurfaceNormal = raycastHit2DPoints.bottomLeftRay.normal;
            }
        }
    }

    public void CheckGroundForFall()
    {
        if (!CheckGroundForJump(DistanceForJump))
        {
            if (CheckGroundWithPerifericalRays(DistanceForJump, true) &&
                !CheckGroundWithPerifericalRays(DistanceForJump, false))
            {
                PhysicsHelpers.AddImpulseForce(3f, false, rigidbody2D);
            }
            else if (CheckGroundWithPerifericalRays(DistanceForJump, false) &&
                     !CheckGroundWithPerifericalRays(DistanceForJump, true))
            {
                PhysicsHelpers.AddImpulseForce(3f, true, rigidbody2D);
            }
        }
    }
}