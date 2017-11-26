using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float reachArea;
    [SerializeField] private float baseDamage;
    [SerializeField] private float currentLife;
    [SerializeField] private float maxLife;

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

    public  void TakeDamage(float damage)
    {
        currentLife -= damage;
        CheckLife();
    }

    public  void Die()
    {
        Destroy(this.gameObject);
    }

    public void CheckLife()
    {
        if (currentLife <= 0)
        {
            Die();
        }
    }
}