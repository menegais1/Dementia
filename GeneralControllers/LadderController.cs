using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    public Collider2D adjacentCollider;
    public LadderType ladder;

    private PlayerStatusVariables playerStatusVariables;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (ladder == LadderType.Ladder) return;
        playerStatusVariables = PlayerStatusVariables.GetInstance();
        adjacentCollider = transform.parent.GetComponent<LadderController>().adjacentCollider;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (ladder != LadderType.BottomLadder && ladder != LadderType.TopLadder) return;
        playerStatusVariables.canClimbLadder = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        if (ladder != LadderType.BottomLadder && ladder != LadderType.TopLadder) return;
        playerStatusVariables.canClimbLadder = false;
    }
}