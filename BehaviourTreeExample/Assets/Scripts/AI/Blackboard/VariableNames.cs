using System;
public enum State
{
    PATROLLING = 0,
    SEARCHING = 1,
    CHASING = 2,
    ATTACKING = 3
}

public static class VariableNames
{
    public const string TARGET_POSITION = "TARGET_POSITION";
    public const string TARGET_POSITION_ALTERNATE = "TARGET_POSITION_ALTERNATE";
    public const string TARGET_POSITION_PLAYER = "TARGET_POSITION_PLAYER";
    public const string HAS_WEAPON = "HAS_WEAPON";
    public const string IS_SEARCHING = "IS_SEARCHING";
    public const string STATE = "STATE";
    public const string SEES_PLAYER = "SEES_PLAYER";
}
