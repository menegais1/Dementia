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

    public CoroutineManager coroutineManager;
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
        JUMPING = 8,
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
    public bool isJumping;
    public bool isClimbingStairs;
    public bool snapToPositionRan;
    public bool isClimbingObject;
    public bool canClimbObject;
    public bool coroutineEndedRunning;
    public bool leaveStairs;
    public float baseStaminaSpent;
    public float forceOnEdge;
    private bool isCrouching;
    private bool isDodging;
    public bool isOnStairs;
    public bool facingRight;
    public float stairsSmoothness;
    private float currentGravityScale;

    private float runningPressingTime;

    public CameraController characterCamera;
    public float cameraZoomSize;

    public float dodgeForce;

    public float crouchingSpeed;
    public float crouchHeigth;
    private float characterHeigth;

    public float detectionDistance;

    private Vector2 forceApplied;


    public Rigidbody2D rigidBody;
    public BoxCollider2D boxCollider;
    public Player player;
    public PlayerController playerController;

    #endregion

    #region Métodos Unity

    void Start()
    {

        playerController = PlayerController.getInstance();

        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GetComponent<Player>();
        movementState = (int)MovementStateENUM.IDLE;

        forceApplied = new Vector2(0, 0);
        isCrouching = false;
        isJogging = false;
        characterHeigth = boxCollider.size.y;

        facingRight = true;
        currentGravityScale = rigidBody.gravityScale;

        coroutineManager = CoroutineManager.getInstance();
        coroutineManager.insertNewCoroutine(teste(), "teste");
    }

    private IEnumerator teste()
    {
        while (Time.time < 10)
        {
            print("Eu Sou uma coroutine");
            yield return new WaitForSeconds(3);
        }

        coroutineManager.findCoroutine("teste").setIsRunning(false);
    }

    private void Update()
    {

        checkFacingDirection();
        playerController.checkForPlayerInput();
        setRaycastPoints();
        castRays();

        canJump = checkGroundForJump();
        isDodging = testForDodging();
        isJumping = testForJumping();
        isOnAir = !canJump && !isClimbingStairs && !isClimbingObject ? true : isOnAir;

        if (canClimbStairs)
        {
            if (playerController.climbStairsPress && !isClimbingStairs)
            {
                isClimbingStairs = true;
                playerController.changeMovementPlayerControl(true);
            }
        }


        if (isClimbingStairs && playerController.climbStairsMovement == 0f)
        {
            resetVelocityY();
        }

        if (isClimbingStairs && canJump && coroutineEndedRunning)
        {
            leaveStairs = true;
            isClimbingStairs = false;
            switchGravity(true);
            coroutineEndedRunning = false;
            snapToPositionRan = false;
            playerController.changeMovementPlayerControl(false);
            revokeControlOnStairs(0.5f);
            resetVelocityY();
        }

        if (playerController.takeOfCamera)
        {
            cameraState = !cameraState;
            takeOfCamera();
        }

        if (isJumping)
        {
            if (player.checkStamina(baseStaminaSpent * 4f))
            {
                jump();
                player.spendStamina(baseStaminaSpent * 4f);
            }
        }

        if (canClimbObject && playerController.jump)
        {
            isClimbingObject = true;
            switchGravity(false);
        }

        if (isClimbingObject && coroutineEndedRunning)
        {
            isClimbingObject = false;
            coroutineEndedRunning = false;
            snapToPositionRan = false;
            switchGravity(true);
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



        if (!isOnAir && !isDodging && !isClimbingStairs)
        {

            if (playerController.move != 0)
            {
                if (isCrouching)
                {
                    movementState = (int)MovementStateENUM.CROUCHING;
                }
                else if (isJogging)
                {
                    if (player.checkStamina(baseStaminaSpent * 1.5f))
                    {
                        movementState = (int)MovementStateENUM.JOGGING;
                    }
                    else
                    {
                        movementState = (int)MovementStateENUM.WALKING;
                    }
                }
                if (playerController.run)
                {
                    movementState = (runningPressingTime >= 0.5 || movementState == (int)MovementStateENUM.IDLE) ? (int)MovementStateENUM.RUNNING : movementState;
                    runningPressingTime += Time.deltaTime;
                    if (movementState == (int)MovementStateENUM.RUNNING)
                    {
                        if (!player.checkStamina(baseStaminaSpent * 2f))
                        {
                            movementState = (int)MovementStateENUM.WALKING;
                        }
                    }
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
        else if (isClimbingStairs)
        {
            switchGravity(false);
            movementState = (int)MovementStateENUM.CLIMBING_STAIRS;
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
                player.spendStamina(baseStaminaSpent * 1.5f * Time.deltaTime);
                break;
            case (int)MovementStateENUM.RUNNING:
                forceApplied = run(playerController.move);
                player.spendStamina(baseStaminaSpent * 2f * Time.deltaTime);
                break;
            case (int)MovementStateENUM.CROUCHING:
                forceApplied = crouchWalk(playerController.move);
                break;
            case (int)MovementStateENUM.DODGING:
                if (player.checkStamina(player.maxStamina))
                {
                    forceApplied = dodge();
                    player.spendStamina(player.maxStamina);
                }
                break;
            case (int)MovementStateENUM.CLIMBING_STAIRS:
                forceApplied = climbStairs(playerController.climbStairsMovement);
                break;
            //case (int)MovementStateENUM.JUMPING:
            //    jump();
            //    break;
            case (int)MovementStateENUM.ON_AIR:
                playerController.revokeMovementPlayerControl();
                break;
        }


        //Diminuir deslizada
        Physics.addContraryForce(forceApplied, rigidBody, movementState);



    }

    void LateUpdate()
    {
        if (rigidBody.velocity.y == 0 && isOnAir && canJump)
        {
            isOnAir = false;
            playerController.giveMovementPlayerControlWithCooldown(0.1f, this);
        }

    }


    #endregion

    #region Métodos Gerais

    public void snapToPositionStairs(Vector2 position)
    {
        resetVelocityX();
        resetVelocityY();
        playerController.revokeMovementPlayerControl();
        StartCoroutine(snapToPositionStairsCoroutine(position));
        snapToPositionRan = true;

    }

    public void snapToPositionObject(Vector2 position)
    {
        resetVelocityX();
        resetVelocityY();
        playerController.revokeMovementPlayerControl();
        StartCoroutine(snapToPositionObjectCoroutine(position));
        snapToPositionRan = true;

    }

    public bool checkPositionX(Vector2 position)
    {
        if (Mathf.Abs(rigidBody.position.x - position.x) <= 0.1)
        {
            return true;
        }
        return false;
    }


    private IEnumerator snapToPositionStairsCoroutine(Vector2 position)
    {
        while (Mathf.Abs(rigidBody.position.x - position.x) >= 0.01 || Mathf.Abs(rigidBody.position.y - position.y) >= 0.01)
        {

            rigidBody.position = Vector2.Lerp(rigidBody.position, new Vector2(position.x, position.y), stairsSmoothness);
            yield return new WaitForEndOfFrame();
        }
        coroutineEndedRunning = true;
        playerController.giveMovementPlayerControl();
    }

    private IEnumerator snapToPositionObjectCoroutine(Vector2 position)
    {
        float localScale = (position.x > rigidBody.position.x) ? +transform.localScale.x : -transform.localScale.x;
        while (Mathf.Abs(rigidBody.position.x - (position.x + localScale)) >= 0.01 || Mathf.Abs(rigidBody.position.y - (position.y + transform.localScale.y)) >= 0.01)
        {
            if (Mathf.Abs(rigidBody.position.y - (position.y + transform.localScale.y)) >= 0.01)
            {
                rigidBody.position = Vector2.Lerp(rigidBody.position, new Vector2(rigidBody.position.x, (position.y + transform.localScale.y)), stairsSmoothness);
            }
            else
            {

                rigidBody.position = Vector2.Lerp(rigidBody.position, new Vector2(position.x + localScale, rigidBody.position.y), stairsSmoothness);
            }
            yield return new WaitForEndOfFrame();
        }
        coroutineEndedRunning = true;
        playerController.giveMovementPlayerControl();
    }


    public void ignoreCollision(Collider2D collider, bool ignore)
    {
        Physics2D.IgnoreCollision(boxCollider, collider, ignore);
    }

    public void checkFacingDirection()
    {

        if (playerController.move == 1)
        {
            facingRight = true;
        }
        else if (playerController.move == -1)
        {
            facingRight = false;
        }


        if (!facingRight)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;

        }


    }



    public void climb()
    {

    }

    public bool testForDodging()
    {

        if (playerController.dodge)
        {
            playerController.revokeMovementPlayerControl(0.5f, this);
            return true;
        }
        return false;
    }

    public bool testForJumping()
    {
        if (playerController.jump && canJump && !canClimbObject)
        {
            return true;
        }

        return false;
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

    public Vector2 walk(float move)
    {

        return Physics.movementByForce(force, 1f, maxVelocity, move, this.rigidBody, false);

    }

    public Vector2 jog(float move)
    {

        return Physics.movementByForce(force, 1.5f, maxVelocity, move, this.rigidBody, false);

    }

    public Vector2 run(float move)
    {

        return Physics.movementByForce(force, 2f, maxVelocity, move, this.rigidBody, false);

    }


    public Vector2 crouchWalk(float move)
    {

        return Physics.movementByForce(force, 0.75f, maxVelocity, move, this.rigidBody, false);

    }


    public void crouch()
    {

        if (boxCollider.size.y > crouchHeigth)
        {
            boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y - crouchingSpeed);
            boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - (crouchingSpeed / 2));
            isCrouching = true;
        }


    }

    public void raise()
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
        //playerController.revokeMovementPlayerControl();

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
        return Physics.movementByForce(force, 0.50f, maxVelocity, move, this.rigidBody, true);
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

        int layerMask = LayerMask.GetMask(new string[] { "Ground" });
        raycastHit2DPoints.bottomLeftRay = Physics2D.Raycast(raycastPoints.bottomLeft, Vector2.down, 0.1f, layerMask);
        raycastHit2DPoints.bottomRightRay = Physics2D.Raycast(raycastPoints.bottomRight, Vector2.down, 0.1f, layerMask);
        raycastHit2DPoints.bottomMidRay = Physics2D.Raycast(raycastPoints.bottomMid, Vector2.down, 0.1f, layerMask);

    }

    public bool checkGroundForJump()
    {

        if (raycastHit2DPoints.bottomMidRay.collider != null && raycastHit2DPoints.bottomMidRay.distance <= 0.10f)
        {
            return true;
        }
        else if (raycastHit2DPoints.bottomLeftRay.collider != null && raycastHit2DPoints.bottomLeftRay.distance <= 0.10f)
        {
            Physics.addImpulseForce(forceOnEdge, rigidBody);
        }
        else if (raycastHit2DPoints.bottomRightRay.collider != null && raycastHit2DPoints.bottomRightRay.distance <= 0.10f)
        {
            Physics.addImpulseForce(-forceOnEdge, rigidBody);
        }

        return false;
    }


    public void revokeControlOnStairs(float time)
    {
        playerController.revokeMovementPlayerControl(time, this);
    }

    public void giveControlOnStairs()
    {
        playerController.revokeMovementPlayerControl(0.5f, this);
    }

    public void setOnStairs(bool isOnStairs)
    {
        this.isOnStairs = isOnStairs;
    }

    public void setCanClimbStairs(bool canClimbStairs)
    {
        this.canClimbStairs = canClimbStairs;
    }

    public void setCanClimbObject(bool canClimbObject)
    {
        this.canClimbObject = canClimbObject;
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
