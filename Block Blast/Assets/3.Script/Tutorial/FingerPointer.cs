using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class FingerPointer : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    public GameObject glowEffect;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();

        // 시작 시 투명
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0;
        }
    }

    /// <summary>
    /// 페이드 인
    /// </summary>
    public IEnumerator FadeIn(float duration = 0.5f)
    {
        if (_canvasGroup == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = 1;
    }

    /// <summary>
    /// 페이드 아웃
    /// </summary>
    public IEnumerator FadeOut(float duration = 0.5f)
    {
        if (_canvasGroup == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = 0;
    }

    /// <summary>
    /// 특정 위치로 이동
    /// </summary>
    public IEnumerator MoveTo(Vector3 targetPosition, float duration = 1f)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Ease Out 효과 (부드럽게 감속)
            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    /// <summary>
    /// 탭 애니메이션 (선택 시)
    /// </summary>
    public IEnumerator TapAnimation(float duration = 0.2f)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 smallScale = originalScale * 0.8f;

        // 작아지기
        float elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, smallScale, elapsed / (duration / 2));
            yield return null;
        }

        // 커지기
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(smallScale, originalScale, elapsed / (duration / 2));
            yield return null;
        }

        transform.localScale = originalScale;
    }

    /// <summary>
    /// 즉시 투명하게
    /// </summary>
    public void Hide()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0;
        }
    }

    /// <summary>
    /// 즉시 보이게
    /// </summary>
    public void Show()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1;
        }
    }
    
/*    //글로우 효과
    public IEnumerator PulseGlow()
    {
        if (glowEffect == null) yield break;

        Image glow = glowEffect.GetComponent<Image>();
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;
            float alpha = (Mathf.Sin(time * 3f) + 1f) / 2f; // 0~1 반복
            Color c = glow.color;
            c.a = alpha * 0.5f;
            glow.color = c;
            yield return null;
        }
    }*/
}
