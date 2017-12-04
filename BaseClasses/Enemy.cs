using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float reachArea;
    [SerializeField] private float baseDamage;
    [SerializeField] private float currentLife;
    [SerializeField] private float maxLife;

    private int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public float CurrentLife
    {
        get { return currentLife; }
        set { currentLife = value; }
    }

    private void Start()
    {
        CurrentLife = maxLife;
    }


    public void primaryAttack()
    {
    }

    public void secondaryAttack()
    {
    }

    public void terciaryAttack()
    {
    }

    public void patrol()
    {
    }

    public void findPlayer()
    {
    }

    public void TakeDamage(float damage)
    {
        CurrentLife -= damage;
        if (CurrentLife <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}