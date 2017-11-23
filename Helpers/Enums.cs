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
    UseItem,
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
    TakeWeapon,
    TakeNote,
    InteractWithScenery,
    ZoomCamera,
}

public enum Ratio
{
    FourByThree,
    SixteenByNine
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

public enum WeaponType
{
    Nothing,
    Revolver,
    Shotgun,
    MachineGun,
    Rifle,
}

public enum ItemType
{
    Nothing,
    RevolverBullet,
    Bandages,
    Molotov,
    Analgesics,
}

public enum DiaryTab
{
    Inventory,
    Diary,
    Menu,
    Nothing
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