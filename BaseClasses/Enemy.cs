using UnityEngine;

public abstract class Enemy : MonoBehaviour {


    #region Váriaveis Gerais

    protected bool worldStatus;
    protected string id;
    protected float life;
 

    #endregion


    #region Métodos Gerais

    public abstract void takeDamage();
    public abstract void regenLife();
    public abstract void die();
    public abstract void checkLife();
    public abstract void loseLife();



    #endregion

}
