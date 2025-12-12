using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class ScoreAnimator : MonoBehaviour
{
    public Text scoreText;
    public RectTransform bg;

    public float animScale = 2.0f;
    public float animDuration = 0.25f;

    //private int currentScore = 0;

    public void SetScore(int newScore)
    {
        scoreText.text = newScore.ToString();

        // 애니메이션
        PlayScoreAnim();
    }

    private void PlayScoreAnim()
    {
        // 기존 트윈 정리
        bg.DOKill();
        scoreText.rectTransform.DOKill();

        // 배경 팝 애니메이션
        bg.localScale = Vector3.one;
        bg.DOScale(animScale, animDuration)
            .SetEase(Ease.OutBack)
            .SetLoops(2, LoopType.Yoyo);

        // 텍스트 팝
        scoreText.rectTransform.localScale = Vector3.one;
        scoreText.rectTransform.DOScale(animScale, animDuration)
            .SetEase(Ease.OutBack)
            .SetLoops(2, LoopType.Yoyo);
    }
}
