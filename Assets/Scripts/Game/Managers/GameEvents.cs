using System;
using System.Numerics;

public static class GameEvents
{
    public static Action<Turtle> OnTurtleAddedInventory;
    public static Action<Turtle> OnTurtlePressed;
    public static Action<bool> OnTurtleMovingWrong;
    public static Action<int> OnTurtlesCountTextSet;
    public static Action OnTurtlesCompleted;
    public static Action OnTurtleDissapear;
    public static Action OnTurtlesSubstract;
    public static Action<bool> OnGameCondition;
    public static Action OnCameraShake;
}
