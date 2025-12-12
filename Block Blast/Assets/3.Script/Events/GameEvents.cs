using UnityEngine;
using System;

/*
    중앙 이벤트 관리 시스템
 */
public class GameEvents : MonoBehaviour
{
    public static Action GameOver;

    public static Action<int> AddScore;

    public static Action CheckIfShapeCanBePlaced; // 도형을 놓을 수 있는지 체크

    public static Action MoveShapeToStartPosition; // 시작 위치로 복귀

    public static Action RequestNewShapes; // 새로운 도형 생성

    public static Action SetShapeInactive; // 사용한 도형 비활성화

    public static Action<int, int> UpdateBestScoreBar;

    public static Action ShowCongratulationWritings;
}
