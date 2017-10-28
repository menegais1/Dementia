using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private string id;
    private bool worldStatus;

    [SerializeField] private int maxMagazine;
    [SerializeField] private float damage;
    [SerializeField] private float reloadTime;
    [SerializeField] private float bulletsPerMinute;
    [SerializeField] private bool automatic;
    [SerializeField] private float recoilBaseModifier;
    [SerializeField] private float recoilBaseTime;
    [SerializeField] private float recoilCooldownTime;
    [SerializeField] private float recoilCooldownModifier;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int currentMagazine;
    [SerializeField] private WeaponType type;
    [SerializeField] private Transform muzzlePosition;
    [SerializeField] private GameObject bullet;


    private float nextShot;
    private float recoilCurrentTime;
    private float timeFromLastShot;
    private float timeAtLastShot;
    private float timePerBullet;

    private void Start()
    {
        timePerBullet = 60 / bulletsPerMinute;
        timeFromLastShot = Time.time;
    }

    private void Update()
    {
        if (timeFromLastShot > recoilCooldownTime)
        {
            recoilCurrentTime = recoilCurrentTime > 0 ? recoilCurrentTime - recoilCooldownModifier : 0;
        }

        timeFromLastShot = Time.time - timeAtLastShot;
    }

    public bool Automatic
    {
        get { return automatic; }
        set { automatic = value; }
    }


    public void Shoot(Vector3 direction)
    {
        if (CheckIfCanShoot())
        {
            var recoilCalc = RecoilCalc(direction);
            var instantiateBullet =
                Bullet.InstantiateBullet(muzzlePosition.position, recoilCalc, bullet);
            instantiateBullet.Shoot();
            SpendBullets();
            recoilCurrentTime += timePerBullet;
            timeAtLastShot = Time.time;
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

    private Vector3 RecoilCalc(Vector3 direction)
    {
        if (recoilCurrentTime > recoilBaseTime)
        {
            var modifier = (recoilCurrentTime - recoilBaseTime) * recoilBaseModifier;
            var angleAxis = Quaternion.AngleAxis(modifier, Vector3.forward);
            return angleAxis * direction;
        }

        return direction;
    }

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