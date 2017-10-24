public enum HorizontalMovementState
{
    Idle,
    Walking,
    Jogging,
    Running,
    CrouchIdle,
    CrouchWalking,
}

public enum CombatMovementState
{
    Aiming,
    None,
}

public enum StairsTriggerType
{
    TopTrigger,
    BottomTrigger,
    GeneralTrigger,
}

public enum LadderType
{
    Ladder,
    BottomLadder,
    TopLadder,
}

public enum HorizontalPressMovementState
{
    None,
    Dodge,
}

public enum CombatPressMovementState
{
    None,
    Shoot,
    Reload,
    Cqc,
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

public enum MiscellaneousPressMovementState
{
    None,
    TakeItem,
    InteractWithScenery,
    ZoomCamera,
}

public enum ControlTypeToRevoke
{
    HorizontalMovement,
    VerticalMovement,
    LadderMovement,
    MiscellaneousMovement,
    CombatMovement,
    AllMovement,
}


public enum FacingDirection
{
    Right,
    Left,
}

public enum AimDirection
{
    Right,
    Up,
    Down,
    Left,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft,
}