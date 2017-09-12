using UnityEngine;

public class SceneryInteraction : MonoBehaviour
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

        PlayerStatusVariables.canInteractWithScenery = true;

        if (PlayerStatusVariables.isInteractingWithScenery)
        {
            miscellaneousMovement.SubscribeInteractiveScenery(Interaction);
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStatusVariables.canInteractWithScenery = false;
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