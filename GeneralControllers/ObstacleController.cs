using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private PlayerStatusVariables playerStatusVariables;
    private ApostleStatusVariables apostleStatusVariables;
    private BoxCollider2D collider2D;

    public BoxCollider2D Collider2D
    {
        get { return collider2D; }
    }


    private void Awake()
    {
        collider2D = GetComponentsInParent<BoxCollider2D>()[1];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Enemy")) || other.isTrigger) return;

        if (other.CompareTag("Player"))
            playerStatusVariables = other.GetComponent<PlayerManager>().PlayerStatusVariables;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables = other.GetComponent<ApostleManager>().ApostleStatusVariables;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Enemy")) || other.isTrigger) return;
        if (other.CompareTag("Player"))
            playerStatusVariables.canClimbObstacle = true;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables.canClimbObstacle = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Enemy")) || other.isTrigger) return;
        if (other.CompareTag("Player"))
            playerStatusVariables.canClimbObstacle = false;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables.canClimbObstacle = false;
    }
}