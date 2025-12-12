using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SimpleTutorial : MonoBehaviour
{
    [Header("Tutorial Elements")]
    public GameObject fingerPointer;        // 손가락 이미지
    public Transform shapeStartPoint;       // Shape 위치
    public Transform gridTargetPoint;       // 그리드 목표 위치

    [Header("Animation Settings")]
    public float fadeInDuration = 0.5f;     // 페이드 인 시간
    public float moveDuration = 1.5f;       // 이동 시간
    public float fadeOutDuration = 0.5f;    // 페이드 아웃 시간
    public float delayBetweenLoops = 1f;    // 반복 딜레이

    [Header("UI")]
    public Text instructionText;
    public Button skipButton;
    public GameObject overlay;              // 반투명 배경
    public GameObject goodPopup;

    private FingerPointer _fingerPointer;
    private bool _tutorialCompleted = false;
    private Tween goodTween;

    private void Awake()
    {
        // 손가락 애니메이터 가져오기
        if (fingerPointer != null)
        {
            _fingerPointer = fingerPointer.GetComponent<FingerPointer>();
            if (_fingerPointer == null)
            {
                _fingerPointer = fingerPointer.AddComponent<FingerPointer>();
            }
        }
    }

    private void Start()
    {        
        // 튜토리얼 이미 완료했는지 확인
        if (_tutorialCompleted)
        {
            // 바로 게임 씬으로
            DOTween.KillAll();
            SceneManager.LoadScene(2);
            return;
        }

        // 건너뛰기 버튼 이벤트
        if (skipButton != null)
        {            
            skipButton.onClick.AddListener(SkipTutorial);
        }

        // 튜토리얼 시작
        StartCoroutine(TutorialLoop());
    }

    /// <summary>
    /// 튜토리얼 애니메이션 반복
    /// </summary>
    private IEnumerator TutorialLoop()
    {
        // 초기 설명
        if (instructionText != null)
        {
            instructionText.text = "도형을 그리드로 드래그하세요!";
        }

        // 손가락 초기 위치
        if (fingerPointer != null && shapeStartPoint != null)
        {
            fingerPointer.transform.position = shapeStartPoint.position;
        }

        // 반복 재생
        while (!_tutorialCompleted)
        {
            yield return StartCoroutine(PlayTutorialAnimation());
            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }

    /// <summary>
    /// 튜토리얼 애니메이션 1회 재생
    /// </summary>
    private IEnumerator PlayTutorialAnimation()
    {
        if (_fingerPointer == null || shapeStartPoint == null || gridTargetPoint == null)
        {
            Debug.LogError("튜토리얼 요소가 설정되지 않았습니다!");
            yield break;
        }

        // 1. 손가락 페이드 인 (Shape 위치에서)
        fingerPointer.transform.position = shapeStartPoint.position;
        yield return StartCoroutine(_fingerPointer.FadeIn(fadeInDuration));

        // 2. 잠시 대기 (사용자가 보게)
        yield return new WaitForSeconds(0.3f);

        // 3. 그리드로 이동
        yield return StartCoroutine(_fingerPointer.MoveTo(gridTargetPoint.position, moveDuration));

        // 4. 잠시 대기
        yield return new WaitForSeconds(0.3f);

        // 5. 손가락 페이드 아웃
        yield return StartCoroutine(_fingerPointer.FadeOut(fadeOutDuration));
    }

    public void SkipTutorial()
    {
        StopAllCoroutines();
        _tutorialCompleted = true;
        CompleteTutorial();
    }

    //튜토리얼 완료 처리
    private void CompleteTutorial()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(2);
    }

    //사용자가 직접 배치하면 튜토리얼 완료
    public void OnUserPlacedShape()
    {
        if (_tutorialCompleted)
        {
            return;
        }

        _tutorialCompleted = true;

        //손가락 멈추고 숨김
        StopFingerAnimation();

        //텍스트 표시
        if(instructionText != null)
        {
            instructionText.text = "Good!";
        }

        PlayGoodTween();

        StartCoroutine(ShowSuccessAndFinish());        
    }

    private IEnumerator ShowSuccessAndFinish()
    {
        yield return new WaitForSeconds(1f); //1초 표시
        CompleteTutorial(); //튜토리얼 종료
    }

    private void StopFingerAnimation()
    {
        StopAllCoroutines(); // 튜토리얼 애니메이션 루프 정지
        if(fingerPointer != null)
        {
            fingerPointer.SetActive(false); //손가락 숨김
        }
    }

    private void PlayGoodTween()
    {
        if (goodPopup == null) return;

        CanvasGroup cg = goodPopup.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = goodPopup.AddComponent<CanvasGroup>();

        goodPopup.SetActive(true);

        // 초기 상태
        cg.alpha = 0f;
        goodPopup.transform.localScale = Vector3.one * 0.5f;
        goodPopup.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // 기존 트윈 정리
        goodTween?.Kill();

        // 새 시퀀스 시작
        Sequence seq = DOTween.Sequence();

        // 등장 (팝 + rotate)
        seq.Append(cg.DOFade(1f, 0.2f))
           .Join(goodPopup.transform.DOScale(0.45f, 0.2f).SetEase(Ease.OutBack))
           .Join(goodPopup.transform.DORotate(new Vector3(0, 0, -8), 0.15f).SetEase(Ease.OutQuad))
           .Append(goodPopup.transform.DORotate(new Vector3(0, 0, +8), 0.15f).SetEase(Ease.OutQuad))
           .Append(goodPopup.transform.DORotate(Vector3.zero, 0.15f).SetEase(Ease.OutQuad));

        // Ping-Pong (미세 흔들림 + 살짝 Scale 업다운)
        seq.Append(
            DOTween.Sequence()
                .Join(goodPopup.transform.DOScale(0.35f, 0.3f))
                .Join(goodPopup.transform.DORotate(new Vector3(0, 0, 3), 0.3f))
                .SetLoops(2, LoopType.Yoyo)
        );

        // FadeOut
        seq.Append(cg.DOFade(0f, 0.25f));

        // 종료 시 비활성화
        seq.OnComplete(() =>
        {
            goodPopup.SetActive(false);
        });

        goodTween = seq;
    }
}