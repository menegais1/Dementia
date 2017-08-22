using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{



    #region Váriaveis Gerais

    private bool cameraState;
    public float maxVelocity;
    public float force;
    public float jumpForce;
    private bool canJump;
    private bool canSwim;
    private bool canClimb;
    private bool canClimbStairs;
    public int movementState;

    #endregion

    #region Váriaveis Adicionadas


    public enum MovementStateENUM
    {
        IDLE = 0,
        WALKING = 1,
        JOGGING = 2,
        RUNNING = 3,
        CROUCHING = 4,
        JUMPING = 5,
        FALLING = 6,
        DODGING = 7,
    }

    public bool isJogging;
    public bool isFalling;
    private bool isCrouching;
    private bool isDodging;
    public bool facingRight;

    private float runningPressingTime;

    public CameraController characterCamera;
    public float cameraZoomSize;

    public float dodgeForce;

    public float crouchingSpeed;
    public float crouchHeigth;
    private float characterHeigth;

    public LayerMask jumpLayer;
    public Transform groundCheckPosition;

    private Vector2 forceApplied;


    Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;

    PlayerController playerController;

    #endregion

    #region Métodos Unity

    void Start()
    {

        playerController = PlayerController.getInstance();

        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        movementState = (int)MovementStateENUM.IDLE;

        forceApplied = new Vector2(0, 0);
        isCrouching = false;
        isJogging = false;
        characterHeigth = boxCollider.size.y;

        facingRight = true;
    }

    private void Update()
    {

        playerController.checkForPlayerInput();
        facingRight = checkFacingDirection();
        RaycastHit2D canJumpRaycast = Physics2D.Raycast(groundCheckPosition.position, Vector2.down);
        canJump = (canJumpRaycast.collider != null && canJumpRaycast.distance <= 0.20f) ? true : false;

        if (playerController.jump && canJump)
        {
            jump();
        }

        if (playerController.dodge)
        {
            playerController.revokeMovementPlayerControl(0.5f, this);
            isDodging = true;
        }

        if (playerController.takeOfCamera)
        {
            cameraState = !cameraState;
            takeOfCamera();
        }


        if (playerController.jog && runningPressingTime <= 0.5)
        {
            isJogging = !isJogging;
            runningPressingTime = 0;
        }
        else if (playerController.jog)
        {
            runningPressingTime = 0;

        }

        if (playerController.crouch)
        {
            crouch();
            if (boxCollider.size.y == crouchHeigth)
            {
                movementState = (int)MovementStateENUM.CROUCHING;
            }
        }
        else if (isCrouching)
        {
            raise();
            movementState = (int)MovementStateENUM.IDLE;

        }

        if (!isFalling && !isDodging)
        {

            if (playerController.move != 0)
            {
                if (isCrouching)
                {
                    movementState = (int)MovementStateENUM.CROUCHING;
                }
                else if (isJogging)
                {
                    movementState = (int)MovementStateENUM.JOGGING;
                }
                if (playerController.run)
                {
                    movementState = (runningPressingTime >= 0.5 || movementState == (int)MovementStateENUM.IDLE) ? (int)MovementStateENUM.RUNNING : movementState;
                    runningPressingTime += Time.deltaTime;
                }

                if (!isJogging && !isCrouching && !playerController.run)
                {
                    movementState = (int)MovementStateENUM.WALKING;

                }

            }
            else
            {
                movementState = (int)MovementStateENUM.IDLE;
            }
        }
        else if (isFalling)
        {
            movementState = (int)MovementStateENUM.FALLING;
        }
        else if (isDodging)
        {
            movementState = (int)MovementStateENUM.DODGING;

        }





    }

    void FixedUpdate()
    {


        switch (movementState)
        {
            case (int)MovementStateENUM.WALKING:
                forceApplied = walk(playerController.move);
                break;
            case (int)MovementStateENUM.JOGGING:
                forceApplied = jog(playerController.move);
                break;
            case (int)MovementStateENUM.RUNNING:
                forceApplied = run(playerController.move);
                break;
            case (int)MovementStateENUM.CROUCHING:
                forceApplied = crouchWalk(playerController.move);
                break;
            case (int)MovementStateENUM.DODGING:
                forceApplied = dodge();
                break;
        }


        //Diminuir deslizada
        Physics.addContraryForce(forceApplied, rigidBody, movementState);



    }

    void LateUpdate()
    {
        if (rigidBody.velocity.y == 0 && isFalling)
        {
            isFalling = false;
            playerController.giveMovementPlayerControlWithCooldown(0.2f, this);
        }
    }


    #endregion

    #region Métodos Gerais


    public bool checkFacingDirection()
    {

        if (playerController.move == 1)
        {
            return true;
        }
        else if (playerController.move == -1)
        {
            return false;
        }

        return facingRight;
    }



    public void climb()
    {

    }

    public void takeOfCamera()
    {

        StartCoroutine(characterCamera.takeOfCamera(cameraZoomSize, 100, cameraState));

    }

    Vector2 walk(float move)
    {

        return Physics.movementByForce(force, 1f, maxVelocity, move, this.rigidBody);

    }

    Vector2 jog(float move)
    {

        return Physics.movementByForce(force, 1.5f, maxVelocity, move, this.rigidBody);

    }

    Vector2 run(float move)
    {

        return Physics.movementByForce(force, 2f, maxVelocity, move, this.rigidBody);

    }


    Vector2 crouchWalk(float move)
    {

        return Physics.movementByForce(force, 0.75f, maxVelocity, move, this.rigidBody);

    }


    void crouch()
    {

        if (boxCollider.size.y > crouchHeigth)
        {
            boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y - crouchingSpeed);
            boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - (crouchingSpeed / 2));
            isCrouching = true;
        }


    }

    void raise()
    {
        if (boxCollider.size.y < characterHeigth)
        {
            boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y + crouchingSpeed);
            boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y + (crouchingSpeed / 2));
        }
        else
        {
            isCrouching = false;
        }

    }


    public void jump()
    {

        Physics.jump(jumpForce, rigidBody);
        isFalling = true;
        playerController.revokeMovementPlayerControl();

    }

    public Vector2 dodge()
    {
        //Adicionar a colisão com inimigos depois

        Vector2 forceApplied = Physics.addImpulseForce(dodgeForce, rigidBody, facingRight);
        isDodging = false;
        return forceApplied;

    }

    public void climbStairs()
    {

    }

    #endregion


    //// Revisar Velocidade nas rampas, falta rotação

    ////Quaternion rotation = Quaternion.FromToRotation(Vector2.up, canJumpRaycast.normal);
    ////transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1);

    //RaycastHit2D isSlope = Physics2D.Raycast(new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y), Vector2.down);

    //if (isSlope.collider != null)
    //{
    //    //float angle = Vector2.Angle(isSlope.normal, Vector2.up);
    //    //angle = (angle != 0f) ? angle : 1f;
    //    //move = move * (angle / 10);
    //}

    //Debug.DrawRay(new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y), Vector2.down, Color.red);
    ////Fim da Revisão
}
