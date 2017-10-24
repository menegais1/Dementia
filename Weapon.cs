using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private string id;
    private bool worldStatus;

    [SerializeField] private int maxMagazine;
    [SerializeField] private float damage;
    [SerializeField] private float precision;
    [SerializeField] private float reloadTime;
    [SerializeField] private float bulletsPerMinute;
    [SerializeField] private bool automatic;
    [SerializeField] private float recoil;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int currentMagazine;
    [SerializeField] private Transform muzzlePosition;
    [SerializeField] private GameObject bullet;

    private float nextShot;
    //private int shotsFired;
    
    public bool Automatic
    {
        get { return automatic; }
        set { automatic = value; }
    }


    public void Shoot(Vector3 direction)
    {
        if (CheckIfCanShoot())
        {
            var instantiateBullet =
                Bullet.InstantiateBullet(muzzlePosition.position, direction, bullet);
            instantiateBullet.Shoot();
            SpendBullets();
            //shotsFired++;
        }
        else if (currentMagazine == 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (currentAmmo > 0 && currentMagazine < maxMagazine)
        {
            var bulletsToMaxMagazine = maxMagazine - currentMagazine;
            if (currentAmmo >= bulletsToMaxMagazine)
            {
                currentAmmo -= bulletsToMaxMagazine;
                currentMagazine += bulletsToMaxMagazine;
            }
            else
            {
                currentMagazine += currentAmmo;
                currentAmmo = 0;
            }
            nextShot += reloadTime;
        }
    }

    /* public float RecoilCalc()
     {
         if (automatic)
         {
             if (shotsFired < maxMagazine / 3)
             {
                 return recoil / precision;
             }
 
             if (shotsFired >= maxMagazine / 3 && shotsFired < maxMagazine * 2 / 3)
             {
                 return recoil / precision * 2;
             }
 
             if (shotsFired >= maxMagazine * 2 / 3)
             {
                 return recoil;
             }
         }
     }*/

    private bool CheckIfCanShoot()
    {
        if (currentMagazine > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + (60 / bulletsPerMinute);
            return true;
        }

        return false;
    }

    private void SpendBullets()
    {
        currentMagazine--;
    }
}