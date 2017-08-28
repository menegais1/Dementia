using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{


    public string tag;
    private Movement playerMovement;
    public Collider2D adjacentCollider;
    public enum LadderType
    {
        LADDER = 0,
        BOTTOM_LADDER = 1,
        MIDDLE_LADDER = 2,
        TOP_LADDER = 3,
    }

    public LadderType ladder;
    // Use this for initialization

    void Start()
    {
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals(tag))
        {
            playerMovement = other.GetComponent<Movement>();
            playerMovement.isOnStairs = true;

        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals(tag))
        {
            if (playerMovement != null)
            {
                if (playerMovement.climbingStairs)
                {
                    playerMovement.snapToPosition(transform.position);
                    Physics2D.IgnoreCollision(adjacentCollider, playerMovement.boxCollider, true);
                }
            }
        }
    }



    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals(tag))
        {
            if (playerMovement != null)
            {
                playerMovement.isOnStairs = false;
                if (playerMovement.climbingStairs)
                {
                    playerMovement.resetVelocityY();
                    playerMovement.climbingStairs = false;
                    Physics2D.IgnoreCollision(adjacentCollider, playerMovement.boxCollider, false);

                }
            }

        }
    }

}
