using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : Item
{
    #region Váriaveis Gerais

    private float velocityGain;
    private float precisionGain;
    private float staminaRegenGain;
    private int status;
    private float duration;

    #endregion


    #region Métodos Unity

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    #endregion

    #region Métodos Gerais

    public override void Effect()
    {
    }

    public void afterEffect()
    {
    }

    public void morphinEffect()
    {
    }

    #endregion
}