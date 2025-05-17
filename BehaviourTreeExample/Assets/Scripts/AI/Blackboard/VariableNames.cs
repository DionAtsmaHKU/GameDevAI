public enum State
{
    PATROLLING = 0,
    SEARCHING = 1,
    CHASING = 2,
    ATTACKING = 3,
    FOLLOWING = 4,
    DEFENDING = 5
}

public static class VariableNames
{
    public const string TARGET_POSITION_A = "TARGET_POSITION_A";
    public const string TARGET_POSITION_B = "TARGET_POSITION_B";
    public const string TARGET_POSITION_C = "TARGET_POSITION_C"; // Im not sure if this is the best way to fill in the waypoints.
    public const string TARGET_POSITION_D = "TARGET_POSITION_D";
    public const string TARGET_POSITION_WEAPON = "TARGET_POSITION_WEAPON";
    public const string TARGET_POSITION_PLAYER = "TARGET_POSITION_PLAYER";
    public const string TARGET_POSITION_COVER = "TARGET_POSITION_COVER";
    public const string HAS_WEAPON = "HAS_WEAPON";
    public const string IS_SEARCHING = "IS_SEARCHING";
    public const string IS_STUNNED = "IS_STUNNED";
    public const string STATE = "STATE";
    public const string SEES_PLAYER = "SEES_PLAYER";
    public const string PLAYER_DEAD = "PLAYER_DEAD";
}
