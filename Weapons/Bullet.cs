using UnityEngine;

public class Bullet : MonoBehaviour
{   
    [SerializeField] private float force;
    [SerializeField] private WeaponType type;
    private Rigidbody2D rigidbody2D;


    public void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Shoot()
    {
        PhysicsHelpers.AddImpulseForce(force, this.rigidbody2D);
        DestroyBullet(3f);
    }

    public static Bullet InstantiateBullet(Vector3 position, Vector3 direction, GameObject bullet)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        var gameObject = Instantiate(bullet, position, rotation);
        return gameObject.GetComponent<Bullet>();
    }


    private void DestroyBullet(float time)
    {
        Destroy(gameObject, time);
    }
}