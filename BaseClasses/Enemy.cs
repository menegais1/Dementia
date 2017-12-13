using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float baseDamage;
    [SerializeField] private float currentLife;
    [SerializeField] private float maxLife;

    private ApostleStatusVariables apostleStatusVariables;
    private ApostleController apostleController;

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
        var apostleManager = GetComponent<ApostleManager>();

        apostleStatusVariables = apostleManager.ApostleStatusVariables;
        apostleController = apostleManager.ApostleController;
    }

    private void Update()
    {
        apostleController.CheckForCombatInput();

        if (apostleController.AttackPress)
        {
            PrimaryAttack();
        }
    }

    public void PrimaryAttack()
    {
        RaycastHit2D ray = Physics2D.Raycast(
            transform.position,
            apostleStatusVariables.facingDirection == FacingDirection.Right ? Vector2.right : Vector2.right * -1,
            range, LayerMask.GetMask("Player"));

        if (ray.collider != null)
        {
            var enemy = ray.collider.GetComponent<PlayerStatusController>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);
            }
        }
        apostleController.RevokeControl(0.5f, true, ControlTypeToRevoke.AllMovement, this);
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