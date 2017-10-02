using UnityEngine;

public abstract class BasicPhysicsMovement
{
    protected MonoBehaviour monoBehaviour;
    protected Rigidbody2D rigidbody2D;
    protected CapsuleCollider2D capsuleCollider2D;
    protected SpriteRenderer spriteRenderer;

    public virtual void FillInstance(MonoBehaviour monoBehaviour)
    {
        this.monoBehaviour = monoBehaviour;
        this.rigidbody2D = monoBehaviour.GetComponent<Rigidbody2D>();
        this.capsuleCollider2D = monoBehaviour.GetComponent<CapsuleCollider2D>();
        this.spriteRenderer = monoBehaviour.GetComponent<SpriteRenderer>();
    }

    public abstract void StartMovement();
    public abstract void PressMovementHandler();
    public abstract void HoldMovementHandler();
    public abstract void ResolvePendencies();
}