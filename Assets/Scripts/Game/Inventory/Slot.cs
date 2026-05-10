using DG.Tweening;
using System;
using UnityEngine;
public class Slot : MonoBehaviour
{
    public Turtle Turtle { get; private set; }
    public bool IsEmpty => Turtle == null;
    public void SetTurtle(Turtle newTurtle)
    {
        Turtle = newTurtle;
        newTurtle.transform.position = transform.position;
        newTurtle.transform.SetParent(transform);
    }

    public void ClearSlot()
    {
        if (Turtle != null)
        {
            Turtle.transform.SetParent(null);
            Turtle = null;
        }
    }
}
