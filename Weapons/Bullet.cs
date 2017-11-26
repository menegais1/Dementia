using System.Runtime.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float maxBulletDistance;
    [SerializeField] private WeaponType type;
    private float damage;
    private Rigidbody2D rigidbody2D;

    private Vector3 initialPosition;
    private Vector3 initialDirection;

    private Vector2 lastBulletPosition;
    private RaycastHit2D raycastHit2D;

    public void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.CompareTag("Scenery") ||
                raycastHit2D.collider.CompareTag("Obstacle") || raycastHit2D.collider.CompareTag("Enemy"))
            {
                var layerCollider = Physics2D.Linecast(lastBulletPosition,
                    rigidbody2D.position, LayerMask.GetMask("Enemy", "Ground"));
                if (layerCollider.collider != null &&
                    LayerMask.LayerToName(layerCollider.transform.gameObject.layer) == "Enemy")
                {
                    Destroy(this.gameObject);
                    var apostleManager = layerCollider.transform.GetComponent<ApostleManager>();
                    apostleManager.Enemy.TakeDamage(damage);
                    raycastHit2D = new RaycastHit2D();
                }
                else if (layerCollider.collider != null &&
                         LayerMask.LayerToName(layerCollider.transform.gameObject.layer) == "Ground")
                {
                    Destroy(this.gameObject);
                    raycastHit2D = new RaycastHit2D();
                }
            }
        }


        lastBulletPosition = rigidbody2D.position;
    }

    public void Shoot()
    {
        raycastHit2D = Physics2D.Raycast(initialPosition, initialDirection, maxBulletDistance,
            LayerMask.GetMask("Enemy", "Ground"));
        PhysicsHelpers.AddImpulseForce(force, this.rigidbody2D);
        lastBulletPosition = rigidbody2D.position;
        if (raycastHit2D.collider == null)
        {
            DestroyBullet(2f);
        }
    }

    public static Bullet InstantiateBullet(Vector3 position, Vector3 direction, GameObject bullet, float damage)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        var gameObject = Instantiate(bullet, position, rotation);
        var instantiateBullet = gameObject.GetComponent<Bullet>();
        instantiateBullet.initialPosition = position;
        instantiateBullet.initialDirection = direction;
        instantiateBullet.damage = damage;
        return instantiateBullet;
    }


    private void DestroyBullet(float time)
    {
        Destroy(gameObject, time);
    }
}