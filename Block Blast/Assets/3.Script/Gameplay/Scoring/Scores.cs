using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    public SquareTextureData squareTextureData;    
    public static bool _newBestScores;

    [Header("UI References")]
    public Text scoreText;
    public Text bestScoreText; //최고 점수 표시용(선택)

    [Header("Current Game")]    
    public static int _currentScores;
    
    [Header("Saved Data")]
    private PlayerData _playerData;

    private void Awake()
    {
        LoadPlayerData();
        UpdateBestScore();
    }

    private void Start()
    {
        //현재 게임 초기화
        _currentScores = 0;
        squareTextureData.SetStartColor();
        _newBestScores = false;

        UpdateScoreText();        
    }

    private void OnEnable()
    {
        GameEvents.AddScore += AddScores;        
        GameEvents.GameOver += SavePlayerData;
    }

    private void OnDisable()
    {
        GameEvents.AddScore -= AddScores;        
        GameEvents.GameOver -= SavePlayerData;
    }

    private void OnApplicationQuit()
    {
        //종료 시 저장
        SavePlayerData();
    }

    // 플레이어 데이터 로드    
    private void LoadPlayerData()
    {
        _playerData = SaveSystem.LoadPlayerData();
    }
   
    //플레이어 데이터 저장   
    private void SavePlayerData()
    {
        if(_playerData == null)
        {
            Debug.LogError("플레이어 점수가 null");
            return;
        }

        SaveSystem.SavePlayerData(_playerData);
        Debug.Log($"저장 완료 - 베스트 점수: { _playerData.bestScore}");
    }
    //점수 추가
    private void AddScores(int scores)
    {
        _currentScores += scores;

        //최고 점수 갱신 시
        if(_currentScores > _playerData.bestScore)
        {            
            _playerData.bestScore = _currentScores;
            _newBestScores = true;
        }

        UpdateSquareColor();

        GameEvents.UpdateBestScoreBar(_currentScores, _playerData.bestScore);//여기 나중에 수정
        UpdateScoreText();        
        UpdateBestScore();
    }

    private void UpdateSquareColor()
    {
        if(_currentScores >= squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColors(_currentScores);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = _currentScores.ToString();
    }

    private void UpdateBestScore()
    {
        if(bestScoreText == null)
        {
            Debug.Log("튜토리얼 버전");
            bestScoreText.text = "0";
            return;
        }

        bestScoreText.text = _playerData.bestScore.ToString();
    }
}
