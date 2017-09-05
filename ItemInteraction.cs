using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{


    public MiscMovement playerMovement;

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player")
        {
            playerMovement = other.GetComponent<MiscMovement>();
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (playerMovement != null)
            {
                playerMovement.setCanTakeItem(true);

                if (playerMovement.isTakingItem)
                {
                    print("Item Taken");
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (playerMovement != null)
            {
                playerMovement.setCanTakeItem(false);
            }
        }
    }
}
