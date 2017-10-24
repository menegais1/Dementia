using UnityEngine;

public class RaycastHit2DPoints
{
    public RaycastHit2D bottomLeftRay;
    public RaycastHit2D bottomRightRay;
    public RaycastHit2D bottomMidRay;
    public RaycastHit2D bottomMidRightwardRay;
    public RaycastHit2D bottomMidLeftwardRay;
}


public class BasicCollisionHandler
{
    protected struct ColliderBounds
    {
        public Vector2 bottomRight, bottomLeft, bottomMid;
    }

    protected float maxAngle;
    protected float offsetForPerifericalRays;
    protected LayerMask layerMaskForCollisions;

    public float SurfaceAngle { get; protected set; }
    public Vector2 SurfaceNormal { get; protected set; }
    public float DistanceForJump { get; protected set; }

    public RaycastHit2DPoints RaycastHit2DPoints { get; protected set; }
    protected ColliderBounds boxColliderBounds;

    protected SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D capsuleCollider2D;
    protected Rigidbody2D rigidbody2D;

    public BasicCollisionHandler(MonoBehaviour monoBehaviour, float maxAngle,
        LayerMask layerMaskForCollisions)
    {
        this.RaycastHit2DPoints = new RaycastHit2DPoints();
        this.maxAngle = maxAngle;
        this.capsuleCollider2D = monoBehaviour.GetComponent<CapsuleCollider2D>();
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.DistanceForJump = 0.1f;
        this.spriteRenderer = monoBehaviour.GetComponent<SpriteRenderer>();
        this.layerMaskForCollisions = layerMaskForCollisions;
    }


