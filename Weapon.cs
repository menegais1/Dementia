using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    #region Váriaveis Gerais

    private string id;
    private bool worldStatus;
    private int maxMagazine;
    private float damage;
    private float precision;
    private float reloadTime;
    private float reach;
    private int bulletsPerSecond;
    private bool automatic;
    private float recoil;
    private int type;
    private Bullet bullet;
    private int currentAmmo;
    private int currentMagazine;
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

    public void shoot()
    {

    }

    public void reload()
    {

    }

    public void recoilCalc()
    {

    }

    public void checkBullets()
    {

    }

    public void spendBullets()
    {

    }


    #endregion
}
