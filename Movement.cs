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
        ON_AIR = 5,
        DODGING = 6,
        CLIMBING_STAIRS = 7,
    }

    public struct RaycastPoints
    {
        public Vector2 bottomRight;
        public Vector2 bottomLeft;
        public Vector2 bottomMid;
    }

    public struct RaycastHit2DPoints
    {
        public RaycastHit2D bottomLeftRay;
        public RaycastHit2D bottomRightRay;
        public RaycastHit2D bottomMidRay;
    }

    public RaycastHit2DPoints raycastHit2DPoints;

    public RaycastPoints raycastPoints;
    public bool isJogging;
    public bool isOnAir;
    public bool climbingStairs;
    private bool isCrouching;
    private bool isDodging;
    public bool isOnStairs;
    public bool facingRight;

    private float currentGravityScale;

    private float runningPressingTime;

    public CameraController characterCamera;
    public float cameraZoomSize;

    public float dodgeForce;

    public float crouchingSpeed;
    public float crouchHeigth;
    private float characterHeigth;

    public LayerMask jumpLayer;

    private Vector2 forceApplied;


    public Rigidbody2D rigidBody;
    public BoxCollider2D boxCollider;

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
        currentGravityScale = rigidBody.gravityScale;

    }

    private void Update()
    {
        facingRight = checkFacingDirection();
        flip();
        playerController.checkForPlayerInput();
        setRaycastPoints();
        castRays();

        canJump = checkGroundForJump();



        if (!canJump && !climbingStairs)
        {
            isOnAir = true;
        }

        if (canJump && climbingStairs)
        {
            climbingStairs = false;
        }

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


        if (isOnStairs && playerController.climbStairs != 0 && !isOnAir)
        {
            climbingStairs = true;
            switchGravity(false);
            resetVelocityX();
            movementState = (int)MovementStateENUM.CLIMBING_STAIRS;
        }
        else if (!isOnStairs || !climbingStairs)
        {
            switchGravity(true);
        }

        if (climbingStairs && playerController.climbStairs == 0 && rigidBody.velocity.y != 0)
        {
            resetVelocityY();
        }


        if (!isOnAir && !isDodging && !climbingStairs)
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
        else if (isOnAir)
        {
            movementState = (int)MovementStateENUM.ON_AIR;
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
            case (int)MovementStateENUM.CLIMBING_STAIRS:
                forceApplied = climbStairs(playerController.climbStairs);
                break;
        }


        //Diminuir deslizada
        Physics.addContraryForce(forceApplied, rigidBody, movementState);



    }

    void LateUpdate()
    {
        if (rigidBody.velocity.y == 0 && isOnAir)
        {
            isOnAir = false;
            playerController.giveMovementPlayerControlWithCooldown(0.1f, this);
        }

    }


    #endregion

    #region Métodos Gerais

    public void snapToPosition(Vector2 position)
    {

        StartCoroutine(snapToPositionCoroutine(position));


    }

    private IEnumerator snapToPositionCoroutine(Vector2 position)
    {
        while (!Mathf.Approximately(rigidBody.position.x, position.x))
        {
            rigidBody.position = Vector2.Lerp(rigidBody.position, new Vector2(position.x, rigidBody.position.y), 0.10f);
            yield return new WaitForEndOfFrame();

        }
    }

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

    public void switchGravity(bool on)
    {
        if (on)
        {
            rigidBody.gravityScale = currentGravityScale;
        }
        else
        {
            rigidBody.gravityScale = 0;

        }

    }

    public void takeOfCamera()
    {

        StartCoroutine(characterCamera.takeOfCamera(cameraZoomSize, 100, cameraState));

    }

    Vector2 walk(float move)
    {

        return Physics.movementByForce(force, 1f, maxVelocity, move, this.rigidBody, false);

    }

    Vector2 jog(float move)
    {

        return Physics.movementByForce(force, 1.5f, maxVelocity, move, this.rigidBody, false);

    }

    Vector2 run(float move)
    {

        return Physics.movementByForce(force, 2f, maxVelocity, move, this.rigidBody, false);

    }


    Vector2 crouchWalk(float move)
    {

        return Physics.movementByForce(force, 0.75f, maxVelocity, move, this.rigidBody, false);

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
        isOnAir = true;
        playerController.revokeMovementPlayerControl();

    }

    public void flip()
    {
        if (!facingRight)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;

        }

    }

    public Vector2 dodge()
    {
        //Adicionar a colisão com inimigos depois

        //Zerar velocidade para desviar a mesma distância
        resetVelocityX();

        Vector2 forceApplied = Physics.addImpulseForce(dodgeForce, rigidBody, facingRight);
        isDodging = false;
        return forceApplied;

    }

    public Vector2 climbStairs(float move)
    {
        return Physics.movementByForce(force, 0.75f, maxVelocity, move, this.rigidBody, true);
    }

    public void resetVelocityY()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
    }

    public void resetVelocityX()
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
    }

    public void setRaycastPoints()
    {
        raycastPoints.bottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y);
        raycastPoints.bottomRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
        raycastPoints.bottomMid = raycastPoints.bottomLeft + (Vector2.right * boxCollider.size.x / 2);


        Debug.DrawRay(raycastPoints.bottomLeft, Vector2.down, Color.green);
        Debug.DrawRay(raycastPoints.bottomRight, Vector2.down, Color.red);
        Debug.DrawRay(raycastPoints.bottomMid, Vector2.down, Color.blue);

    }

    public void castRays()
    {

        raycastHit2DPoints.bottomLeftRay = Physics2D.Raycast(raycastPoints.bottomLeft, Vector2.down);
        raycastHit2DPoints.bottomRightRay = Physics2D.Raycast(raycastPoints.bottomLeft, Vector2.down);
        raycastHit2DPoints.bottomMidRay = Physics2D.Raycast(raycastPoints.bottomLeft, Vector2.down);

    }

    public bool checkGroundForJump()
    {
        if (raycastHit2DPoints.bottomMidRay.collider != null && raycastHit2DPoints.bottomMidRay.distance <= 0.10f)
        {
            return true;
        }

        return false;
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