    public virtual void StartCollisions(HorizontalMovementState horizontalMovementState)
    {
        UpdateColliderBounds();
        CastDownwardRays();
        CastLateralRays();
        CheckGroundForSlopes();
        CheckGroundForFall(horizontalMovementState);
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

    protected virtual void CastDownwardRays()
    {
        var direction = capsuleCollider2D.transform.up * -1;

        RaycastHit2DPoints.bottomLeftRay =
            Physics2D.Raycast(boxColliderBounds.bottomLeft, direction, 1f + offsetForPerifericalRays,
                layerMaskForCollisions.value);
        RaycastHit2DPoints.bottomRightRay =
            Physics2D.Raycast(boxColliderBounds.bottomRight, direction, 1f + offsetForPerifericalRays,
                layerMaskForCollisions.value);
        RaycastHit2DPoints.bottomMidRay = Physics2D.Raycast(
            boxColliderBounds.bottomMid, direction, 1f,
            layerMaskForCollisions.value);

        Debug.DrawRay(boxColliderBounds.bottomLeft, direction, Color.red);
        Debug.DrawRay(boxColliderBounds.bottomMid, direction,
            Color.green);
        Debug.DrawRay(boxColliderBounds.bottomRight, direction, Color.blue);
    }

    protected virtual void CastLateralRays()
    {
        var direction = capsuleCollider2D.transform.up * -1;

        RaycastHit2DPoints.bottomMidLeftwardRay =
            Physics2D.Raycast(boxColliderBounds.bottomLeft + (Vector2.down * (capsuleCollider2D.size.y / 4)),
                capsuleCollider2D.transform.right * -1);
        RaycastHit2DPoints.bottomMidRightwardRay =
            Physics2D.Raycast(boxColliderBounds.bottomRight + (Vector2.down * (capsuleCollider2D.size.y / 4)),
                capsuleCollider2D.transform.right);

        Debug.DrawRay(boxColliderBounds.bottomRight + (Vector2.down * (capsuleCollider2D.size.y / 4)),
            capsuleCollider2D.transform.right, Color.red);
        Debug.DrawRay(boxColliderBounds.bottomLeft + (Vector2.down * (capsuleCollider2D.size.y / 4)),
            capsuleCollider2D.transform.right * -1,
            Color.green);
    }

    public virtual bool CheckGroundForJump(float distance)
    {
        if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
        {
            distance = RaycastHit2DPoints.bottomMidRay.distance <= distance ||
                       RaycastHit2DPoints.bottomMidRay.distance > 0.25f
                ? distance
                : RaycastHit2DPoints.bottomMidRay.distance;
        }
        return RaycastHit2DPoints.bottomMidRay.collider != null &&
               RaycastHit2DPoints.bottomMidRay.distance <= distance;
    }

    public virtual bool CheckGroundWithPerifericalRays(float distance, bool rightRay)
    {
        distance += offsetForPerifericalRays;
        if (rightRay)
        {
            if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
            {
                distance = RaycastHit2DPoints.bottomRightRay.distance <= distance ||
                           RaycastHit2DPoints.bottomRightRay.distance > 0.25f + offsetForPerifericalRays
                    ? distance
                    : RaycastHit2DPoints.bottomRightRay.distance;
            }
            return RaycastHit2DPoints.bottomRightRay.collider != null &&
                   RaycastHit2DPoints.bottomRightRay.distance <= distance;
        }

        if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
        {
            distance = RaycastHit2DPoints.bottomLeftRay.distance <= distance ||
                       RaycastHit2DPoints.bottomLeftRay.distance > 0.25f + offsetForPerifericalRays
                ? distance
                : RaycastHit2DPoints.bottomLeftRay.distance;
        }
        return RaycastHit2DPoints.bottomLeftRay.collider != null &&
               RaycastHit2DPoints.bottomLeftRay.distance <= distance;
    }

    protected virtual void CheckGroundForSlopes()
    {
        if (!spriteRenderer.flipX &&
            RaycastHit2DPoints.bottomRightRay.collider != null)
        {
            var rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomRightRay.normal, Vector2.up);
            var surfaceNormal = RaycastHit2DPoints.bottomRightRay.normal;

            if (RaycastHit2DPoints.bottomRightRay.normal.x > 0 && rotationAngle > SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomLeftRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.bottomLeftRay.normal;
            }
            else if (MathHelpers.Approximately(RaycastHit2DPoints.bottomRightRay.normal.x, 0, 0.05f) &&
                     rotationAngle < SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomMidRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.bottomMidRay.normal;
            }

            if (rotationAngle <= maxAngle)
            {
                SurfaceAngle = rotationAngle;
                SurfaceNormal = surfaceNormal;
            }
        }
        else if (spriteRenderer.flipX &&
                 RaycastHit2DPoints.bottomLeftRay.collider != null)
        {
            var rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomLeftRay.normal, Vector2.up);
            var surfaceNormal = RaycastHit2DPoints.bottomLeftRay.normal;
            if (RaycastHit2DPoints.bottomLeftRay.normal.x < 0 && rotationAngle > SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomRightRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.bottomRightRay.normal;
            }
            else if (MathHelpers.Approximately(RaycastHit2DPoints.bottomLeftRay.normal.x, 0, 0.05f) &&
                     rotationAngle < SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomMidRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.bottomMidRay.normal;
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

    public void CheckGroundForFall(HorizontalMovementState horizontalMovementState)
    {
        if (!CheckGroundForJump(DistanceForJump) && MathHelpers.Approximately(SurfaceAngle, 0, 5f) &&
            (horizontalMovementState == HorizontalMovementState.Idle ||
             horizontalMovementState == HorizontalMovementState.CrouchIdle) &&
            MathHelpers.Approximately(rigidbody2D.velocity.magnitude, 0, 0.1f))
        {
            if (CheckGroundWithPerifericalRays(DistanceForJump, true) &&
                !CheckGroundWithPerifericalRays(DistanceForJump, false))
            {
                PhysicsHelpers.AddImpulseForce(30f, false, rigidbody2D);
            }
            else if (CheckGroundWithPerifericalRays(DistanceForJump, false) &&
                     !CheckGroundWithPerifericalRays(DistanceForJump, true))
            {
                PhysicsHelpers.AddImpulseForce(30f, true, rigidbody2D);
            }
        }
    }

    public virtual void SetLayerForCollisions(string[] layersName)
    {
        layerMaskForCollisions = LayerMask.GetMask(layersName);
    }
}