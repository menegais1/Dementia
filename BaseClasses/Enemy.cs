using System.Collections;
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
            if (!apostleStatusVariables.isAttacking)
            {
                CoroutineManager.AddCoroutine(PrimaryAttack(), "PrimaryAttackCoroutine", this);
            }
        }
    }

    public IEnumerator PrimaryAttack()
    {
        apostleStatusVariables.isAttacking = true;
        yield return new WaitForSeconds(0.6f);
        RaycastHit2D ray = Physics2D.Raycast(
            transform.position,
            apostleStatusVariables.facingDirection == FacingDirection.Right ? Vector2.right : Vector2.right * -1,
            range, LayerMask.GetMask("Player"));

        if (ray.collider != null &&
            !Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player")))
        {
            var player = ray.collider.GetComponent<PlayerStatusController>();
            if (player != null)
            {
                player.TakeDamage(baseDamage);
            }
        }
        apostleController.RevokeControl(1f, true, ControlTypeToRevoke.AllMovement, this);
        apostleStatusVariables.isAttacking = false;
        CoroutineManager.DeleteCoroutine("PrimaryAttackCoroutine", this);
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