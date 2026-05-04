using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Turtle Turtle { get; set; }

    public bool IsOccupied => Turtle != null;
}
