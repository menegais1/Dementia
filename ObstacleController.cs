
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        PlayerStatusVariables.canClimbObstacle = true;

        /*  if (PlayerStatusVariables.isClimbingObject)
          {
              if (!playerMovement.SnapToPositionRan)
              {
                  BoxCollider2D parentCollider = transform.parent.GetComponent<BoxCollider2D>();
                  Vector2 position = Vector2.zero;
                  if (transform.position.x > transform.parent.position.x)
                  {
                      position = new Vector2(parentCollider.bounds.max.x, parentCollider.bounds.max.y);
                  }
                  else
                  {
                      position = new Vector2(parentCollider.bounds.min.x, parentCollider.bounds.max.y);
                  }
                  playerMovement.snapToPositionObject(position);
              }
          }*/
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            PlayerStatusVariables.canClimbObstacle = false;
        }
    }
}