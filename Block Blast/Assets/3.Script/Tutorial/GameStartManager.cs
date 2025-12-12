using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    public void StartGame()
    {
        PlayerData data = SaveSystem.LoadPlayerData();

        if(data == null)
        {
            //데이터가 없으면 새로 생성
            data = new PlayerData();
            SaveSystem.SavePlayerData(data);

            //튜토리얼 씬 최초 진입
            DOTween.KillAll();
            SceneManager.LoadScene(1);
            return;
        }
                
        //튜토리얼 한 적 없는 경우
        if(data.tutorialCompleted == false)
        {
            DOTween.KillAll();
            SceneManager.LoadScene(1);
        }
        else
        {
            DOTween.KillAll();
            SceneManager.LoadScene(2); //Game
        }
    }
}
