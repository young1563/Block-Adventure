using UnityEngine;
using TMPro;
using DG.Tweening;

public class ShowTopMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText; // UI 연결
    public float duration = 3f; // 표시 시간

    public void ShowMessage(string msg)
    {
        if (messageText == null) return;

        messageText.text = msg;
        messageText.gameObject.SetActive(true);

        CanvasGroup cg = messageText.GetComponent<CanvasGroup>();
        if (cg == null) cg = messageText.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0;
        messageText.rectTransform.anchoredPosition = new Vector2(0, -150);

        Sequence seq = DOTween.Sequence();

        seq.Append(cg.DOFade(1f, 0.4f))
           .Join(messageText.rectTransform.DOAnchorPos(new Vector2(0, -100), 0.4f).SetEase(Ease.OutQuad))
           .AppendInterval(duration)
           .Append(cg.DOFade(0f, 0.4f))
           .OnComplete(() =>
           {
               messageText.gameObject.SetActive(false);
           });
    }
}
