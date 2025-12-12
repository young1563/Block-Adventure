using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//도형의 한 칸 > 여러 개가 모여서 Shape로 구성됨
public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage; // 상태 표시용 이미지
    private BoxCollider2D _collider;
    private Image _mainImage;

    private void Awake()
    {        
        gameObject.GetComponent<BoxCollider2D>(); //클릭 감지 활성화            
        gameObject.SetActive(true); //사각형 표시        
        _mainImage = GetComponent<Image>(); //

        if (occupiedImage != null)
        {
            occupiedImage.gameObject.SetActive(false);
        }
    }
        
    public void ActivateShape() //사각형 활성화 (사용가능상태)
    {
        _collider.enabled = true;
        gameObject.SetActive(true);        
    }

    public void DeactivateShape()
    {
        _collider.enabled = false;
        gameObject.SetActive(false);
    }

    public void SetOccupied() //사각형을 '점유됨' 상태로 표시
    {
        if(occupiedImage != null)
        {
            occupiedImage.gameObject.SetActive(true);
        }        
    }

    public void UnsetOccupied()
    {
        if(occupiedImage != null)
        {
            occupiedImage.gameObject.SetActive(false);
        }        
    }

    //sprite 설정
    public void SetSprite(Sprite sprite)
    {
        if(_mainImage != null && sprite != null)
        {
            _mainImage.sprite = sprite;
        }
    }

    //현재 Sprite 반환
    public Sprite GetSprite()
    {
        if(_mainImage != null)
        {
            return _mainImage.sprite;
        }
        return null;
    }
}
