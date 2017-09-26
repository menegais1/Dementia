using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    private float offsetForPerifericalRays;
    private LayerMask layerMaskForCollisions;

    public float SurfaceAngle { get; private set; }
    public Vector2 SurfaceNormal { get; private set; }
    public float DistanceForJump { get; private set; }

    private static PlayerCollisions instance;
    private RaycastHit2DPoints raycastHit2DPoints;
    private ColliderBounds boxColliderBounds;

    private CapsuleCollider2D capsuleCollider2D;
    private Rigidbody2D rigidbody2D;
    private HorizontalMovement horizontalMovement;

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

    public void InitializeCollisions(MonoBehaviour monoBehaviour, float maxAngle, LayerMask layerMaskForCollisions)
    {
        this.maxAngle = maxAngle;
        this.capsuleCollider2D = monoBehaviour.GetComponent<CapsuleCollider2D>();
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.DistanceForJump = 0.1f;
        this.horizontalMovement = HorizontalMovement.GetInstance();
        this.layerMaskForCollisions = layerMaskForCollisions;
    }

    public void StartCollisions()
    {
        UpdateColliderBounds();
        CastRays();
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

        var center = capsuleCollider2D.offset;
        offsetForPerifericalRays = capsuleCollider2D.size.y / 2;
        boxColliderBounds.bottomLeft =
            capsuleCollider2D.transform.TransformPoint(new Vector2(center.x - capsuleCollider2D.size.x / 2,
                center.y));
        boxColliderBounds.bottomRight =
            capsuleCollider2D.transform.TransformPoint(new Vector2(center.x + capsuleCollider2D.size.x / 2,
                center.y));
        boxColliderBounds.topLeft =
            capsuleCollider2D.transform.TransformPoint(new Vector2(center.x - capsuleCollider2D.size.x / 2,
                center.y + capsuleCollider2D.size.y / 2));
        boxColliderBounds.topRight =
            capsuleCollider2D.transform.TransformPoint(new Vector2(center.x + capsuleCollider2D.size.x / 2,
                center.y + capsuleCollider2D.size.y / 2));
        boxColliderBounds.bottomMid = capsuleCollider2D.transform.TransformPoint(new Vector2(
            center.x,
            center.y - capsuleCollider2D.size.y / 2));
    }

    private void CastRays()
    {
        var direction = capsuleCollider2D.transform.up * -1;

        raycastHit2DPoints.bottomLeftRay =
            Physics2D.Raycast(boxColliderBounds.bottomLeft, direction, 1f + offsetForPerifericalRays,
                layerMaskForCollisions.value);
        raycastHit2DPoints.bottomRightRay =
            Physics2D.Raycast(boxColliderBounds.bottomRight, direction, 1f + offsetForPerifericalRays,
                layerMaskForCollisions.value);
        raycastHit2DPoints.bottomMidRay = Physics2D.Raycast(
            boxColliderBounds.bottomMid, direction, 1f,
            layerMaskForCollisions.value);

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
        distance += offsetForPerifericalRays;
        if (rightRay)
        {
            if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
            {
                distance = raycastHit2DPoints.bottomRightRay.distance <= distance ||
                           raycastHit2DPoints.bottomRightRay.distance > 0.25f + offsetForPerifericalRays
                    ? distance
                    : raycastHit2DPoints.bottomRightRay.distance;
            }
            return raycastHit2DPoints.bottomRightRay.collider != null &&
                   raycastHit2DPoints.bottomRightRay.distance <= distance;
        }

        if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
        {
            distance = raycastHit2DPoints.bottomLeftRay.distance <= distance ||
                       raycastHit2DPoints.bottomLeftRay.distance > 0.25f + offsetForPerifericalRays
                ? distance
                : raycastHit2DPoints.bottomLeftRay.distance;
        }
        return raycastHit2DPoints.bottomLeftRay.collider != null &&
               raycastHit2DPoints.bottomLeftRay.distance <= distance;
    }

    private void CheckGroundForSlopes()
    {
        if (PlayerStatusVariables.facingDirection == FacingDirection.Right &&
            raycastHit2DPoints.bottomRightRay.collider != null)
        {
            var rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomRightRay.normal, Vector2.up);
            var surfaceNormal = raycastHit2DPoints.bottomRightRay.normal;

            if (raycastHit2DPoints.bottomRightRay.normal.x > 0 && rotationAngle > SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomLeftRay.normal, Vector2.up);
                surfaceNormal = raycastHit2DPoints.bottomLeftRay.normal;
            }
            else if (MathHelpers.Approximately(raycastHit2DPoints.bottomRightRay.normal.x, 0, float.Epsilon) &&
                     rotationAngle < SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomMidRay.normal, Vector2.up);
                surfaceNormal = raycastHit2DPoints.bottomMidRay.normal;
            }

            if (rotationAngle <= maxAngle)
            {
                SurfaceAngle = rotationAngle;
                SurfaceNormal = surfaceNormal;
            }
        }
        else if (PlayerStatusVariables.facingDirection == FacingDirection.Left &&
                 raycastHit2DPoints.bottomLeftRay.collider != null)
        {
            var rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomLeftRay.normal, Vector2.up);
            var surfaceNormal = raycastHit2DPoints.bottomLeftRay.normal;
            if (raycastHit2DPoints.bottomLeftRay.normal.x < 0 && rotationAngle > SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomRightRay.normal, Vector2.up);
                surfaceNormal = raycastHit2DPoints.bottomRightRay.normal;
            }
            else if (MathHelpers.Approximately(raycastHit2DPoints.bottomLeftRay.normal.x, 0, float.Epsilon) &&
                     rotationAngle < SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(raycastHit2DPoints.bottomMidRay.normal, Vector2.up);
                surfaceNormal = raycastHit2DPoints.bottomMidRay.normal;
            }

            if (rotationAngle <= maxAngle)
            {
                SurfaceAngle = rotationAngle;
                SurfaceNormal = surfaceNormal;
            }
        }
    }

    public bool CheckForLayerCollision(LayerMask layerMask, float distance)
    {
        var ray = Physics2D.Raycast(boxColliderBounds.bottomMid, Vector2.down, distance, layerMask.value);
        return ray.collider != null;
    }

    public void CheckGroundForFall()
    {
        if (!CheckGroundForJump(DistanceForJump) && MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon) &&
            (horizontalMovement.horizontalMovementState == HorizontalMovementState.Idle ||
             horizontalMovement.horizontalMovementState == HorizontalMovementState.CrouchIdle))
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

    public void SetLayerForCollisions(string[] layersName)
    {
        layerMaskForCollisions = LayerMask.GetMask(layersName);
    }
}