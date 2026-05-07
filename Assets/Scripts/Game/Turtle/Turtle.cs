using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using static GridData;
public class Turtle : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] float speed = 10f;
    public Texture Texture
    {
        get 
        {
            return meshRenderer.material.GetTexture("_BaseMap");
        } 
        set
        {
            meshRenderer.material.SetTexture("_BaseMap", value);
        }
    }
    public void MoveTo(Vector3 targetPosition, bool comeBack = false)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(targetPosition, comeBack));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, bool comeBack)
    {
        //Vector3 originalPosition = transform.position;

        //while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        //{
        //    transform.position = Vector3.MoveTowards(
        //        transform.position,
        //        targetPosition,
        //        speed * Time.deltaTime
        //    );

        //    yield return null;
        //}

        //transform.position = targetPosition;

        if (comeBack)
        {
            //while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
            //{
            //    transform.position = Vector3.MoveTowards(
            //        transform.position,
            //        originalPosition,
            //        speed * Time.deltaTime
            //    );

            //    yield return null;
            //}

            //transform.position = originalPosition;
            transform.DOMove(targetPosition, 0.5f).SetLoops(2, LoopType.Yoyo);
        }
        else
        {
            transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InOutCubic).OnComplete(
                () =>
                {    GridCell cell = GetComponentInParent<GridCell>();
                if (cell != null)
                {
                    cell.Turtle = null;
                }

                GameEvents.OnTurtleAddedInventory?.Invoke(this);
                    Debug.Log("Turtle moved to inventory");
                }
                );

            
        }
        yield return null;
    }
}
