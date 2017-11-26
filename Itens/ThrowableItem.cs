using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : Item
{
    [SerializeField] private float damagePerSecond;
    [SerializeField] private float area;
    [SerializeField] private float duration;
    [SerializeField] private float throwForce;
    [SerializeField] private float torque;

    private bool hasExploded;
    private float durationCurrentTime;
    private Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        var collider2Ds = GetComponents<Collider2D>();
        
        for (var i = 0; i < collider2Ds.Length; i++)
        {
            if (!collider2Ds[i].isTrigger)
            {
                collider2D = collider2Ds[i];
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Scenery") || other.CompareTag("Obstacle") && !hasExploded)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.angularVelocity = 0;
            spriteRenderer.enabled = false;
            hasExploded = true;
            durationCurrentTime = Time.time + duration;
            CoroutineManager.AddCoroutine(EffectCoroutine(), "EffectCoroutine");
        }
    }

    private IEnumerator EffectCoroutine()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Enemy")
        };

        var collider2Ds = new Collider2D[5];

        while (Time.time < durationCurrentTime && hasExploded)
        {
            var areaOfEffect = Physics2D.OverlapBox(transform.position, new Vector2(area, 1), 0,
                contactFilter2D, collider2Ds);


            if (areaOfEffect > 0)
            {
                for (var i = 0; i < collider2Ds.Length; i++)
                {
                    if (collider2Ds[i] != null)
                    {
                        PhysicsHelpers.IgnoreCollision(collider2D, collider2Ds[i], true);
                        var enemy = collider2Ds[i].GetComponent<Enemy>();
                        if (enemy != null)
                            enemy.TakeDamage(damagePerSecond * Time.fixedDeltaTime);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }

        CoroutineManager.DeleteCoroutine("EffectCoroutine");
        Destroy(this.gameObject);
    }


    public void Effect(Vector3 aimDirection)
    {
        PhysicsHelpers.AddImpulseForce(throwForce, aimDirection, rigidbody2D);

        rigidbody2D.AddTorque(torque);
    }
}