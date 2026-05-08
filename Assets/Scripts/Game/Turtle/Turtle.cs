using DG.Tweening;
using System.Collections;
using UnityEngine;
using static GridData;
public class Turtle : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;
    private bool isMoving;
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
    public IEnumerator MoveTo(Vector3 targetPosition, bool comeBack = false)
    {
        AudioManager.Instance.PlaySound("TurtleAdd");
        if (isMoving) yield break;

        isMoving = true;

        if (comeBack)
        {
            GameEvents.OnTurtleMovingWrong?.Invoke(true);

            bool completed = false;

            transform.DOMove(targetPosition, 0.25f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    GameEvents.OnTurtleMovingWrong?.Invoke(false);
                    isMoving = false;
                    completed = true;
                });

            yield return new WaitUntil(() => completed);
        }
        else
        {
            bool completed = false;

            transform.DOMove(targetPosition, 0.25f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    GridCell cell = GetComponentInParent<GridCell>();

                    if (cell != null)
                    {
                        cell.Turtle = null;
                    }

                    GameEvents.OnTurtleAddedInventory?.Invoke(this);

                    Debug.Log("Turtle moved to inventory");
                    GameEvents.OnTurtlesSubstract?.Invoke();
                    completed = true;
                    isMoving = false;
                });

            yield return new WaitUntil(() => completed);
        }
    }
}
