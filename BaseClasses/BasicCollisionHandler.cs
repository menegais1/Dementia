using UnityEngine;

public class BasicCollisionHandler
{
    protected struct ColliderBounds
    {
        public Vector2 bottomRight, bottomLeft, bottomMid;
    }

    protected struct RaycastHit2DPoints
    {
        public RaycastHit2D bottomLeftRay;
        public RaycastHit2D bottomRightRay;
        public RaycastHit2D bottomMidRay;
    }

    protected float maxAngle;
    protected float offsetForPerifericalRays;
    protected LayerMask layerMaskForCollisions;

    public float SurfaceAngle { get; protected set; }
    public Vector2 SurfaceNormal { get; protected set; }
    public float DistanceForJump { get; protected set; }

    protected static BasicCollisionHandler instance;
    protected RaycastHit2DPoints raycastHit2DPoints;
    protected ColliderBounds boxColliderBounds;

    protected SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D capsuleCollider2D;
    protected Rigidbody2D rigidbody2D;
    protected PlayerHorizontalMovement horizontalMovement;


    public virtual void InitializeCollisions(MonoBehaviour monoBehaviour, float maxAngle,
        LayerMask layerMaskForCollisions)
    {
        this.maxAngle = maxAngle;
        this.capsuleCollider2D = monoBehaviour.GetComponent<CapsuleCollider2D>();
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.DistanceForJump = 0.1f;
        this.horizontalMovement = PlayerHorizontalMovement.GetInstance();
        this.spriteRenderer = monoBehaviour.GetComponent<SpriteRenderer>();
        this.layerMaskForCollisions = layerMaskForCollisions;
    }

    public virtual void StartCollisions()
    {
        UpdateColliderBounds();
        CastRays();
        CheckGroundForSlopes();
        CheckGroundForFall();
    }

    protected virtual void UpdateColliderBounds()
    {
        var center = capsuleCollider2D.offset;
        offsetForPerifericalRays = capsuleCollider2D.size.y / 2;
        boxColliderBounds.bottomLeft =
            capsuleCollider2D.transform.TransformPoint(new Vector2(center.x - capsuleCollider2D.size.x / 2,
                center.y));
        boxColliderBounds.bottomRight =
            capsuleCollider2D.transform.TransformPoint(new Vector2(center.x + capsuleCollider2D.size.x / 2,
                center.y));
        boxColliderBounds.bottomMid = capsuleCollider2D.transform.TransformPoint(new Vector2(
            center.x,
            center.y - capsuleCollider2D.size.y / 2));
    }

    protected virtual void CastRays()
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

    public virtual bool CheckGroundForJump(float distance)
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

    public virtual bool CheckGroundWithPerifericalRays(float distance, bool rightRay)
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

    protected virtual void CheckGroundForSlopes()
    {
        if (!spriteRenderer.flipX &&
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
        else if (spriteRenderer.flipX &&
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

    public virtual bool CheckForLayerCollision(LayerMask layerMask, float distance)
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

    public virtual void SetLayerForCollisions(string[] layersName)
    {
        layerMaskForCollisions = LayerMask.GetMask(layersName);
    }
}