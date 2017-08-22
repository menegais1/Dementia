using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{

    public bool jump;
    public float move;
    public bool jog;
    public bool run;
    public bool crouch;
    public bool dodge;
    public bool climb;
    public bool climbStairs;
    public bool takeOfCamera;

    public bool revokeControl;

    private static PlayerController instance;

    public static PlayerController getInstance()
    {
        if (instance == null)
        {
            new PlayerController();
        }

        return instance;

    }

    private PlayerController()
    {
        instance = this;
    }

    public void checkForPlayerInput()
    {
        if (!revokeControl)
        {
            move = Input.GetAxisRaw("Horizontal");
            jump = Input.GetButtonDown("Jump");
            dodge = Input.GetButtonDown("Dodge");
            run = Input.GetButton("Jogging/Running");
            jog = Input.GetButtonUp("Jogging/Running");
            crouch = Input.GetButton("Crouching");
            takeOfCamera = Input.GetButtonDown("Take Of Camera");
        }
        else
        {
            move = 0;
            jump = false;
            dodge = false;
            run = false;
            jog = false;
            crouch = false;
            takeOfCamera = false;
        }

    }

    public void revokeMovementPlayerControl(float timeToRevoke, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(waitForSeconds(timeToRevoke));
    }

    public void revokeMovementPlayerControl()
    {

        revokeControl = true;
    }

    public void giveMovementPlayerControl()
    {
        revokeControl = false;
    }

    public void giveMovementPlayerControlWithCooldown(float timeToCooldown, MonoBehaviour monoBehaviour)
    {
        revokeControl = false;
        move = 0;
        monoBehaviour.StartCoroutine(waitForSeconds(timeToCooldown));
    }

    private IEnumerator waitForSeconds(float timeToRevoke)
    {
        for (int i = 0; i <= 1; i++)
        {
            revokeControl = !revokeControl;
            yield return new WaitForSeconds(timeToRevoke);

        }

    }

}
