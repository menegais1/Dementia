/*using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Structs & Enums

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

    private RaycastHit2DPoints raycastHit2DPoints;

    private RaycastPoints raycastPoints;

    #endregion

    #region Váriaveis Gerais

    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _force;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _forceOnEdge;
    [SerializeField] private float _cameraZoomSize;
    [SerializeField] private float _stairsSmoothness;
    [SerializeField] private float _baseStaminaSpent;
    [SerializeField] private float _dodgeForce;
    [SerializeField] private float _crouchingSpeed;
    [SerializeField] private float _crouchHeigth;


    public bool SnapToPositionRan { get; set; }

    public bool CameraState { get; set; }

    public bool CanJump { get; set; }

    public bool CanSwim { get; set; }

    public bool CanClimb { get; set; }

    public bool CanClimbStairs { get; set; }

    public int MovementState { get; set; }

    public bool CanClimbObject { get; set; }

    public bool CoroutineEndedRunning { get; set; }

    public bool LeaveStairs { get; set; }

    public bool FacingRight { get; set; }

    public float CurrentGravityScale { get; set; }

    public float RunningPressingTime { get; set; }

//    public CameraController CharacterCamera { get; set; }

    public float CharacterHeigth { get; set; }

    public float DetectionDistance { get; set; }

    public Vector2 ForceApplied { get; set; }

    public Rigidbody2D RigidBody { get; set; }

    public BoxCollider2D BoxCollider { get; set; }

    public Player Player { get; set; }

    public PlayerController PlayerController { get; set; }

    #region Properties

    public float CameraZoomSize
    {
        get { return _cameraZoomSize; }
        set { _cameraZoomSize = value; }
    }

    public float DodgeForce
    {
        get { return _dodgeForce; }
        set { _dodgeForce = value; }
    }

    public float CrouchingSpeed
    {
        get { return _crouchingSpeed; }
        set { _crouchingSpeed = value; }
    }

    public float CrouchHeigth
    {
        get { return _crouchHeigth; }
        set { _crouchHeigth = value; }
    }

    public float StairsSmoothness
    {
        get { return _stairsSmoothness; }
        set { _stairsSmoothness = value; }
    }


    public float BaseStaminaSpent
    {
        get { return _baseStaminaSpent; }
        set { _baseStaminaSpent = value; }
    }

    public float ForceOnEdge
    {
        get { return _forceOnEdge; }
        set { _forceOnEdge = value; }
    }

    public float MaxVelocity
    {
        get { return _maxVelocity; }
        set { _maxVelocity = value; }
    }

    public float Force
    {
        get { return _force; }
        set { _force = value; }
    }

    public float JumpForce
    {
        get { return _jumpForce; }
        set { _jumpForce = value; }
    }

    #endregion

    #endregion

    #region Métodos Unity

    void Start()
    {
        PlayerController = PlayerController.getInstance();
        RigidBody = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();
        Player = GetComponent<Player>();
        MovementState = (int) MovementStateENUM.IDLE;

        ForceApplied = new Vector2(0, 0);
        CharacterHeigth = BoxCollider.size.y;

        FacingRight = true;
        CurrentGravityScale = RigidBody.gravityScale;
    }


    private void Update()
    {
        checkFacingDirection();
        PlayerController.checkForPlayerInput();
        setRaycastPoints();
        castRays();
        //   checkGround();

        CanJump = checkGroundForJump();
        PlayerStatusVariables.isDodging = testForDodging();
        PlayerStatusVariables.isJumping = testForJumping();
        PlayerStatusVariables.isOnAir = !CanJump && !PlayerStatusVariables.isClimbingStairs &&
                                        !PlayerStatusVariables.isClimbingObject || PlayerStatusVariables.isOnAir;

        if (CanClimbStairs)
        {
            if (PlayerController.climbStairsPress && !PlayerStatusVariables.isClimbingStairs)
            {
                PlayerStatusVariables.isClimbingStairs = true;
                PlayerController.changeMovementPlayerControl(true);
            }
        }

        if (PlayerStatusVariables.isClimbingStairs &&
            MathHelpers.Approximately(PlayerController.climbStairsMovement, 0, float.Epsilon))
        {
            resetVelocityY();
        }

        if (PlayerStatusVariables.isClimbingStairs && CanJump && CoroutineEndedRunning)
        {
            LeaveStairs = true;
            PlayerStatusVariables.isClimbingStairs = false;
            switchGravity(true);
            CoroutineEndedRunning = false;
            SnapToPositionRan = false;
            PlayerController.changeMovementPlayerControl(false);
            revokeControlOnStairs(0.5f);
            resetVelocityY();
        }

        if (PlayerController.takeOfCamera)
        {
            CameraState = !CameraState;
            takeOfCamera();
        }

        if (PlayerStatusVariables.isJumping)
        {
            if (Player.checkStamina(BaseStaminaSpent * 4f))
            {
                jump();
                Player.spendStamina(BaseStaminaSpent * 4f);
            }
        }

        if (CanClimbObject && PlayerController.jump)
        {
            PlayerStatusVariables.isClimbingObject = true;
            switchGravity(false);
        }

        if (PlayerStatusVariables.isClimbingObject && CoroutineEndedRunning)
        {
            PlayerStatusVariables.isClimbingObject = false;
            CoroutineEndedRunning = false;
            SnapToPositionRan = false;
            switchGravity(true);
        }

        if (PlayerController.jog && RunningPressingTime <= 0.5)
        {
            PlayerStatusVariables.isJogging = !PlayerStatusVariables.isJogging;
            RunningPressingTime = 0;
        }
        else if (PlayerController.jog)
        {
            RunningPressingTime = 0;
        }

        if (PlayerController.crouch)
        {
            crouch();
            if (MathHelpers.Approximately(BoxCollider.size.y, CrouchHeigth, float.Epsilon))
            {
                MovementState = (int) MovementStateENUM.CROUCHING;
            }
        }
        else if (PlayerStatusVariables.isCrouching)
        {
            raise();
            MovementState = (int) MovementStateENUM.IDLE;
        }


        if (!PlayerStatusVariables.isOnAir && !PlayerStatusVariables.isDodging &&
            !PlayerStatusVariables.isClimbingStairs)
        {
            if (!MathHelpers.Approximately(PlayerController.horizontalMove, 0, float.Epsilon))
            {
                if (PlayerStatusVariables.isCrouching)
                {
                    MovementState = (int) MovementStateENUM.CROUCHING;
                }
                else if (PlayerStatusVariables.isJogging)
                {
                    if (Player.checkStamina(BaseStaminaSpent * 1.5f))
                    {
                        MovementState = (int) MovementStateENUM.JOGGING;
                    }
                    else
                    {
                        MovementState = (int) MovementStateENUM.WALKING;
                    }
                }
                if (PlayerController.run)
                {
                    MovementState = (RunningPressingTime >= 0.5 || MovementState == (int) MovementStateENUM.IDLE)
                        ? (int) MovementStateENUM.RUNNING
                        : MovementState;
                    RunningPressingTime += Time.deltaTime;
                    if (MovementState == (int) MovementStateENUM.RUNNING)
                    {
                        if (!Player.checkStamina(BaseStaminaSpent * 2f))
                        {
                            MovementState = (int) MovementStateENUM.WALKING;
                        }
                    }
                }

                if (!PlayerStatusVariables.isJogging && !PlayerStatusVariables.isCrouching && !PlayerController.run)
                {
                    MovementState = (int) MovementStateENUM.WALKING;
                }
            }
            else
            {
                MovementState = (int) MovementStateENUM.IDLE;
            }
        }
        else if (PlayerStatusVariables.isClimbingStairs)
        {
            switchGravity(false);
            MovementState = (int) MovementStateENUM.CLIMBING_STAIRS;
        }
        else if (PlayerStatusVariables.isOnAir)
        {
            MovementState = (int) MovementStateENUM.ON_AIR;
        }
        else if (PlayerStatusVariables.isDodging)
        {
            MovementState = (int) MovementStateENUM.DODGING;
        }
    }

    void FixedUpdate()
    {
        switch (MovementState)
        {
            case (int) MovementStateENUM.WALKING:
                ForceApplied = walk(PlayerController.horizontalMove);
                break;
            case (int) MovementStateENUM.JOGGING:
                ForceApplied = jog(PlayerController.horizontalMove);
                Player.spendStamina(BaseStaminaSpent * 1.5f * Time.deltaTime);
                break;
            case (int) MovementStateENUM.RUNNING:
                ForceApplied = run(PlayerController.horizontalMove);
                Player.spendStamina(BaseStaminaSpent * 2f * Time.deltaTime);
                break;
            case (int) MovementStateENUM.CROUCHING:
                ForceApplied = crouchWalk(PlayerController.horizontalMove);
                break;
            case (int) MovementStateENUM.DODGING:
                if (Player.checkStamina(Player.maxStamina))
                {
                    ForceApplied = dodge();
                    Player.spendStamina(Player.maxStamina);
                }
                break;
            case (int) MovementStateENUM.CLIMBING_STAIRS:
                ForceApplied = climbStairs(PlayerController.climbStairsMovement);
                break;
            //case (int)MovementStateENUM.JUMPING:
            //    jump();
            //    break;
            case (int) MovementStateENUM.ON_AIR:
                PlayerController.revokeMovementPlayerControl();
                break;
        }


        //Diminuir deslizada
        PhysicsHelpers.addContraryForce(ForceApplied, RigidBody, MovementState);
    }

    void LateUpdate()
    {
        if (MathHelpers.Approximately(RigidBody.velocity.y, 0, float.Epsilon) && PlayerStatusVariables.isOnAir &&
            CanJump)
        {
            PlayerStatusVariables.isOnAir = false;
            PlayerController.giveMovementPlayerControlWithCooldown(0.1f, this);
        }
    }

    #endregion

    #region Métodos Gerais

    public void snapToPositionStairs(Vector2 position)
    {
        resetVelocityX();
        resetVelocityY();
        PlayerController.revokeMovementPlayerControl();
        StartCoroutine(snapToPositionStairsCoroutine(position));
        SnapToPositionRan = true;
    }

    public void snapToPositionObject(Vector2 position)
    {
        resetVelocityX();
        resetVelocityY();
        PlayerController.revokeMovementPlayerControl();
        StartCoroutine(snapToPositionObjectCoroutine(position));
        SnapToPositionRan = true;
    }

    public bool checkPositionX(Vector2 position)
    {
        if (Mathf.Abs(RigidBody.position.x - position.x) <= 0.1)
        {
            return true;
        }
        return false;
    }


    private IEnumerator snapToPositionStairsCoroutine(Vector2 position)
    {
        while (Mathf.Abs(RigidBody.position.x - position.x) >= 0.01 ||
               Mathf.Abs(RigidBody.position.y - position.y) >= 0.01)
        {
            RigidBody.position =
                Vector2.Lerp(RigidBody.position, new Vector2(position.x, position.y), StairsSmoothness);
            yield return new WaitForEndOfFrame();
        }
        CoroutineEndedRunning = true;
        PlayerController.giveMovementPlayerControl();
    }

    private IEnumerator snapToPositionObjectCoroutine(Vector2 position)
    {
        float localScale = (position.x > RigidBody.position.x) ? +transform.localScale.x : -transform.localScale.x;
        while (Mathf.Abs(RigidBody.position.x - (position.x + localScale)) >= 0.01 ||
               Mathf.Abs(RigidBody.position.y - (position.y + transform.localScale.y)) >= 0.01)
        {
            if (Mathf.Abs(RigidBody.position.y - (position.y + transform.localScale.y)) >= 0.01)
            {
                RigidBody.position = Vector2.Lerp(RigidBody.position,
                    new Vector2(RigidBody.position.x, (position.y + transform.localScale.y)), StairsSmoothness);
            }
            else
            {
                RigidBody.position = Vector2.Lerp(RigidBody.position,
                    new Vector2(position.x + localScale, RigidBody.position.y), StairsSmoothness);
            }
            yield return new WaitForEndOfFrame();
        }
        CoroutineEndedRunning = true;
        PlayerController.giveMovementPlayerControl();
    }

    public void ignoreCollision(Collider2D other, bool ignore)
    {
        Physics2D.IgnoreCollision(BoxCollider, other, ignore);
    }

    public void checkFacingDirection()
    {
        if (MathHelpers.Approximately(PlayerController.horizontalMove, 1, float.Epsilon))
        {
            FacingRight = true;
        }
        else if (MathHelpers.Approximately(PlayerController.horizontalMove, -1, float.Epsilon))
        {
            FacingRight = false;
        }


        GetComponent<SpriteRenderer>().flipX = !FacingRight;
    }

    public void climb()
    {
    }

    public bool testForDodging()
    {
        if (PlayerController.dodge)
        {
            PlayerController.RevokeHorizontalMovementPlayerControl(0.5f, this);
            return true;
        }
        return false;
    }

    public bool testForJumping()
    {
        if (PlayerController.jump && CanJump && !CanClimbObject)
        {
            return true;
        }

        return false;
    }

    public void switchGravity(bool on)
    {
        RigidBody.gravityScale = on ? CurrentGravityScale : 0;
    }

    public void takeOfCamera()
    {
        StartCoroutine(CameraController.takeOfCamera(CameraZoomSize, 100, CameraState));
    }

    public Vector2 walk(float horizontalMove)
    {
        return PhysicsHelpers.movementByForce(Force, 1f, MaxVelocity, horizontalMove, RigidBody, false);
    }

    public Vector2 jog(float horizontalMove)
    {
        return PhysicsHelpers.movementByForce(Force, 1.5f, MaxVelocity, horizontalMove, RigidBody, false);
    }

    public Vector2 run(float horizontalMove)
    {
        return PhysicsHelpers.movementByForce(Force, 2f, MaxVelocity, horizontalMove, RigidBody, false);
    }

    public Vector2 crouchWalk(float horizontalMove)
    {
        return PhysicsHelpers.movementByForce(Force, 0.75f, MaxVelocity, horizontalMove, RigidBody, false);
    }

    public void crouch()
    {
        if (BoxCollider.size.y > CrouchHeigth)
        {
            BoxCollider.size = new Vector2(BoxCollider.size.x, BoxCollider.size.y - CrouchingSpeed);
            BoxCollider.offset = new Vector2(BoxCollider.offset.x, BoxCollider.offset.y - (CrouchingSpeed / 2));
            PlayerStatusVariables.isCrouching = true;
        }
    }

    public void raise()
    {
        if (BoxCollider.size.y < CharacterHeigth)
        {
            BoxCollider.size = new Vector2(BoxCollider.size.x, BoxCollider.size.y + CrouchingSpeed);
            BoxCollider.offset = new Vector2(BoxCollider.offset.x, BoxCollider.offset.y + (CrouchingSpeed / 2));
        }
        else
        {
            PlayerStatusVariables.isCrouching = false;
        }
    }


    public void jump()
    {
        PhysicsHelpers.jump(JumpForce, RigidBody);
        PlayerStatusVariables.isOnAir = true;
        //playerController.revokeMovementPlayerControl();
    }

    public Vector2 dodge()
    {
        //Adicionar a colisão com inimigos depois

        //Zerar velocidade para desviar a mesma distância
        resetVelocityX();

        //Vector2 forceApplied = PhysicsHelpers.AddImpulseForce(DodgeForce, RigidBody, FacingRight);
        PlayerStatusVariables.isDodging = false;
        //return forceApplied;
        return Vector2.zero;
    }

    public Vector2 climbStairs(float horizontalMove)
    {
        return PhysicsHelpers.movementByForce(Force, 0.50f, MaxVelocity, horizontalMove, RigidBody, true);
    }

    public void resetVelocityY()
    {
        RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0);
    }

    public void resetVelocityX()
    {
        RigidBody.velocity = new Vector2(0, RigidBody.velocity.y);
    }

    public void setRaycastPoints()
    {
        raycastPoints.bottomLeft = new Vector2(BoxCollider.bounds.min.x, BoxCollider.bounds.min.y);
        raycastPoints.bottomRight = new Vector2(BoxCollider.bounds.max.x, BoxCollider.bounds.min.y);
        raycastPoints.bottomMid = raycastPoints.bottomLeft + (Vector2.right * BoxCollider.size.x / 2);


        Debug.DrawRay(raycastPoints.bottomLeft, Vector2.down, Color.green);
        Debug.DrawRay(raycastPoints.bottomRight, Vector2.down, Color.red);
        Debug.DrawRay(raycastPoints.bottomMid, Vector2.down, Color.blue);
    }

    public void castRays()
    {
        int layerMask = LayerMask.GetMask("Ground");
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
        return false;
    }

    public void checkGround()
    {
        if (raycastHit2DPoints.bottomMidRay.collider != null) return;
        if (raycastHit2DPoints.bottomLeftRay.collider != null)
        {
            PhysicsHelpers.addImpulseForce(ForceOnEdge, RigidBody);
        }
        else if (raycastHit2DPoints.bottomRightRay.collider != null)
        {
            PhysicsHelpers.addImpulseForce(-ForceOnEdge, RigidBody);
        }
    }

    public void revokeControlOnStairs(float time)
    {
        PlayerController.RevokeHorizontalMovementPlayerControl(time, this);
    }

    public void giveControlOnStairs()
    {
        PlayerController.RevokeHorizontalMovementPlayerControl(0.5f, this);
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
    //    //horizontalMove = horizontalMove * (angle / 10);
    //}

    //Debug.DrawRay(new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y), Vector2.down, Color.red);
    ////Fim da Revisão
}*/