using DG.Tweening;
using UnityEngine;

public class TitleBounce : MonoBehaviour
{
    [Header("Scale Bounce")]
    public float minScale = 1.1f;
    public float maxScale = 1.25f;
    public float minScaleDuration = 1.2f;
    public float maxScaleDuration = 3.0f;

    [Header("Rotation Wobble")]
    public float minRotate = -14f;
    public float maxRotate = 14f;
    public float minRotateDuration = 1.2f;
    public float maxRotateDuration = 3.5f;

    private void Start()
    {
        float delay = Random.Range(0f, 0.9f); //시작 시간 랜덤

        // 원래 상태 기억
        Vector3 originalScale = transform.localScale;

        // 랜덤 스케일 목표값
        float randomScale = Random.Range(minScale, maxScale);
        float scaleDuration = Random.Range(minScaleDuration, maxScaleDuration);

        // 랜덤 회전 목표값
        float randomRotate = Random.Range(minRotate, maxRotate);
        float rotateDuration = Random.Range(minRotateDuration, maxRotateDuration);

        // Scale Ping-Pong
        transform.DOScale(originalScale * randomScale, scaleDuration)
                 .SetEase(Ease.InOutSine)
                 .SetDelay(delay)
                 .SetLoops(-1, LoopType.Yoyo);

        // Rotate Ping-Pong
        transform.DORotate(new Vector3(0, 0, randomRotate), rotateDuration)
                 .SetEase(Ease.InOutSine)
                 .SetDelay(delay)
                 .SetLoops(-1, LoopType.Yoyo);
    }
}
