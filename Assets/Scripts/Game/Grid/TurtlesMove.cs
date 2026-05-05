using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class TurtlesMove : MonoBehaviour
{
    private bool isMoving = false;
    private Vector3 rayOffset = new Vector3(0, 0.3f, 0);
    [SerializeField] private List<Turtle> turtlesToMove;

    private void OnEnable()
    {
        GameEvents.OnTurtlePressed += AddTurtle;
    }

    private void OnDisable()
    {
        GameEvents.OnTurtlePressed -= AddTurtle;
    }

    private void AddTurtle(Turtle turtle)
    {
        if (turtle == null || !turtle.gameObject) return;

        if (turtlesToMove.Contains(turtle)) return;

        turtlesToMove.Add(turtle);

        if (!isMoving)
            StartCoroutine(ProcessMoves());
    }
    private IEnumerator ProcessMoves()
    {
        isMoving = true;

        while (turtlesToMove.Count > 0)
        {
            var turtle = turtlesToMove[0];
            turtlesToMove.RemoveAt(0);

            var (target, comeBack) = GetTurtlePositionInFrontOf(turtle);
            turtle.MoveTo(target, comeBack);
            if (!comeBack) GameEvents.OnTurtleAddedInventory?.Invoke(turtle);

            yield return null;
        }

        isMoving = false;
    }

    public (Vector3, bool) GetTurtlePositionInFrontOf(Turtle turtle)
    {
        Ray ray = new Ray(
            turtle.transform.position - rayOffset,
            Quaternion.Euler(0, 120, 0) * turtle.transform.right
        );
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f, Color.red, 10f);
        RaycastHit[] hits = Physics.RaycastAll(ray, 2f);
        Debug.Log($"Raycast hits for turtle {turtle.name}: {hits.Length}");
        if (hits.Length == 0)
            return (turtle.transform.position, false);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        GridCell lastValidCell = null;
        foreach (var hit in hits)
        {
            if (!hit.collider.CompareTag("GridCell"))
            {
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
                return (cell.transform.position + rayOffset, true);
            }
            Debug.Log("No turtle in this cell, but it's valid: " + cell.transform.position);
            lastValidCell = cell;
        }
        Debug.Log("No turtle in front, last valid cell: " + (lastValidCell != null ? lastValidCell.transform.position.ToString() : "none"));
        if (lastValidCell != null)
            return (lastValidCell.transform.position + rayOffset, false);

        return (turtle.transform.position, false);
    }
}
