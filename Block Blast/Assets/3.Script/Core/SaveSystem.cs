using System;
using System.IO;
using UnityEngine;

/*
 JSON 기반 저장/로드 시스템
 */

public class SaveSystem : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "playerdata.json");


    //플레이어 데이터 저장
    public static void SavePlayerData(PlayerData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] 저장 완료: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] 저장 실패: {e.Message}");
        }
    }

    //플레이어 데이터 로드
    public static PlayerData LoadPlayerData()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[SaveSystem] 저장 파일 없음, 새로 생성");
                return CreateNewPlayerData();
            }

            string json = File.ReadAllText(SavePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log($"[SaveSystem] 로드 완료 - 베스트 스코어: {data.bestScore}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] 로드 실패: {e.Message}");
            return CreateNewPlayerData();
        }
    }

    //새 플레이어 데이터 생성
    private static PlayerData CreateNewPlayerData()
    {
        PlayerData data = new PlayerData
        {
            bestScore = 0,
            totalGamesPlayed = 0,
            totalLinesCleared = 0,
            totalBlocksPlaced = 0,
            maxCombo = 0,
            lastPlayTime = GetCurrnetTimestamp()
        };

        SavePlayerData(data);
        return data;
    }

    //현재 시간
    private static long GetCurrnetTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    //저장 파일 삭제
    public static void DeleteSaveFile()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("저장 파일 삭제");
        }
    }

    //저장 파일 존재 여부
    public static bool SaveFileExists()
    {
        return File.Exists(SavePath);
    }

    // 타임스탬프를 한국 시간 문자열로 변환 (디버깅/UI용)
    public static string TimestampToKoreanDateString(long timestamp)
    {
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        TimeZoneInfo kstZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
        DateTimeOffset koreanTime = TimeZoneInfo.ConvertTime(dateTime, kstZone);
        return koreanTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

