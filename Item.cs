using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{


    #region Váriaveis Gerais

    protected bool worldStatus;
    protected string id;
    protected int quantity;


    #endregion


    #region Métodos Gerais

    public abstract void effect();

    #endregion

}
