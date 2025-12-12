using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public GameObject losePopup;
    public GameObject newBestScorePopup;
    public Text endScore; //최종 점수

    private void Start()
    {        
        gameOverPopup.SetActive(false);        
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        bool isNew = Scores._newBestScores;
        int score = Scores._currentScores;
      
        if(endScore != null)
        {
            endScore.text = score.ToString();
        }
        
        gameOverPopup.SetActive(true);        

        if (isNew)
        {
            losePopup.SetActive(false);
            newBestScorePopup.SetActive(true);
        }
        else
        {        
            losePopup.SetActive(true);
            newBestScorePopup.SetActive(false);
        }          

    }

}
