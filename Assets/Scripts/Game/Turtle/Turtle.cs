using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using static GridData;
using static TurtlesMove;
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
    public IEnumerator MoveTo(Vector3 targetPosition, MoveType moveType = MoveType.Jump)
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
        if (moveType == MoveType.ComeBack)
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
        else if (moveType == MoveType.Dive)
        {
            AudioManager.Instance.PlaySound("TurtleAdd");
            turtleCollider.enabled = false;
            Sequence seq = DOTween.Sequence();

            // 1. Движение к точке
            seq.Append(transform.DOMove(targetPosition, 0.6f)
                .SetEase(Ease.OutQuad));

            // 2. Нырок вниз после прибытия
            seq.Append(transform.DOMoveY(targetPosition.y - 1.5f, 0.3f)
                .SetEase(Ease.InCubic));

            // 3. Наклон как нырок
            seq.Join(transform.DORotate(new Vector3(0, 60f, 0), 0.3f, RotateMode.LocalAxisAdd));

            // 4. Исчезновение (если нужно)
            seq.Append(transform.DOScale(0f, 0.2f));
            seq.OnComplete(() =>
            {
                ScaleSizeJump();
                Debug.Log("Turtle moved to inventory");
                GameEvents.OnTurtlesSubstract?.Invoke();
                completed = true;
                isMoving = false;
                turtleAnimator.SetBool("isMoving", false);
                transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
                GameEvents.OnTurtleAddedInventory?.Invoke(this);
            });
            GridCell cell = GetComponentInParent<GridCell>();

            if (cell != null)
            {
                cell.Turtle = null;
            }
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
            initEffectPos - transform.GetChild(0).forward,
            Quaternion.identity
        );

        GameObject ripple = Instantiate(
            rippleEffect,
            initEffectPos -transform.GetChild(0).forward,
            Quaternion.identity
        );
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);


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
