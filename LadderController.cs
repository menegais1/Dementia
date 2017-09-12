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


    public Collider2D adjacentCollider;
    public LadderType ladder;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (ladder != LadderType.LADDER)
        {
            adjacentCollider = transform.parent.GetComponent<LadderController>().adjacentCollider;
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (ladder == LadderType.BOTTOM_LADDER || ladder == LadderType.TOP_LADDER)
        {
            PlayerStatusVariables.canClimbLadder = true;
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (ladder == LadderType.BOTTOM_LADDER || ladder == LadderType.TOP_LADDER)
        {
            PlayerStatusVariables.canClimbLadder = false;
        }
    }
}