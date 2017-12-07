using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    public Collider2D adjacentCollider;
    public LadderType ladder;

    private PlayerStatusVariables playerStatusVariables;
    private ApostleStatusVariables apostleStatusVariables;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Enemy")) || other.isTrigger) return;
        if (ladder == LadderType.Ladder) return;

        if (other.CompareTag("Player"))
            playerStatusVariables = other.GetComponent<PlayerManager>().PlayerStatusVariables;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables = other.GetComponent<ApostleManager>().ApostleStatusVariables;

        adjacentCollider = transform.parent.GetComponent<LadderController>().adjacentCollider;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Enemy")) || other.isTrigger) return;
        if (ladder != LadderType.BottomLadder && ladder != LadderType.TopLadder) return;
        if (other.CompareTag("Player"))
            playerStatusVariables.canClimbLadder = true;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables.canClimbLadder = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Enemy")) || other.isTrigger) return;
        if (ladder != LadderType.BottomLadder && ladder != LadderType.TopLadder) return;
        if (other.CompareTag("Player"))
            playerStatusVariables.canClimbLadder = false;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables.canClimbLadder = false;
    }
}