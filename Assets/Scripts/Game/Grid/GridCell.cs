using UnityEngine;

public class GridCell : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Turtle Turtle { get; set; }

    public bool IsOccupied => Turtle != null;
}
