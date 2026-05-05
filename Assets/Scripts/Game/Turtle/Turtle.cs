using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class Turtle : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;
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

    public void MoveTo(Vector3 targetPosition, bool comeBack=false)
    {
        if(comeBack)
        {
            Vector3 originalPosition = transform.position;
            transform.DOMove(targetPosition, 0.25f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.DOMove(originalPosition, 0.25f).SetEase(Ease.InOutSine);
            });
        }
        else
            transform.DOMove(targetPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() => {
                GridCell cell = GetComponentInParent<GridCell>();
                if (cell != null)
                {
                    cell.Turtle = null;
                }
                Debug.Log("Turtle moved to inventory");
            });
    }   

}
