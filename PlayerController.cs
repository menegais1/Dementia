using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{

    public float move;
    public float climbStairsMovement;

    public bool jump;
    public bool jog;
    public bool run;
    public bool crouch;
    public bool dodge;
    public bool climb;
    public bool climbStairsPress;
    public bool takeOfCamera;
    public bool interactWithScenery;
    public bool takeItem;
    public bool revokeControl;
    public bool onStairsControl;
    public bool runningCoroutine;

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
        if (!revokeControl && !onStairsControl)
        {
            move = Input.GetAxisRaw("Horizontal");
            jump = Input.GetButtonDown("Jump");
            dodge = Input.GetButtonDown("Dodge");
            run = Input.GetButton("Jogging/Running");
            jog = Input.GetButtonUp("Jogging/Running");
            crouch = Input.GetButton("Crouching");
            takeOfCamera = Input.GetButtonDown("Take Of Camera");
            climbStairsMovement = Input.GetAxisRaw("Vertical");
            climbStairsPress = Input.GetButtonDown("Climb Stairs");
            interactWithScenery = Input.GetButtonDown("Interact Scenery");
            takeItem = Input.GetButtonDown("Take Item");

        }
        else if (onStairsControl && !revokeControl)
        {
            move = 0;
            jump = false;
            dodge = false;
            run = false;
            jog = false;
            crouch = false;
            takeOfCamera = false;
            interactWithScenery = false;
            takeItem = false;
            climbStairsMovement = Input.GetAxisRaw("Vertical");
            climbStairsPress = Input.GetButtonDown("Climb Stairs");

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
            climbStairsMovement = 0;
            climbStairsPress = false;
            interactWithScenery = false;
            takeItem = false;
        }

    }

    public void revokeMovementPlayerControl(float timeToRevoke, MonoBehaviour monoBehaviour)
    {

        if (!runningCoroutine)
            monoBehaviour.StartCoroutine(waitForSeconds(timeToRevoke, true));
    }

    public void revokeMovementPlayerControl()
    {

        revokeControl = true;
    }

    public void changeMovementPlayerControl(bool onStairs)
    {

        onStairsControl = onStairs;
    }

    public void giveMovementPlayerControl()
    {
        revokeControl = false;
    }

    public void giveMovementPlayerControlWithCooldown(float timeToCooldown, MonoBehaviour monoBehaviour)
    {
        revokeControl = false;
        move = 0;
        if (!runningCoroutine)
            monoBehaviour.StartCoroutine(waitForSeconds(timeToCooldown, false));
    }

    private IEnumerator waitForSeconds(float time, bool revoke)
    {

        runningCoroutine = true;
        for (int i = 0; i <= 1; i++)
        {

            revokeControl = !revokeControl;
            yield return new WaitForSeconds(time);

        }
        runningCoroutine = false;

    }

}
