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
    [SerializeField] private Collider turtleCollider;
    
    public bool isMoving { get; set; }
    private void Start()
    {
        transform.DOScale(
            1.1f,
            1f
        ).SetLoops(-1, LoopType.Yoyo);
        turtleAnimator = GetComponent<Animator>();
        turtleCollider = GetComponent<Collider>();
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
        transform.DOPunchScale(
            Vector3.one * 0.25f,
            0.25f,
            6,
            0.5f
        );
        bool completed = false;
        if (isMoving) yield break;
        
        isMoving = true;
        if (comeBack)
        {
            AudioManager.Instance.PlaySound("TurtleAhead");
            GameEvents.OnTurtleMovingWrong?.Invoke(true);
            transform.DOMove(targetPosition + Quaternion.Euler(0, 0, 0) * transform.GetChild(0).forward* 0.3f, .4f)
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
            AudioManager.Instance.PlaySound("TurtleAdd");
            turtleCollider.enabled = false;

            transform.DOMove(targetPosition, .5f)
                .SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    ScaleSizeJump();
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

    public void ScaleSizeJump()
    {
        transform.DOScale(
            new Vector3(1.2f, 1.2f, 1.2f),
            0.1f
        ).SetLoops(2, LoopType.Yoyo);
    }
}
