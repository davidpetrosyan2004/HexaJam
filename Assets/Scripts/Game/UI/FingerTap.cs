using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class FingerTap : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerInputHandler playerInput;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");

        transform.DOScale(Vector3.zero, 0.25f)
            .OnComplete(() => { Destroy(gameObject); });
        playerInput.isTutorial = false;
    }
}
