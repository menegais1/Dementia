using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    public enum LadderType
    {
        LADDER = 0,
        BOTTOM_LADDER = 1,
        MIDDLE_LADDER = 2,
        TOP_LADDER = 3,
    }


    public string tag;
    private Movement playerMovement;
    public Collider2D adjacentCollider;
    public LadderType ladder;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals(tag))
        {

            playerMovement = other.GetComponent<Movement>();
            if (ladder != LadderType.LADDER)
            {
                adjacentCollider = transform.parent.GetComponent<LadderController>().adjacentCollider;
            }
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals(tag))
        {
            if (ladder == LadderType.BOTTOM_LADDER || ladder == LadderType.TOP_LADDER)
            {
                playerMovement.setCanClimbStairs(true);

                if (playerMovement.StatusVariables.IsClimbingStairs)
                {
                    if (!playerMovement.SnapToPositionRan)
                    {
                        Vector2 position = transform.GetChild(0).position;
                        playerMovement.snapToPositionStairs(position);
                        playerMovement.ignoreCollision(adjacentCollider, true);

                    }
                }
                else if (playerMovement.LeaveStairs)
                {
                    playerMovement.ignoreCollision(adjacentCollider, false);
                    playerMovement.LeaveStairs = false;
                }
            }

            //if (ladder == LadderType.LADDER)
            //{
            //    playerMovement.ignoreCollision(adjacentCollider, true);

            //}

        }
    }



    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals(tag))
        {
            if (playerMovement != null)
            {
                if (ladder == LadderType.BOTTOM_LADDER || ladder == LadderType.TOP_LADDER)
                {
                    playerMovement.setCanClimbStairs(false);

                }

                if (ladder == LadderType.LADDER)
                {
                    playerMovement.ignoreCollision(adjacentCollider, false);
                }
            }

        }
    }

}
