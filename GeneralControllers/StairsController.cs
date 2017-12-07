using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsController : MonoBehaviour
{
    public Collider2D adjacentCollider;
    public Collider2D stairsCollider;
    public StairsTriggerType stairsTriggerType;
    private PlayerStatusVariables playerStatusVariables;
    private ApostleStatusVariables apostleStatusVariables;


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
            playerStatusVariables.canClimbStairs = true;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables.canClimbStairs = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if ((!other.CompareTag("Player") && !other.CompareTag("Enemy")) || other.isTrigger) return;
        if (other.CompareTag("Player"))
            playerStatusVariables.canClimbStairs = false;
        else if (other.CompareTag("Enemy"))
            apostleStatusVariables.canClimbStairs = false;
    }
}