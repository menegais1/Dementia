using UnityEngine;

public class SceneryInteraction : MonoBehaviour
{
    private PlayerMiscellaneousMovement playerMiscellaneousMovement;
    private PlayerStatusVariables playerStatusVariables;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        playerMiscellaneousMovement = other.GetComponent<PlayerManager>().MiscellaneousMovement;
        playerStatusVariables = other.GetComponent<PlayerManager>().PlayerStatusVariables;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        playerStatusVariables.canInteractWithScenery = true;

        if (playerStatusVariables.isInteractingWithScenery)
        {
            Debug.Log("subscribed");
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


    public void Interaction()
    {
        print("interação");
    }
}