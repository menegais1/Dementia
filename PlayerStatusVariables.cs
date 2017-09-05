public class PlayerStatusVariables
{
    private static PlayerStatusVariables instance;

    public bool IsJogging { get; set; }

    public bool IsOnAir { get; set; }

    public bool IsJumping { get; set; }

    public bool IsClimbingStairs { get; set; }

    public bool IsClimbingObject { get; set; }

    public bool IsCrouching { get; set; }

    public bool IsDodging { get; set; }

    public bool IsOnStairs { get; set; }


    public static PlayerStatusVariables getInstance()
    {
        if (instance == null)
        {
            instance = new PlayerStatusVariables();
        }

        return instance;
    }

    private PlayerStatusVariables()
    {
        IsJogging = IsClimbingObject =
            IsClimbingStairs = IsCrouching =
                IsDodging = IsOnStairs =
                    IsOnAir = IsJumping = false;
    }
}