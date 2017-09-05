using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscMovement : MonoBehaviour
{


    #region Váriaveis Gerais

    private bool canTakeItem;
    private bool existNote;
    private bool canReadNote;
    private bool canInteractWithScenery;
    public bool isInteractingWithScenery;
    public bool isTakingItem;

    private bool readingNote;

    private PlayerController playerController;
    #endregion



    #region Métodos Unity


    // Use this for initialization
    void Start()
    {

        playerController = PlayerController.getInstance();

    }

    // Update is called once per frame
    void Update()
    {

        if (playerController.interactWithScenery && canInteractWithScenery)
        {
            interactScenery();
        }
        else if (!canInteractWithScenery)
        {
            isInteractingWithScenery = false;
        }


        if (canTakeItem && playerController.takeItem)
        {
            takeItem();
        }
        else if (!canTakeItem)
        {
            isTakingItem = false;
        }
    }

    #endregion

    #region Métodos Gerais

    public void readNotes()
    {

    }

    public void interactScenery()
    {
        isInteractingWithScenery = true;
    }

    public void takeItem()
    {
        isTakingItem = true;
    }

    public void setCanInteractWithScenery(bool canInteractWithScenery)
    {
        this.canInteractWithScenery = canInteractWithScenery;
    }

    public void setCanTakeItem(bool canTakeItem)
    {
        this.canTakeItem = canTakeItem;
    }

    #endregion

}
