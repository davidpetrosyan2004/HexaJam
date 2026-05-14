using DG.Tweening;
using TMPro;
using UnityEngine;

public class TurtlesCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;
    private int turtlesCount;
    private void OnEnable()
    {
        GameEvents.OnTurtlesCountTextSet += SetTurtlesCountText;
        GameEvents.OnTurtlesSubstract += TurtleSubstract;
    }
    private void OnDisable()
    {
        GameEvents.OnTurtlesCountTextSet -= SetTurtlesCountText;
        GameEvents.OnTurtlesSubstract -= TurtleSubstract;
    }
    public void SetTurtlesCountText(int count)
    {
        turtlesCount = count;

        Sequence seq = DOTween.Sequence();
        seq.Join(DOTween.To(() => turtlesCount, x =>
        {
            turtlesCount = x;
            countText.text = x.ToString();
        }, turtlesCount, 0.3f));
        seq.Join(countText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1));
    }

    public void TurtleSubstract()
    {
        turtlesCount--; 
        SetTurtlesCountText(turtlesCount);
    }
}
