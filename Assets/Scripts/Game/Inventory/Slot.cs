using UnityEngine;

public class Slot : MonoBehaviour
{
    public Turtle Turtle { get; private set; }
    public bool IsEmpty => Turtle == null;
    public void SetTurtle(Turtle newTurtle)
    {
        Turtle = newTurtle;
        Turtle.transform.position = transform.position;
        Turtle.transform.SetParent(transform);
    }
}
