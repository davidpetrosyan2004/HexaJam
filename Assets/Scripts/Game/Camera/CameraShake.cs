using DG.Tweening;
using UnityEngine;
public class CameraShake : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnCameraShake += Shake;
    }
    private void OnDisable()
    {
        GameEvents.OnCameraShake -= Shake;
    }
    public void Shake()
    {
        transform
            .DOShakePosition(
                duration: 0.8f,
                strength: 0.05f,
                vibrato: 0,
                randomness: 90,
                fadeOut: true
            )
            .SetEase(Ease.OutQuad);
    }
}
