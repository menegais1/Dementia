using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    private PlayerMiscellaneousMovement playerMiscellaneousMovement;
    private PlayerStatusVariables playerStatusVariables;

    #region Métodos Unity

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerMiscellaneousMovement = PlayerMiscellaneousMovement.GetInstance();
        playerStatusVariables = PlayerStatusVariables.GetInstance();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        playerStatusVariables.canTakeItem = true;

        if (playerStatusVariables.isTakingItem)
        {
            playerMiscellaneousMovement.SubscribeItemEffect(Interaction);
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerStatusVariables.canTakeItem = false;
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