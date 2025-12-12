using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//그리드의 각 칸을 관리
public class GridSquare : MonoBehaviour
{
    public Image hoverImage; // 이 칸에 마우스가 올라갔을 때 표시할 이미지 컴포넌트
    public Image activeImage; // 이 칸이 활성화되었을 때 표시할 이미지 컴포넌트
    public Image normalImage; // 이 칸의 기본 이미지 컴포넌트 - Unity UI 시스템을 사용해 2D 블록 게임 제작, Canvas 위에 Image로 칸을 표현
    public List<Sprite> normalImages; // 칸의 다양한 상태를 나타낼 스프라이트 모음
    
    public bool Selected { get; set; } // 칸이 선택되었는지 여부를 나타내는 속성
    public int SquareIndex { get; set; } // 칸의 인덱스를 나타내는 속성
    public bool SquareOccupied { get; set; } // 칸이 점유되었는지 여부를 나타내는 속성

    private void Awake()
    {
        Image img = GetComponent<Image>();
    }

    private void Start()
    {
        Selected = false;// 칸이 선택되지 않은 상태로 초기화
        SquareOccupied = false;// 칸이 점유되지 않은 상태로 초기화
    }

    public bool CanWeUseThisSquare() // 사용가능 여부 체크
    {
        return hoverImage.gameObject.activeSelf; // hover 이미지가 활성화되어 있으면 칸을 사용할 수 있음
    }
    public void ActivateSquare(Sprite shapeSprite) // ShapeSquare 의 Sprite를 받아서 배치
    {
        hoverImage.gameObject.SetActive(false); 
        //ShapeSquare의 Sprite를 activeImage에 복사
        if(shapeSprite != null && activeImage != null)
        {
            activeImage.sprite = shapeSprite;
        }
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;         
    }
    public void DeActivate()
    {
        activeImage.gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        Selected = false;
        SquareOccupied = false;
    }

    public Image GetVisibleImage()
    {
        // SquareOccupied == true → ActiveImage가 현재 보이는 이미지
        if (SquareOccupied)
            return activeImage;

        // 비어있음 → NormalImage가 보이는 상태
        return normalImage;
    }

    public void PlayClearEffect(float delay)
    {
        Image target = GetVisibleImage();
        if (target == null)
        {
            return;
        }

        target.DOKill();

        Color original = target.color;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.Append(target.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack));
        seq.Join(target.DOFade(0f, 0.2f));
        seq.Append(target.transform.DOScale(1f, 0.1f));
        seq.OnComplete(() =>
        {
            // 제거 로직
            DeActivate();
            ClearOccupied();

            // 색상 초기화
            target.color = original;
        });
    }
    public void SetImage(bool setFirstImage)
    {        
        if (setFirstImage)
        {
            normalImage.sprite = normalImages[1];
        }
        else
        {
            normalImage.sprite = normalImages[0];
        }
    }    
    private void HandleTrigger(Collider2D collision)
    {
        if (SquareOccupied == false) // 칸이 비어있으면
        {
            Selected = true;
            hoverImage.gameObject.SetActive(true);// hover 이미지 활성화
        }
        else if (collision.GetComponent<ShapeSquare>() != null) // 칸이 이미 차있으면
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTrigger(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleTrigger(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(SquareOccupied == false)
        {
            Selected = false;
            hoverImage.gameObject.SetActive(false); // 마우스가 칸에서 나갈 때 hover 이미지 비활성화
        }
        else if (collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnsetOccupied();
        }
    }
}
