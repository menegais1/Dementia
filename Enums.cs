public enum HorizontalMovementState
{
    Idle,
    Walking,
    Jogging,
    Running,
    CrouchIdle,
    CrouchWalking,
}

public enum HorizontalPressMovementState
{
    None,
    Dodge,
}

public enum VerticalMovementState
{
    Grounded,
    OnAir,
    ClimbingLadder,
    ClimbingObstacle,
}

public enum VerticalPressMovementState
{
    None,
    Jump,
    ClimbLadder,
    ClimbObstacle,
}

public enum ControlTypeToRevoke
{
    HorizontalMovement,
    VerticalMovement,
    StairsMovement,
    MiscellaneousMovement,
    AllMovement,
}


public enum FacingDirection
{
    Right,
    Left,
}