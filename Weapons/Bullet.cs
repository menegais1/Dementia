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
    private ContactFilter2D contactFilter2DForBulletCheck;

    public void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        this.contactFilter2DForBulletCheck = new ContactFilter2D()
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Ground", "Ground Ignore", "Enemy", "Wall"),
        };
    }

    private void Update()
    {
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.CompareTag("Scenery") ||
                raycastHit2D.collider.CompareTag("Obstacle") || raycastHit2D.collider.CompareTag("Enemy"))
            {
                var layerColliderList = new RaycastHit2D[1];
                Physics2D.Linecast(lastBulletPosition,
                    rigidbody2D.position, contactFilter2DForBulletCheck, layerColliderList);
                var layerCollider = layerColliderList[0];
                if (layerCollider.collider != null &&
                    LayerMask.LayerToName(layerCollider.transform.gameObject.layer) == "Enemy")
                {
                    Destroy(this.gameObject);
                    var apostleManager = layerCollider.transform.GetComponent<ApostleManager>();
                    apostleManager.Apostle.TakeDamage(damage);
                    raycastHit2D = new RaycastHit2D();
                }
                else if ((layerCollider.collider != null &&
                          (LayerMask.LayerToName(layerCollider.transform.gameObject.layer) == "Ground" ||
                           LayerMask.LayerToName(layerCollider.transform.gameObject.layer) == "Ground Ignore" ||
                           LayerMask.LayerToName(layerCollider.transform.gameObject.layer) == "Wall")))
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
        var raycastHit2DList = new RaycastHit2D[1];
        Physics2D.Raycast(initialPosition, initialDirection,
            contactFilter2DForBulletCheck, raycastHit2DList, maxBulletDistance);
        raycastHit2D = raycastHit2DList[0];
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