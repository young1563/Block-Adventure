using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shape : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public GameObject squareShapeImage; //
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 700f); //드래그 오프셋 - 마우스 포인터와 오브젝트 중심점 사이의 거리

    public SquareTextureData squareTextureData;

    [HideInInspector]
    public ShapeData CurrentShapeData; //

    public int TotalSquareNumber { get; set; }

    private List<GameObject> _currentShape = new List<GameObject>();
    private List<ShapeSquare> currentShape = new List<ShapeSquare>();
    private Vector3 _shapeStartScale; //초기 스케일
    private RectTransform _transform; //RectTransform 컴포넌트    
    private Canvas _canvas; //캔버스 컴포넌트
    private Vector3 _startPosition;
    private bool _shapeActive = true;
    

    private void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;//초기 스케일 저장
        _transform = this.GetComponent<RectTransform>();//RectTransform 컴포넌트 저장
        _canvas = GetComponentInParent<Canvas>(); //캔버스 컴포넌트 저장 - 부모 오브젝트에서 찾음        
        _startPosition = _transform.localPosition;
        _shapeActive = true;
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }
    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }
    
    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach(GameObject square in _currentShape)
        {
            if (square.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
        
    public void ActivateShape()
    {
        if (!_shapeActive) //비활성화일 경우 - 중복 활성화 방지
        {
            foreach (ShapeSquare square in currentShape)
            {
                if (square != null)
                {
                    square.ActivateShape();
                }                
            }
        }
        _shapeActive = true; // 도형 전체를 활성 상태로 표시
    }

    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (ShapeSquare square in currentShape)
            {
                if(square != null)
                {
                    square.DeactivateShape();
                }
            }
        }
        _shapeActive = false;
    }

    private void SetShapeInactive()
    {
        if (IsOnStartPosition() == false && IsAnyOfShapeSquareActive()) //시작위치에 없고, 활성상태 -> 배치가 완료된 도형 -> 모든 사각형 숨김 처리
        {
            foreach(GameObject square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapedata)
    {
        CurrentShapeData = shapedata;
        TotalSquareNumber = GetNumberOfSquare(shapedata);

        while (_currentShape.Count <= TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        //랜덤 Sprite 선택( 도형 전체에 적용)
        Sprite randomSprite = null;
        if(squareTextureData != null)
        {
            randomSprite = squareTextureData.GetRandomSprite();
        }

        foreach (GameObject square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);

            //Sprite 적용
            if(randomSprite != null)
            {
                ShapeSquare shapeSquare = square.GetComponent<ShapeSquare>();
                if(shapeSquare != null)
                {
                    shapeSquare.SetSprite(randomSprite);
                }
            }
        }

        RectTransform squareRect = squareShapeImage.GetComponent<RectTransform>();
        Vector2 moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        //set position to form final shape
        for (int row = 0; row < shapedata.rows; row++)
        {
            for (int column = 0; column < shapedata.columns; column++)
            {
                if (shapedata.board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetPositionForShape(column, shapedata.columns, moveDistance.x), -GetPositionForShape(row, shapedata.rows, moveDistance.y));

                    currentIndexInList++;
                }
            }
        }
    }
    
    private float GetPositionForShape(int index, int maxSize, float distance)
    {
        if(maxSize <= 1)
        {
            return 0f;
        }

        if (maxSize % 2 != 0) //홀수
        {
            int center = (maxSize - 1) / 2;
            return (index - center) * distance;
        }
        else //짝수
        {
            int Middle1 = (maxSize / 2) - 1;
            int Middle2 = (maxSize / 2);
            if (index <= Middle1)
            {
                return (index - Middle1 - 0.5f) * distance;
            }
            else
            {
                return (index - Middle2 + 0.5f) * distance;
            }
        }
    }

    private int GetNumberOfSquare(ShapeData shapedata)
    {
        int number = 0;

        foreach (ShapeData.Row rowData in shapedata.board)
        {
            foreach (bool active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }

        return number;
    }

    public void OnDrag(PointerEventData eventData)
    {        
        _transform.anchorMin = new Vector2(0, 0); //드래그 중에도 앵커 고정
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0); //드래그 중에도 피벗 고정

        Vector2 pos; //드래그 위치 저장 변수
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, 
            eventData.position, _canvas.worldCamera, out pos);//스크린 좌표를 로컬 좌표로 변환
        _transform.localPosition = pos + offset; //오브젝트 위치 업데이트 -이유: 마우스 포인터와 오브젝트 중심점 사이의 거리 보정
    }
    public void OnEndDrag(PointerEventData eventData)
    {        
        this.GetComponent<RectTransform>().localScale = _shapeStartScale; //드래그 끝난 후 스케일 원래대로
        GameEvents.CheckIfShapeCanBePlaced(); //도형을 놓을 수 있는지 확인하는 이벤트 호출
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale; //드래그 시작 시 스케일 변경     
    }

    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }

}
