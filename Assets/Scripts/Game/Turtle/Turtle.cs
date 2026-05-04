using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class Turtle : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;

    public Color Color
    {
        get 
        {
            return meshRenderer.material.color;
        } 
        set
        {
            meshRenderer.material.color = value; 
        }
    }

    public void MoveTo(Vector3 targetPosition, float duration)
    {
        transform.DOMove(targetPosition, duration).SetEase(Ease.InOutSine);
    }   
}
