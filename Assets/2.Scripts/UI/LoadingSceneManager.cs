using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextscene;

    [SerializeField]
    private Image theLoadingSlider;
    [SerializeField]
    private Text loadingText;

    public static float loadingTime = 0.0f;

    private void Start()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    public static void SetLoadScene(string _sceneName) // 로딩씬을 실행시키는 함수
    {
        nextscene = _sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextscene); //다음 씬을 비동기식 동작
        operation.allowSceneActivation = false; //씬 동작을 비활성화

        while (!operation.isDone)
        {
            yield return null;
            loadingTime += Time.deltaTime;
            if (loadingTime < 5f)
            {
                theLoadingSlider.fillAmount = Mathf.Clamp(loadingTime, 0f,.99f);
                loadingText.text = Mathf.RoundToInt(theLoadingSlider.fillAmount * 100) + "%";

            }
            else
            {
                theLoadingSlider.fillAmount = Mathf.Clamp01(1f);
                loadingText.text = ((int)theLoadingSlider.fillAmount * 100) + "%";
                if (theLoadingSlider.fillAmount == 1.0f)  //만약 로딩바가 100%가 되면
                {
                    yield return new WaitForSecondsRealtime(2f);
                    operation.allowSceneActivation = true; //씬 동작을 활성화
                    loadingTime = 0.0f;
                    yield break;
                }
            }
        }
    }
}