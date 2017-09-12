﻿using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    private MiscellaneousMovement miscellaneousMovement;

    #region Métodos Unity

    private void OnTriggerEnter2D(Collider2D other)
    {
        miscellaneousMovement = MiscellaneousMovement.GetInstance();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        PlayerStatusVariables.canTakeItem = true;

        if (PlayerStatusVariables.isTakingItem)
        {
            miscellaneousMovement.SubscribeItemEffect(Interaction);
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStatusVariables.canTakeItem = false;
        }
    }

    #endregion

    #region Métodos Gerais

    public void Interaction()
    {
        print("Item");
        Destroy(this.gameObject);
    }

    #endregion
}