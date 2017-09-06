using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableObject : MonoBehaviour
{
    private Movement playerMovement;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerMovement = other.GetComponent<Movement>();
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (playerMovement != null)
            {
                playerMovement.CanClimbObject = true;

                if (playerMovement.StatusVariables.IsClimbingObject)
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
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (playerMovement != null)
            {
                playerMovement.CanClimbObject = false;
            }
        }
    }
}