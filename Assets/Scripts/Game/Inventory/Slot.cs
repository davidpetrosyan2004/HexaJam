using DG.Tweening;
using System;
using UnityEngine;
public class Slot : MonoBehaviour
{
    public Turtle Turtle { get; private set; }
    public bool IsEmpty => Turtle == null;
    public MeshRenderer meshRenderer;
    private Color meshColor;
    public void SetTurtle(Turtle newTurtle)
    {
        Turtle = newTurtle;
        newTurtle.transform.SetParent(transform);
    }
    private void Start()
    {
        meshColor = meshRenderer.material.color;
    }
    public void SetInitColor()
    {
        meshRenderer.material.color = meshColor;
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
