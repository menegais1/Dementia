using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected bool worldStatus;
    protected string id;
    protected float maxLife = 100f;


    public abstract void TakeDamage(float damage);
    public abstract void Die();
    public abstract void CheckLife();
}