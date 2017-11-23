using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : Item
{
    [SerializeField] private float lifeRegen;


    public void Effect(PlayerStatusController playerStatusController)
    {
        if (playerStatusController == null) return;

        playerStatusController.RegenLife(lifeRegen);
        Destroy(this.gameObject);
    }
}