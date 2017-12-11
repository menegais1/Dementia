using UnityEngine;

public class RaycastHit2DPoints
{
    public RaycastHit2D midLeftDownwardRay;
    public RaycastHit2D midRightDownwardRay;
    public RaycastHit2D bottomMidDownwardRay;
    public RaycastHit2D midRightRightwardRay;
    public RaycastHit2D midLeftLeftwardRay;
}

public struct ColliderBounds
{
    public Vector2 midRight, midLeft;
    public Vector2 topRight, topLeft, topMid;
    public Vector2 bottomMid, bottomRight, bottomLeft;
    public Vector2 boundingBoxBottomY, boundingBoxTopY;
}

public class BasicCollisionHandler
{
    protected float maxAngle;
    protected float offsetForPerifericalRays;
    protected LayerMask layerMaskForCollisions;

    public float SurfaceAngle { get; protected set; }
    public Vector2 SurfaceNormal { get; protected set; }
    public float DistanceForJump { get; protected set; }

    public RaycastHit2DPoints RaycastHit2DPoints { get; protected set; }

    public ColliderBounds BoxColliderBounds
    {
        get { return boxColliderBounds; }
    }

    public CapsuleCollider2D CapsuleCollider2D
    {
        get { return capsuleCollider2D; }
        set { capsuleCollider2D = value; }
    }

    protected ColliderBounds boxColliderBounds;

    protected SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider2D;
    protected Rigidbody2D rigidbody2D;

    public BasicCollisionHandler(MonoBehaviour monoBehaviour, float maxAngle,
        LayerMask layerMaskForCollisions)
    {
        this.RaycastHit2DPoints = new RaycastHit2DPoints();
        this.maxAngle = maxAngle;
        this.CapsuleCollider2D = monoBehaviour.GetComponent<CapsuleCollider2D>();
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.DistanceForJump = 0.1f;
        this.spriteRenderer = monoBehaviour.GetComponent<SpriteRenderer>();
        this.layerMaskForCollisions = layerMaskForCollisions;
    }


    public virtual void StartCollisions(HorizontalMovementState horizontalMovementState,
        FacingDirection facingDirection)
    {
        UpdateColliderBounds();
        CastDownwardRays();
        CastLateralRays();
        CheckGroundForSlopes();
        CheckGroundForFall(horizontalMovementState);
        CheckBottom(facingDirection);
    }

    protected virtual void UpdateColliderBounds()
    {
        var center = CapsuleCollider2D.offset;
        offsetForPerifericalRays = CapsuleCollider2D.size.y / 2;
        boxColliderBounds.midLeft =
            CapsuleCollider2D.transform.TransformPoint(new Vector2(center.x - CapsuleCollider2D.size.x / 2,
                center.y));
        boxColliderBounds.midRight =
            CapsuleCollider2D.transform.TransformPoint(new Vector2(center.x + CapsuleCollider2D.size.x / 2,
                center.y));
        boxColliderBounds.bottomMid = CapsuleCollider2D.transform.TransformPoint(new Vector2(
            center.x,
            center.y - CapsuleCollider2D.size.y / 2));
        boxColliderBounds.topLeft =
            CapsuleCollider2D.transform.TransformPoint(new Vector2(center.x - CapsuleCollider2D.size.x / 2,
                center.y + CapsuleCollider2D.size.y / 2));

        boxColliderBounds.topRight =
            CapsuleCollider2D.transform.TransformPoint(new Vector2(center.x + CapsuleCollider2D.size.x / 2,
                center.y + CapsuleCollider2D.size.y / 2));
        boxColliderBounds.topMid =
            CapsuleCollider2D.transform.TransformPoint(new Vector2(center.x,
                center.y + CapsuleCollider2D.size.y / 2));
    }

    protected virtual void CastDownwardRays()
    {
        var direction = CapsuleCollider2D.transform.up * -1;

        RaycastHit2DPoints.midLeftDownwardRay =
            Physics2D.Raycast(BoxColliderBounds.midLeft, direction, 1f + offsetForPerifericalRays,
                layerMaskForCollisions.value);
        RaycastHit2DPoints.midRightDownwardRay =
            Physics2D.Raycast(BoxColliderBounds.midRight, direction, 1f + offsetForPerifericalRays,
                layerMaskForCollisions.value);
        RaycastHit2DPoints.bottomMidDownwardRay = Physics2D.Raycast(
            BoxColliderBounds.bottomMid, direction, 1f,
            layerMaskForCollisions.value);

        Debug.DrawRay(BoxColliderBounds.midLeft, direction, Color.red);
        Debug.DrawRay(BoxColliderBounds.bottomMid, direction,
            Color.green);
        Debug.DrawRay(BoxColliderBounds.midRight, direction, Color.blue);
    }

    protected virtual void CastLateralRays()
    {
        var direction = CapsuleCollider2D.transform.up * -1;

        RaycastHit2DPoints.midLeftLeftwardRay =
            Physics2D.Raycast(BoxColliderBounds.midLeft + (Vector2.down * (CapsuleCollider2D.size.y / 4)),
                CapsuleCollider2D.transform.right * -1);
        RaycastHit2DPoints.midRightRightwardRay =
            Physics2D.Raycast(BoxColliderBounds.midRight + (Vector2.down * (CapsuleCollider2D.size.y / 4)),
                CapsuleCollider2D.transform.right);

        Debug.DrawRay(BoxColliderBounds.midRight + (Vector2.down * (CapsuleCollider2D.size.y / 4)),
            CapsuleCollider2D.transform.right, Color.red);
        Debug.DrawRay(BoxColliderBounds.midLeft + (Vector2.down * (CapsuleCollider2D.size.y / 4)),
            CapsuleCollider2D.transform.right * -1,
            Color.green);
    }

