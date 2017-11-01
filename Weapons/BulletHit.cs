using UnityEngine;

public class BulletHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player")) return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Scenery") || other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log((other.gameObject.CompareTag("Scenery")));
        if (other.gameObject.CompareTag("Player")) return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Scenery") || other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        if (other.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Scenery") || other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
    }
}