using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0; //그리드 가로 칸 개수
    public int rows = 0;    //그리드 세로 칸 개수 
    public float squaresGap = 0.1f; //사이 간격- 블록들이 명확히 구분되어 보이도록
    public GameObject gridSquare; 
    public Vector2 startPosition = new Vector2(0.0f, 0.0f);
    public float squareScale = 0.5f; //크기
    
    private Vector2 _offset = new Vector2(0.0f, 0.0f); // 위치 계산용 임시 변수
    private List<GameObject> _gridSquares = new List<GameObject>();

    private LineIndicator _lineIndicator;
    private SimpleTutorial _tutorial;
    
    // 생성된 모든 칸을 저장
    // 이유 1: 나중에 특정 칸에 접근해야 함 (블록 배치, 라인 체크 등)
    // 이유 2: 칸의 상태를 확인/변경해야 함 (색상 변경, 채워짐 표시)
    // 이유 3: 게임 종료 시 모든 칸을 초기화하거나 삭제해야 함

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        _tutorial = FindAnyObjectByType<SimpleTutorial>();
        CreateGrid(); //게임판 생성        
    }

    private void CreateGrid()
    {
        // 분리한 이유:
        // - 생성과 배치를 나누면 코드 이해가 쉬움
        // - 나중에 그리드 재배치가 필요할 때 SetGridSquaresPosition만 호출 가능
        // - 애니메이션 효과를 넣을 때 유용 (생성 후 → 하나씩 등장)
        SpawnGridSquares();// 1단계: 칸들 생성
        SetGridSquaresPosition();// 2단계: 칸들을 정확한 위치에 배치

        PlayGridIntroAnimation();        
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;// 이벤트 구독 해제
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;// 이벤트 구독
    }

    private void PlayGridIntroAnimation()
    {
        float maxDelay = 0.25f;     // 슬롯마다 랜덤 지연
        float duration = 0.22f;     // pop 속도

        foreach (GameObject square in _gridSquares)
        {
            RectTransform rt = square.GetComponent<RectTransform>();

            // 초기 스케일 0 설정
            rt.localScale = Vector3.zero;

            float delay = Random.Range(0f, maxDelay);

            rt.DOScale(squareScale, duration)
              .SetDelay(delay)
              .SetEase(Ease.OutBack);
        }
    }

    private void SpawnGridSquares()
    {
        // 그리드 칸 생성: 좌상단(0,0)에서 시작해 우하단까지
        // 예시 3x3:
        // [0] [1] [2]
        // [3] [4] [5]
        // [6] [7] [8]

        int square_index = 0;// 각 칸의 고유 번호 (0부터 시작)

        for (int i = 0; i< rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);// 칸 생성 및 리스트 추가

                /* 계층 구조 정리: Grid 오브젝트 아래에 모든 칸 배치
                 이유: 
                 - Hierarchy 창이 깔끔해짐
                 - Grid를 움직이면 모든 칸도 함께 움직임
                 - Grid를 비활성화하면 모든 칸도 함께 꺼짐*/
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index; 
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);                
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);//칸 크기 설정

                /* 체스판 패턴 적용 (짝수/홀수 칸 다른 색) => 3x3 패턴으로 변경
                 이유:
                 - 시각적으로 칸 구분이 명확해짐
                 - 게임이 더 보기 좋고 전문적으로 보임
                 - 플레이어가 블록 위치를 파악하기 쉬움*/
                //_gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(squre_index % 2 == 0);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 ==0);
                square_index++;
            }
        }
    }    
    private void SetGridSquaresPosition()
    {
        //위치 계산을 위한 변수 초기화
        int column_number = 0;
        int row_number = 0;

        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        // 3칸마다 추가되는 간격의 개수를 카운트
        // 이유: 9x9 그리드를 3x3 블록으로 구분하기 위함 (스도쿠 스타일)
        // 예: [0,1,2] | [3,4,5] | [6,7,8]
        //      간격 0개   간격 1개   간격 2개

        bool row_moves = false; //행이 바뀌었는지 추정하는 플래그

        //각 칸의 실제 크기 계산        
        RectTransform square_rect = _gridSquares[0].GetComponent<RectTransform>();//첫번째 칸의 RectTransform을 가져옴. 모든 칸이 같은 크기이므로 하나만 참조하면 됨

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x ;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y ;

        /*_offset 계산 상세 설명:
        -------------------------------------------
        square_rect.rect.width: 칸의 원본 너비 (예: 100픽셀)
        square_rect.transform.localScale.x: 스케일 (예: 0.5)
        실제 크기 = 100 * 0.5 = 50픽셀
        
        결과: _offset = 한 칸이 차지하는 전체 공간
        이걸 곱하면 다음 칸의 위치를 알 수 있음*/

        foreach (GameObject square in _gridSquares)
        {
            //1. 행 넘김 체크
            if(column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                // x축 추가 간격 카운터를 0으로 리셋
                // 이유: 새 줄이 시작되면 다시 왼쪽부터 세야 함
                column_number = 0;//첫번째 열로 돌아감                
                row_number++; //다음 행으로 이동
                row_moves = false;
            }

            //2. x축 위치 계산 --????? 이해안됨 쵸ㅣ비상
            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);

            if(column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }

            if (row_number > 0 && row_number % 3 == 0 && row_moves == false)
            {
                row_moves = true;
                pos_y_offset += squaresGap;
            }

            RectTransform rectTransform = square.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset);
            //rectTransform.localPosition = new Vector3(startPosition.x + pos_x_offset, startPosition.y - pos_y_offset, 0.0f);

            column_number++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        // 1) 선택된 사각형 인덱스 가져오기
        List<int> selectedIndexes = GetSelectedSquareIndexes();

        Shape currentShape = shapeStorage.GetCurrentSelectedShape();
        if (currentShape == null)
        {
            return;
        }           
        // 2) 배치 가능한지 확인
        if (CanShapeFitOnGrid(currentShape, selectedIndexes.Count))
        {
            // 3) 실제 배치 처리
            PlaceShapeOnGrid(currentShape, selectedIndexes);
            
            // 4) 점수 처리
            GameEvents.AddScore(currentShape.TotalSquareNumber);

            // 5) 튜토리얼 반영
            if (_tutorial != null)
                _tutorial.OnUserPlacedShape();

            // 6) 남은 shape 체크
            int activeShapes = 0;
            foreach (Shape shape in shapeStorage.shapesList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                    activeShapes++;
            }

            if (activeShapes == 0)
                GameEvents.RequestNewShapes();
            else
                GameEvents.SetShapeInactive();

            // 7) 줄 체크
            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
    }

    private List<int> GetSelectedSquareIndexes()
    {
        List<int> selectedIndexes = new List<int>();

        foreach(GameObject square in _gridSquares)
        {
            GridSquare gridSquare = square.GetComponent<GridSquare>();

            if(gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                selectedIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        return selectedIndexes;
    }
    private bool CanShapeFitOnGrid(Shape shape, int selectedCount)
    {
        if(shape == null)
        {
            return false;
        }
        return shape.TotalSquareNumber == selectedCount;
    }
    private void PlaceShapeOnGrid(Shape shape, List<int> squareIndexes)
    {
        List<ShapeSquare> activeShapeSquares = new List<ShapeSquare>();

        foreach (Transform child in shape.transform)
        {
            ShapeSquare shapeSquare = child.GetComponent<ShapeSquare>();
            if(shapeSquare != null && shapeSquare.gameObject.activeSelf)
            {
                activeShapeSquares.Add(shapeSquare);
            }
        }

        squareIndexes.Sort(); //선택된 그리드 인덱스를 오름차순으로 정렬

        //안전 장치
        if(activeShapeSquares.Count != squareIndexes.Count)
        {
            Debug.LogError("도령과 그리드 사이즈가 안 맞는다.");
            return;
        }

        // 정렬된 순서대로 배치
        for(int i = 0; i < squareIndexes.Count; i++)
        {
            Sprite shapeSprite = activeShapeSquares[i].GetSprite();
            _gridSquares[squareIndexes[i]].GetComponent<GridSquare>().ActivateSquare(shapeSprite);          

            /*Sprite shapeSprite = (i < activeShapeSquares.Count) ? activeShapeSquares[i].GetSprite() : null;
            _gridSquares[squareIndexes[i]].GetComponent<GridSquare>().ActivateSquare(shapeSprite);*/
        }
    }
    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> allLines = BuildAllLines();
        int completed = CountCompletedLines(allLines);

        //콤보 보너스 점수 적용
        int baseScore = completed * 10;
        int bonusScore = 0;

        int[] bonusTable = { 0, 0, 5, 15, 30, 50 };
        if (completed < bonusTable.Length)
            bonusScore = bonusTable[completed];
        else
            bonusScore = 50; // 6줄 이상일 때도 안전하게 처리

        int totalScore = baseScore + bonusScore;

        if (completed >= 2)
        {
            //GameEvents.ShowCongratulationWritings?.Invoke(); // TODO            
            transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0), 0.15f, 2, 0.3f); //지속시간, 진동 횟수, 탄성
        }

        GameEvents.AddScore(totalScore);

        CheckIfPlayerLost();
    }

    private List<int[]> BuildAllLines()
    {
        List<int[]> lines = new List<int[]>();
        
        for (int col = 0; col < 9; col++)//세로줄(0~8)
        {
            List<int> vertical = new List<int>(9);

            for (int row = 0; row < 9; row++)
            {
                vertical.Add(_lineIndicator.line_data[row, col]);
            }

            lines.Add(vertical.ToArray());
        }
        
        for (int row = 0; row < 9; row++)//가로줄(0~8)
        {
            List<int> horizontal = new List<int>(9);

            for(int col = 0; col<9; col++)
            {
                horizontal.Add(_lineIndicator.line_data[row, col]);
            }

            lines.Add(horizontal.ToArray());
        }

        return lines;
    }

    private int CountCompletedLines(List<int[]> lines)
    {
        int completedCount = 0;
        List<int[]> completedLines = new List<int[]>();

        //1. 완성된 줄 찾기
        foreach(int[] line in lines)
        {
            bool completed = true;

            foreach(int squareIndex in line)
            {
                if (!_gridSquares[squareIndex].GetComponent<GridSquare>().SquareOccupied)
                {
                    completed = false;
                }
            }
            if (completed)
            {
                completedLines.Add(line);
            }
        }
        //2. 애니메이션 적용
        float delayStep = 0.05f;

        //완성된 줄 제거
        foreach (int[] line in completedLines)
        {
            int order = 0;

            foreach(int sq in line)
            {
                GridSquare g = _gridSquares[sq].GetComponent<GridSquare>();

                float delay = order * delayStep;
                g.PlayClearEffect(delay);

                order++;
            }

            completedCount++;
        }

        return completedCount;
    }        

    private void CheckIfPlayerLost() //게임오버 체크
    {
        int validShapes = 0;

        //1. 모든 도형 체크
        for(int i = 0; i <shapeStorage.shapesList.Count; i++) 
        {
            bool isShapeActive = shapeStorage.shapesList[i].IsAnyOfShapeSquareActive();

            //2. 이 도형을 그리드에 배치 가능한지?
            if (CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapesList[i]) && isShapeActive)
            {
                shapeStorage.shapesList[i]?.ActivateShape();
                validShapes++;
            }            
        }
        //3. 배치 가능한 도형이 하나도 없으면 게임 오버
        if (validShapes == 0)
        {            
            GameEvents.GameOver();
            Debug.Log("게임오버");
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape) // 아직 배치할 수 있는 공간 있는지 체크
    {
        ShapeData currentShapeData = currentShape.CurrentShapeData;
        int shapeColumns = currentShapeData.columns;        
        int shapeRows = currentShapeData.rows;
                
        List<int> originalShapeFilledUpSquares = new List<int>();
        int squareIndex = 0;
        /*
         originalShapeFilledUpSquares = [0, 2, 4, 5]

        **시각화:**        
        인덱스 맵:
        [0][1]
        [2][3]
        [4][5]

        채워진 칸:
        [■][ ]  ← 0
        [■][ ]  ← 2
        [■][■]  ← 4, 5
         -> [0, 2, 4, 5]
         
         */
        for (int rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for(int colIndex = 0; colIndex < shapeColumns; colIndex++)
            {
                if (currentShapeData.board[rowIndex].column[colIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }
                
        if(currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count) //검증 코드
        {
            Debug.LogError("Number of filled up squares are not the same as the original shape have"); //버그 경고
        }

        List<int[]> squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;

        foreach(int[] number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            
            foreach(int squareIndexToCheck in originalShapeFilledUpSquares)
            {
                GridSquare comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if (shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }
        return canBePlaced;
    }

    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        List<int[]> squareList = new List<int[]>();
        int lastColumnIndex = 0;
        int lastRowIndex = 0;

        int safeIndex = 0;

        while (lastRowIndex + (rows - 1) < 9)
        {
            List<int> rowData = new List<int>();

            for(int row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for(int col = lastColumnIndex; col < lastColumnIndex + columns; col++)
                {
                    rowData.Add(_lineIndicator.line_data[row, col]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if(lastColumnIndex + (columns -1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }
            
            safeIndex++;
            if(safeIndex > 100)
            {
                break;
            }
        }
        return squareList;
    }
}
