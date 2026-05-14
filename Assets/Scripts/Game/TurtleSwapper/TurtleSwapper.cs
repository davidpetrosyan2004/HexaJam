using DG.Tweening;
using System;
using UnityEngine;
using static GridData;

public class TurtleSwapper : Turtle
{
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private Collider colliderSpawner;

    private void Start()
    {
        colliderSpawner = GetComponent<Collider>();
    }
    public void Swap()
    {
        var (cell1, cell2) = GetTwoGridCellsToSwap();

        if (cell1 == null || cell2 == null)
        {
            Debug.Log("Null Cells");
            return;
        }

        if (cell1.Turtle == null && cell2.Turtle == null)
        {
            Debug.Log("Both turtles are null");
            return;
        }
        Quaternion initRot1 = Quaternion.identity;
        Quaternion initRot2 = Quaternion.identity;
        (initRot1, initRot2) = GetInitRotations(cell1, cell2);
        SwapAnimation(cell1, cell2);
        SetInitRotations(cell1, cell2, initRot1, initRot2);
    }

    public (GridCell, GridCell) GetTwoGridCellsToSwap()
    {
        GridCell gridCell1 = null;
        GridCell gridCell2 = null;

        Ray ray1 = new Ray(point1.transform.position,-point1.transform.forward);
        Debug.DrawLine(
    ray1.origin,
    ray1.origin + ray1.direction * 10f,
    Color.red, 10f
);
        if (Physics.Raycast(ray1, out RaycastHit hit1))
        {
            if (hit1.collider.CompareTag("GridCell"))
            {
                gridCell1 = hit1.collider.GetComponentInParent<GridCell>();
                if (gridCell1 == null) Debug.Log("gridCell 1 is NUll");
                
                Debug.Log("gridCell1 selected");

            }
            else
            {
                Debug.Log("gridCell2 does not exists");
            }
        }

        Ray ray2 = new Ray(point2.transform.position, -point2.transform.forward);
        Debug.DrawLine(
            ray2.origin,
            ray2.origin + ray2.direction * 10f,
            Color.red, 10f
        );
        if (Physics.Raycast(ray2, out RaycastHit hit2))
        {
            if (hit2.collider.CompareTag("GridCell"))
            {
                gridCell2 = hit2.collider.GetComponentInParent<GridCell>();
                if (gridCell2 == null) Debug.Log("gridCell 2 is NUll");
                Debug.Log("gridCell2 selected");

            }
            else
            {
                Debug.Log("gridCell2 does not exists");
            }
        }
        else
        {
            Debug.Log("does not hit anything");
        }

        return (gridCell1, gridCell2);
    }

    public (Quaternion, Quaternion) GetInitRotations(GridCell cell1, GridCell cell2)
    {
        Turtle turtle1 = cell1.Turtle;
        Quaternion rot1 = Quaternion.identity;
        Quaternion rot2 = Quaternion.identity;
        if (turtle1 != null)
            rot1 = turtle1.transform.GetChild(0).localRotation;
        Turtle turtle2 = cell2.Turtle;
        if (turtle2 != null)
            rot2 = turtle2.transform.GetChild(0).localRotation;

        return (rot1, rot2);
    }

    public void SwapTurtles(GridCell cell1, GridCell cell2)
    {
        Debug.Log("Swap Turtles");
        Turtle turtle1 = cell1.Turtle;
        if (turtle1 != null) 
        {
            turtle1.transform.SetParent(point1, true);
        }

        Turtle turtle2 = cell2.Turtle;
        if (turtle2 != null)
        {
            turtle2.transform.SetParent(point2, true);
        }

        Turtle temp = cell1.Turtle;

        cell1.Turtle = cell2.Turtle;
        cell2.Turtle = temp;
    }

    public void SwapAnimation(GridCell cell1, GridCell cell2)
    {
        SwapTurtles(cell1, cell2);
        colliderSpawner.enabled = false;
        AudioManager.Instance.PlaySound("ButtonClick");
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DORotate(new Vector3(0, 0, 180), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack).OnComplete(() =>
        {
            PutDownTurtles(cell1, cell2);
            colliderSpawner.enabled = true;
        }));
        sequence.Join(transform.DOScaleZ(30f, 0.15f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo));
    }

    public void PutDownTurtles(GridCell cell1, GridCell cell2)
    {
        Debug.Log("PutDownTurtles");

        if (cell1.Turtle != null)
        {
            cell1.Turtle.transform.SetParent(cell1.transform);
        }

        if (cell2.Turtle != null)
        {
            cell2.Turtle.transform.SetParent(cell2.transform);
        }
    }

    public void SetInitRotations(GridCell cell1, GridCell cell2, Quaternion initRot1, Quaternion initRot2)
    {
        Turtle turtle1 = cell1.Turtle;
        if (turtle1 != null)
        {
            turtle1.transform.rotation = Quaternion.Euler(initRot1.x - 90f, initRot1.y + 120, initRot1.z + 90f);
        }
        Turtle turtle2 = cell2.Turtle;
        if (turtle2 != null)
        {
            turtle2.transform.rotation = Quaternion.Euler(initRot2.x - 90f, initRot2.y + 120, initRot2.z + 90f);
        }
    }
}
