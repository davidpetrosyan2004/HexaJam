using System;
using System.Numerics;

public static class GameEvents
{
    public static Action<Turtle> OnTurtlePressed;
    public static Action OnGameOver;
    public static Action OnTurtlesCompleted;
    public static Action<Turtle> OnTurtleAddedInventory;
    public static Action OnTurtleDissapear;
    public static Action<bool> OnTurtleMovingWrong;

}
