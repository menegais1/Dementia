public enum HorizontalMovementState
{
    Idle,
    Walking,
    Jogging,
    Running,
    CrouchIdle,
    CrouchWalking,
    Dodging,
}

public enum VerticalMovementState
{
    Idle,
    Jumping,
    OnAir,
    ClimbingLadder,
    ClimbingObstacle,
}

public enum ControlTypeToRevoke
{
    HorizontalMovement,
    VerticalMovement,
    MiscellaneousMovement,
    AllMovement,
}


public enum FacingDirection
{
    Right,
    Left,
}