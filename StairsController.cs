using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsController : MonoBehaviour
{
    public Collider2D adjacentCollider;
    public Collider2D stairsCollider;
    public StairsTriggerType stairsTriggerType;


    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;
        PlayerStatusVariables.canClimbStairs = true;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;

        PlayerStatusVariables.canClimbStairs = false;
    }
}