using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButtonBinder : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Button button;
    [SerializeField] private ButtonType buttonType;
    enum ButtonType
    {
        LoadSceneButton,
        CurrentLevelButton,
    }
    private void Start()
    {
        if (buttonType == ButtonType.CurrentLevelButton)
        {
            int levelIndex = PlayerPrefs.GetInt("CurrentLevel", 1);
            Debug.Log($"CurrentLevel: {levelIndex}");
            string levelSceneName = $"Level {levelIndex}";
            button.onClick.AddListener(() => {
                transform.DOPunchScale(
                    Vector3.one * 0.1f,
                    0.3f,
                    8,
                    0.5f
                ).OnComplete(()=> SceneManager.LoadScene(levelSceneName));

                PlaySound();
            });
        }
        else
        {
            button.onClick.AddListener(() => {
                transform.DOPunchScale(
                        Vector3.one * 0.1f,
                        0.3f,
                        8,
                        0.5f
                    ).OnComplete(() => SceneManager.LoadScene(sceneName));
                PlaySound();
                });
        }
    }


    public void PlaySound()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
    }
}
