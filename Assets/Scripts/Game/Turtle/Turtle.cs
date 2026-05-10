using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using static GridData;
public class Turtle : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private Animator turtleAnimator;
    [SerializeField] private ParticleSystem waterSplashEffect;
    [SerializeField] private GameObject rippleEffect;
    [SerializeField] private Transform rootBonePosition;
    
    private bool isMoving;
    private void Start()
    {
        turtleAnimator = GetComponent<Animator>();
    }
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
        turtleAnimator.SetBool("isMoving", true);

        bool completed = false;
        AudioManager.Instance.PlaySound("TurtleAdd");
        if (isMoving) yield break;

        isMoving = true;

        if (comeBack)
        {
            GameEvents.OnTurtleMovingWrong?.Invoke(true);
            transform.DOKill();
            transform.DOMove(targetPosition, .5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    GameEvents.OnTurtleMovingWrong?.Invoke(false);
                    isMoving = false;
                    completed = true;
                    turtleAnimator.SetBool("isMoving", false);

                });

            yield return new WaitUntil(() => completed);
        }
        else
        {
            transform.DOKill();

            transform.DOMove(targetPosition, .5f)
                .SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    turtleAnimator.SetTrigger("Jump");
                    Debug.Log("Turtle moved to inventory");
                    GameEvents.OnTurtlesSubstract?.Invoke();
                    completed = true;
                    isMoving = false;
                    turtleAnimator.SetBool("isMoving", false);
                });
            GridCell cell = GetComponentInParent<GridCell>();

            if (cell != null)
            {
                cell.Turtle = null;
            }
            yield return new WaitUntil(() => completed);
        }

    }

    public void JumpWaterEffect()
    {
        Debug.Log("Effect");

        transform.localScale = Vector3.zero;
        var initEffectPos=  transform.position;
        transform.position = rootBonePosition.position;
        AudioManager.Instance.PlaySound("WaterSplash");

        Instantiate(
            waterSplashEffect,
            initEffectPos + Quaternion.Euler(0, 120, 0) * transform.right,
            Quaternion.identity
        );

        GameObject ripple = Instantiate(
            rippleEffect,
            initEffectPos + Quaternion.Euler(0, 120, 0) * transform.right,
            Quaternion.identity
        );
        transform.localRotation = Quaternion.Euler(-90, 70, 90);


        GameEvents.OnTurtleAddedInventory?.Invoke(this);
    }
}