    public RaycastHit2D CastRightwardRay(LayerMask layerMask)
    {
        return Physics2D.Raycast(BoxColliderBounds.midRight + (Vector2.down * (CapsuleCollider2D.size.y / 4)),
            CapsuleCollider2D.transform.right, 5, layerMask.value);
    }

    public virtual RaycastHit2D CastLeftwardRay(LayerMask layerMask)
    {
        return Physics2D.Raycast(BoxColliderBounds.midLeft + (Vector2.down * (CapsuleCollider2D.size.y / 4)),
            CapsuleCollider2D.transform.right * -1, 5, layerMask.value);
    }

    public virtual bool CheckGroundForJump(float distance)
    {
        if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
        {
            distance = RaycastHit2DPoints.bottomMidDownwardRay.distance <= distance ||
                       RaycastHit2DPoints.bottomMidDownwardRay.distance > 0.25f
                ? distance
                : RaycastHit2DPoints.bottomMidDownwardRay.distance;
        }
        return RaycastHit2DPoints.bottomMidDownwardRay.collider != null &&
               RaycastHit2DPoints.bottomMidDownwardRay.distance <= distance;
    }

    public virtual bool CheckGroundWithPerifericalRays(float distance, bool rightRay)
    {
        distance += offsetForPerifericalRays;
        if (rightRay)
        {
            if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
            {
                distance = RaycastHit2DPoints.midRightDownwardRay.distance <= distance ||
                           RaycastHit2DPoints.midRightDownwardRay.distance > 0.25f + offsetForPerifericalRays
                    ? distance
                    : RaycastHit2DPoints.midRightDownwardRay.distance;
            }
            return RaycastHit2DPoints.midRightDownwardRay.collider != null &&
                   RaycastHit2DPoints.midRightDownwardRay.distance <= distance;
        }

        if (!MathHelpers.Approximately(SurfaceAngle, 0, float.Epsilon))
        {
            distance = RaycastHit2DPoints.midLeftDownwardRay.distance <= distance ||
                       RaycastHit2DPoints.midLeftDownwardRay.distance > 0.25f + offsetForPerifericalRays
                ? distance
                : RaycastHit2DPoints.midLeftDownwardRay.distance;
        }
        return RaycastHit2DPoints.midLeftDownwardRay.collider != null &&
               RaycastHit2DPoints.midLeftDownwardRay.distance <= distance;
    }

    protected virtual void CheckGroundForSlopes()
    {
        if (!spriteRenderer.flipX &&
            RaycastHit2DPoints.midRightDownwardRay.collider != null)
        {
            var rotationAngle = Vector2.Angle(RaycastHit2DPoints.midRightDownwardRay.normal, Vector2.up);
            var surfaceNormal = RaycastHit2DPoints.midRightDownwardRay.normal;

            if (RaycastHit2DPoints.midRightDownwardRay.normal.x > 0 && rotationAngle > SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.midLeftDownwardRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.midLeftDownwardRay.normal;
            }
            else if (MathHelpers.Approximately(RaycastHit2DPoints.midRightDownwardRay.normal.x, 0, 0.05f) &&
                     rotationAngle < SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomMidDownwardRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.bottomMidDownwardRay.normal;
            }

            if (rotationAngle <= maxAngle)
            {
                SurfaceAngle = rotationAngle;
                SurfaceNormal = surfaceNormal;
            }
        }
        else if (spriteRenderer.flipX &&
                 RaycastHit2DPoints.midLeftDownwardRay.collider != null)
        {
            var rotationAngle = Vector2.Angle(RaycastHit2DPoints.midLeftDownwardRay.normal, Vector2.up);
            var surfaceNormal = RaycastHit2DPoints.midLeftDownwardRay.normal;
            if (RaycastHit2DPoints.midLeftDownwardRay.normal.x < 0 && rotationAngle > SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.midRightDownwardRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.midRightDownwardRay.normal;
            }
            else if (MathHelpers.Approximately(RaycastHit2DPoints.midLeftDownwardRay.normal.x, 0, 0.05f) &&
                     rotationAngle < SurfaceAngle)
            {
                rotationAngle = Vector2.Angle(RaycastHit2DPoints.bottomMidDownwardRay.normal, Vector2.up);
                surfaceNormal = RaycastHit2DPoints.bottomMidDownwardRay.normal;
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
        var ray = Physics2D.Raycast(BoxColliderBounds.bottomMid, Vector2.down, distance, layerMask.value);
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

    public void CheckBottom(FacingDirection facingDirection)
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Enemy", "Player"),
        };

        var raycastHit2Ds = new RaycastHit2D[1];

        Physics2D.Raycast(
            BoxColliderBounds.bottomMid, Vector2.down,
            contactFilter2D, raycastHit2Ds, 0.5f);
        if (raycastHit2Ds[0].collider == null) return;
        PhysicsHelpers.AddImpulseForce(30f, facingDirection != FacingDirection.Right, rigidbody2D);
    }

    public virtual void SetLayerForCollisions(string[] layersName)
    {
        layerMaskForCollisions = LayerMask.GetMask(layersName);
    }
}