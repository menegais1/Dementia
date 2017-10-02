using UnityEngine;

public class SceneryInteraction : MonoBehaviour
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

        playerStatusVariables.canInteractWithScenery = true;

        if (playerStatusVariables.isInteractingWithScenery)
        {
            playerMiscellaneousMovement.SubscribeInteractiveScenery(Interaction);
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerStatusVariables.canInteractWithScenery = false;
        }
    }

    #endregion

    #region Métodos Gerais

    public void Interaction()
    {
        print("Teste");
    }

    #endregion
}