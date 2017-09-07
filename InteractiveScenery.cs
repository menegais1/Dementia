/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveScenery : MonoBehaviour
{


    public MiscMovement playerMovement;

    #region Métodos Unity


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
                playerMovement.setCanInteractWithScenery(true);

                if (playerMovement.isInteractingWithScenery)
                {
                    interaction();
                    playerMovement.isInteractingWithScenery = false;
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
                playerMovement.setCanInteractWithScenery(false);

            }
        }
    }
    #endregion

    #region Métodos Gerais

    public void interaction()
    {
        print("Interação");
    }

    #endregion

}
*/
