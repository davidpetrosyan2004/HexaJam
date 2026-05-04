using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GridData;
public class Grid : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap board;

    [Header("Prefabs")]
    [SerializeField] private GridCell hexPrefab;
    [SerializeField] private Turtle turtlePrefab;
    [SerializeField] private GridData gridData;
    
    [Header("Characteristics")]
    [SerializeField] private Vector3 turtleOffset;
    [SerializeField] private Vector3 tileOffset;
    private float timeToMove = 0f;

    [Header("Lists")]
    [SerializeField] private List<Turtle> turtlesToMove;

    private void OnEnable()
    {
        GameEvents.OnTurtlePressed += AddTurtle;
    }

    private void Start()
    {
        GenerateGrid();
    }
    private void Update()
    {
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red);
        if (turtlesToMove.Count <= 0) return;
        if (timeToMove >= 0.5f)
        {
            MoveTurtles();
            timeToMove = 0f;
        }
        timeToMove += Time.deltaTime;
    }

    private void AddTurtle(Turtle turtle)
    {
        if(turtlesToMove.Contains(turtle)) return;
        turtlesToMove.Add(turtle);
    }

    private void MoveTurtles()
    {
        foreach (var turtle in turtlesToMove)
        {
            // Move the turtle
            Debug.Log("Moving turtle: " + turtle.name);
            Vector3 targetPosition = GetTurtlePositionInFrontOf(turtle);
            turtle.MoveTo(targetPosition, 0.5f);
        }
        turtlesToMove.Clear();
    }
    Ray ray;

    public Vector3 GetTurtlePositionInFrontOf(Turtle turtle)
    {
        Ray ray = new Ray(
            turtle.transform.position - turtleOffset,
            Quaternion.Euler(0, 90f, 0) * transform.right
        );
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 10f);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        Debug.Log($"Raycast hits for turtle {turtle.name}: {hits.Length}");
        if (hits.Length == 0)
            return turtle.transform.position;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        GridCell lastValidCell = null;
        foreach (var hit in hits)
        {
            if (!hit.collider.CompareTag("GridCell")){
                Debug.Log("Hit object that is not a GridCell: " + hit.collider.name);
                continue;
            }

            GridCell cell = hit.collider.GetComponentInParent<GridCell>();
            if (cell == null)
                continue;

            if (cell.transform.position == turtle.transform.position)
                continue;

            if (cell.Turtle != null)
            {
                Debug.Log($"Turtle in front at: {cell.transform.position}");
                return cell.transform.position + turtleOffset;
            }
            Debug.Log("No turtle in this cell, but it's valid: " + cell.transform.position);
            lastValidCell = cell;
        }
        Debug.Log("No turtle in front, last valid cell: " + (lastValidCell != null ? lastValidCell.transform.position.ToString() : "none"));
        if (lastValidCell != null)
            return lastValidCell.transform.position + turtleOffset;

        return turtle.transform.position;
    }

    public void GenerateGrid()
    {
        for (int i = 0; i < gridData.rows; i++)
        {
            for (int j = 0; j < gridData.columns; j++)
            {
                var pos = new Vector3Int(j, i, 0);
                PlaceTileTypeOnBoard(gridData.board[i].column[j].type, gridData.board[i].column[j].rotation,pos);
            }
        }
    }

    private void PlaceTileTypeOnBoard(GridData.CellType cellType, float rotation, Vector3Int pos)
    {
        var spawnPos = board.GetCellCenterWorld(pos);
        if (cellType == GridData.CellType.Empty)
        {
            return;
        }
        GridCell gridCell = Instantiate(hexPrefab, spawnPos + tileOffset, Quaternion.identity, board.transform);

        Color turtleColor = GetColorOfTurtle(cellType);
        if (turtleColor == Color.white) return;

        Turtle turtle = Instantiate(turtlePrefab, spawnPos + turtleOffset, Quaternion.Euler(0, rotation, 0), board.transform);
        turtle.Color = turtleColor;

        gridCell.Turtle = turtle;
    }

    public Color GetColorOfTurtle(GridData.CellType cellType)
    {
        if (cellType == GridData.CellType.Red)
        {
            return Color.red;
        }
        else if (cellType == GridData.CellType.Green)
        {
            return Color.green;
        }
        else if (cellType == GridData.CellType.Blue)
        {
            return Color.blue;
        }
        else if (cellType == GridData.CellType.Yellow)
        {
            return Color.yellow;
        }
        return Color.white;
    }
}
