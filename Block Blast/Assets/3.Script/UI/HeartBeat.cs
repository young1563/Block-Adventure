using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HeartBeat : MonoBehaviour
{
    public Image heartImage;      // 하트 스프라이트
    public float beatScale = 1.15f;
    public float beatDuration = 0.6f;

    private void Start()
    {
        PlayHeartbeat();
    }

    private void PlayHeartbeat()
    {
        if (heartImage == null) return;

        // 초기 상태
        transform.localScale = Vector3.one;

        // 무한 반복 두근두근
        transform.DOScale(beatScale, beatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // 밝기 번쩍 효과 (Color)
        Color original = heartImage.color;
        Color glow = new Color(original.r + 0.15f, original.g + 0.15f, original.b + 0.15f, original.a);

        heartImage.DOColor(glow, beatDuration * 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
