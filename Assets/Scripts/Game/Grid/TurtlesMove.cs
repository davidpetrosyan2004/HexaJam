using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
public class TurtlesMove : MonoBehaviour
{
    private bool oneTurtleMovingWrong = false;
    private int wrongMovingCount = 0;
    private Vector3 rayOffset = new Vector3(0, 0.3f, 0);
    [SerializeField] private List<Turtle> turtlesToMove;
    [SerializeField] private Inventory inventory;
    private void OnEnable()
    {
        GameEvents.OnTurtlePressed += AddTurtle;
        GameEvents.OnTurtleMovingWrong += OnTurtleMovingWrong;
    }

    private void OnDisable()
    {
        GameEvents.OnTurtlePressed -= AddTurtle;
        GameEvents.OnTurtleMovingWrong -= OnTurtleMovingWrong;
    }

    private void OnTurtleMovingWrong(bool isMovingWrong)
    {
        if (isMovingWrong)
            wrongMovingCount++;
        else
            wrongMovingCount--;

        oneTurtleMovingWrong = wrongMovingCount > 0;
    }
    private void AddTurtle(Turtle turtle)
    {
        
        if (!inventory.HasFreeSlot() &&
!inventory.IsResolvingMatches)
        {
            Debug.Log(inventory.hasFreeSlot);
            return;
        }
        if (turtle == null || !turtle.gameObject) return;

        if (oneTurtleMovingWrong) return;
        if (!inventory.hasFreeSlot) return;
        var (target, comeBack) = GetTurtlePositionInFrontOf(turtle);

        if (!comeBack) inventory.ChangeSlotsCount(-1);
        StartCoroutine(turtle.MoveTo(target, comeBack));
    }

    public (Vector3, bool) GetTurtlePositionInFrontOf(Turtle turtle)
    {
        Ray ray = new Ray(
            turtle.transform.position - rayOffset,
             -turtle.transform.GetChild(0).forward
        );
        RaycastHit[] hits = Physics.RaycastAll(ray, 30f);
        if (hits.Length == 0)
            return (turtle.transform.position, false);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        GridCell lastValidCell = null;
        foreach (var hit in hits)
        {
            if (!hit.collider.CompareTag("GridCell"))
            {
                continue;
            }

            GridCell cell = hit.collider.GetComponentInParent<GridCell>();
            if (cell == null)
                continue;

            if (cell.transform.position == turtle.transform.position)
                continue;

            if (cell.Turtle != null)
            {
                return (cell.transform.position + rayOffset, true);
            }
            lastValidCell = cell;
        }
        if (lastValidCell != null)
            return (lastValidCell.transform.position + rayOffset, false);

        return (turtle.transform.position, false);
    }
}
