using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private PlayerStatusVariables playerStatusVariables;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        playerStatusVariables = other.GetComponent<PlayerManager>().PlayerStatusVariables;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        playerStatusVariables.canClimbObstacle = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        playerStatusVariables.canClimbObstacle = false;
    }
}