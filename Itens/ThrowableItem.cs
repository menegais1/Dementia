using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : Item
{
    [SerializeField] private float damage;
    [SerializeField] private float area;
    [SerializeField] private float duration;
    [SerializeField] private float throwForce;
    [SerializeField] private bool imediate;

    private bool hasExploded;
    private float durationCurrentTime;
    private Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private Collider2D triggerCollider2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        var collider2Ds = GetComponents<Collider2D>();

        for (var i = 0; i < collider2Ds.Length; i++)
        {
            if (!collider2Ds[i].isTrigger)
            {
                triggerCollider2D = collider2Ds[i];
                break;
            }
        }
    }

    private void Update()
    {
        if (Time.time >= durationCurrentTime && imediate)
        {
            hasExploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Enemy", "Player")
        };

        var collider2Ds = new Collider2D[5];

        if (hasExploded)
        {
            var areaOfEffect = Physics2D.OverlapCircle(transform.position, area,
                contactFilter2D, collider2Ds);
            if (areaOfEffect > 0)
            {
                for (var i = 0; i < collider2Ds.Length; i++)
                {
                    if (collider2Ds[i] != null)
                    {
                        PhysicsHelpers.IgnoreCollision(triggerCollider2D, collider2Ds[i], true);
                        var enemy = collider2Ds[i].GetComponent<Enemy>();
                        if (enemy != null)
                            enemy.TakeDamage(damage);
                        else
                        {
                            Debug.Log("teste");
                            var player = collider2Ds[i].GetComponent<PlayerStatusController>();
                            if (player != null)
                                player.TakeDamage(damage);
                        }
                    }
                }
            }
        }
        PhysicsHelpers.IgnoreLayerCollision(LayerMask.NameToLayer("Item"), LayerMask.NameToLayer("Enemy"), false);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RaycastHit2D ground = new RaycastHit2D();
        if (!hasExploded)
        {
            ground = Physics2D.Raycast(transform.position, Vector2.down, 0.3f,
                LayerMask.GetMask("Ground", "Ground Ignore"));
        }
        if (!imediate)
        {
            if ((other.CompareTag("Scenery") || other.CompareTag("Obstacle")) && !hasExploded &&
                ground.collider != null)
            {
                var angle = Vector2.Angle(ground.normal, Vector2.up);

                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.angularVelocity = 0;
                rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                spriteRenderer.enabled = false;
                hasExploded = true;
                durationCurrentTime = Time.time + duration;
                CoroutineManager.AddCoroutine(EffectCoroutine(angle, ground.point), "EffectCoroutine");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        RaycastHit2D ground = new RaycastHit2D();
        if (!hasExploded)
        {
            ground = Physics2D.Raycast(transform.position, Vector2.down, 0.3f,
                LayerMask.GetMask("Ground", "Ground Ignore"));
        }
        if (!imediate)
        {
            if ((other.CompareTag("Scenery") || other.CompareTag("Obstacle")) && !hasExploded &&
                ground.collider != null)
            {
                var angle = Vector2.Angle(ground.normal, Vector2.up);

                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.angularVelocity = 0;
                rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                spriteRenderer.enabled = false;
                hasExploded = true;
                durationCurrentTime = Time.time + duration;
                CoroutineManager.AddCoroutine(EffectCoroutine(angle, ground.point), "EffectCoroutine");
            }
        }
    }

    private IEnumerator EffectCoroutine(float areaOfEffectAngle, Vector3 position)
    {
        var contactFilter2D = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Enemy", "Player")
        };

        var collider2Ds = new Collider2D[5];


        while (Time.time < durationCurrentTime && hasExploded)
        {
            var areaOfEffect = Physics2D.OverlapBox(position, new Vector2(area, 1), areaOfEffectAngle,
                contactFilter2D, collider2Ds);


            if (areaOfEffect > 0)
            {
                for (var i = 0; i < collider2Ds.Length; i++)
                {
                    if (collider2Ds[i] != null)
                    {
                        PhysicsHelpers.IgnoreCollision(triggerCollider2D, collider2Ds[i], true);
                        var enemy = collider2Ds[i].GetComponent<Enemy>();
                        if (enemy != null)
                            enemy.TakeDamage(damage * Time.fixedDeltaTime);
                        else
                        {
                            var player = collider2Ds[i].GetComponent<PlayerStatusController>();
                            if (player != null)
                                player.TakeDamage(damage * Time.fixedDeltaTime);
                        }
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }

        CoroutineManager.DeleteCoroutine("EffectCoroutine");
        Destroy(this.gameObject);
    }


    public void Effect(Vector3 aimDirection, bool imediate)
    {
        PhysicsHelpers.AddImpulseForce(throwForce, aimDirection, rigidbody2D);

        if (imediate)
        {
            durationCurrentTime = Time.time + duration;
            PhysicsHelpers.IgnoreLayerCollision(LayerMask.NameToLayer("Item"), LayerMask.NameToLayer("Enemy"),
                true);
        }
    }
}