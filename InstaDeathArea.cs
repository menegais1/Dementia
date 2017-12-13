using UnityEngine;

public class InstaDeathArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.PlayerStatusController.Die();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            var apostleManager = other.GetComponent<ApostleManager>();
            if (apostleManager != null)
            {
                apostleManager.Apostle.Die();
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.PlayerStatusController.Die();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            var apostleManager = other.GetComponent<ApostleManager>();
            if (apostleManager != null)
            {
                apostleManager.Apostle.Die();
            }
        }
    }
}