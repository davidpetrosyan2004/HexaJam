using DG.Tweening;
using UnityEngine;

public class GameWinLose : MonoBehaviour
{
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject gameLosePanel;
    [SerializeField] private GameObject disableGamePanel;
    private void Start()
    {
        disableGamePanel.SetActive(false);

    }
    private void OnEnable()
    {
        GameEvents.OnGameCondition += PanelPopUp;
    }
    private void OnDisable()
    {
        GameEvents.OnGameCondition -= PanelPopUp;
    }

    public void PanelPopUp(bool gameCondition)
    {
        GameObject gamePanel;
        if (gameCondition)
        {
            gamePanel = gameWinPanel;
        }
        else
        {
            gamePanel = gameLosePanel;
        }
        gamePanel.transform.localScale = Vector3.zero;

        gamePanel.SetActive(true);
        disableGamePanel.SetActive(true);

        gamePanel.transform
            .DOScale(1f, 0.4f)
            .SetEase(Ease.OutBack);
        if (gameCondition)
        {
            AudioManager.Instance.PlaySound("GameWin");
        }
        else
        {
            AudioManager.Instance.PlaySound("GameLose");
        }
    }
}
