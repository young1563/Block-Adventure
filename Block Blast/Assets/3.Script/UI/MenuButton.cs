using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.6f;
    
    private void Awake()
    {
        if(Application.isEditor == false)
        {
            Debug.unityLogger.logEnabled = false;
        }

        //시작 시 fadeImage 투명하게 만들기
        if (fadeImage != null) 
        {
            Color fc = fadeImage.color;
            fc.a = 0;
            fadeImage.color = fc;
            fadeImage.gameObject.SetActive(true);        
        }
    }

    public void LoadScene(int number)
    {
        DOTween.KillAll();
        SceneManager.LoadScene(number);
    }

    public void Restart()
    {
        StartCoroutine(FadeAndLoadScene(2));        
    }

    private IEnumerator FadeAndLoadScene(int sceneNumber)
    {
        yield return StartCoroutine(FadeOut());
        DOTween.KillAll();
        SceneManager.LoadScene(sceneNumber);
    }

    private IEnumerator FadeOut()
    {
        if(fadeImage == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Color fc = fadeImage.color;

        //카메라 자연스러운 zoom in
        Transform cam = Camera.main.transform;
        Vector3 originalScale = cam.localScale;
        Vector3 targetScale = originalScale * 1.05f; //5% 확대

        while(elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            fc.a = Mathf.SmoothStep(0, 1, t);
            fadeImage.color = fc;

            //카메라 확대
            cam.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.SmoothStep(0, 1, t));

            yield return null;
        }

        fc.a = 1;
        fadeImage.color = fc;
    }
}
