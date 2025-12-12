using System;
using UnityEngine;

/*
    플레이어 데이터 구조
 */
[Serializable]
public class PlayerData
{
    [Header("Tutorial")]
    public bool tutorialCompleted = false;

    [Header("Score Data")]
    public int bestScore = 0;
    public int lastScore = 0;

    [Header("Game Stats")]
    public int totalGamesPlayed = 0;
    public int totalLinesCleared = 0;
    public int totalBlocksPlaced = 0;
    public int maxCombo = 0;

    [Header("Timestamps (KST - Korean Standard Time)")]
    public long lastPlayTime = 0;    // 마지막 플레이 시간 (KST)
    public long firstPlayTime = 0;   // 첫 플레이 시간 (KST)

    [Header("Settings")]
    public bool soundEnabled = true;
    public bool musicEnabled = true;
    public bool vibrateEnabled = true;

    // 총 플레이 시간 (초)
    public int GetTotalPlayTimeSeconds()
    {
        if (firstPlayTime == 0)
        {
            return 0;
        }
        return (int)(lastPlayTime - firstPlayTime);
    }

    // 총 플레이 시간 (시:분:초 형식)
    public string GetTotalPlayTimeFormatted()
    {
        int totalSeconds = GetTotalPlayTimeSeconds();
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    // 평균 점수
    public float GetAverageScore()
    {
        if (totalGamesPlayed == 0)
        {
            return 0f;
        }
        return (float)bestScore / totalGamesPlayed; // 수정: bestScore만 나누면 안됨
    }

    //마지막 플레이 시간 (한국 시간 문자열)
    public string GetLastPlayTimeString()
    {
        if (lastPlayTime == 0)
        {
            return "플레이 기록 없음";
        }
        return SaveSystem.TimestampToKoreanDateString(lastPlayTime);
    }

    //첫 플레이 시간 (한국 시간 문자열)
    public string GetFirstPlayTimeString()
    {
        if (firstPlayTime == 0)
        {
            return "플레이 기록 없음";
        }
        return SaveSystem.TimestampToKoreanDateString(firstPlayTime);
    }
}

