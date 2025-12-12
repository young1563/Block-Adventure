using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class SquareTextureData : ScriptableObject
{
    [Serializable]
    public class TextureData
    {
        public Sprite texture;  // 블록 이미지
        public Config.SquareColor squareColor; // 색상 
    }

    [Header("Available Textures")]
    public List<TextureData> activeSquareTextures;//  사용 가능한 모든 색상 리스트
    private List<int> _recentIndices = new List<int>();

    //랜덤 Sprite 반환
    public Sprite GetRandomSprite()
    {
        if(activeSquareTextures == null || activeSquareTextures.Count == 0)
        {
            Debug.LogWarning("activeSquareTexture가 비어있음");
            return null;
        }
        //선택 가능한 인덱스 생성
        List<int> availableIndices = new List<int>();
        for(int i =0; i< activeSquareTextures.Count; i++)
        {
            if (!_recentIndices.Contains(i))
            {
                availableIndices.Add(i);
            }
        }

        //모두 제외되면 전체에서 선택
        if(availableIndices.Count == 0)
        {
            for(int i=0; i< activeSquareTextures.Count; i++)
            {
                availableIndices.Add(i);
            }
            _recentIndices.Clear();
        }

        //랜덤 선택
        int randomIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];

        //최근 리스트 업데이트
        _recentIndices.Add(randomIndex);
        if(_recentIndices.Count > 2) // 최근 2개 제외
        {
            _recentIndices.RemoveAt(0);
        }
        return activeSquareTextures[randomIndex].texture;
    }




    public int tresholdVal = 10;    // 색상 변경 임계값
    private const int StartTresholdVal = 100;   // 시작 임계값
    
    public Config.SquareColor currentColor;
    private Config.SquareColor nextColor;

    public int GetCurrentColorIndex()
    {
        //int currentIndex = 0;

        for (int i = 0; i < activeSquareTextures.Count; i++)
        {
            if (activeSquareTextures[i].squareColor == currentColor)
            {                
                return i;
            }
        }
        //return currentIndex;
        return 0;
    }
    
    public void UpdateColors(int current_score)
    {
        currentColor = nextColor;
        int currentColorIndex = GetCurrentColorIndex();

        if(currentColorIndex == activeSquareTextures.Count -1)
        {
            nextColor = activeSquareTextures[0].squareColor;
        }
        else
        {
            nextColor = activeSquareTextures[currentColorIndex + 1].squareColor;
        }

        tresholdVal = StartTresholdVal + current_score;
    }

    public void SetStartColor() //초기화
    {
        tresholdVal = StartTresholdVal;
        currentColor = activeSquareTextures[0].squareColor;
        nextColor = activeSquareTextures[1].squareColor;
    }

    private void Awake()
    {
        SetStartColor();
    }

    private void OnEnable()
    {
        SetStartColor();
    }

}
