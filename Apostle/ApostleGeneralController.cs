using UnityEngine;

public class ApostleGeneralController : Enemy
{
    [SerializeField] private float reachArea;
    [SerializeField] private float baseDamage;
    private float currentLife;

    private void Start()
    {
        currentLife = maxLife;
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

    public override void TakeDamage(float damage)
    {
        currentLife -= damage;
        CheckLife();
    }

    public override void Die()
    {
        Destroy(this.gameObject);
    }

    public override void CheckLife()
    {
        if (currentLife <= 0)
        {
            Die();
        }
    }
}