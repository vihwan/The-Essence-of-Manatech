using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextscene;

    private bool isLoading = false;

    [SerializeField] private Image theLoadingSlider;
    [SerializeField] private Text loadingText;
    [SerializeField] private Text toolTipText;

    public static float loadingTime = 0.0f;

    private void Start()
    {
        if (isLoading == false)
        {
            ChangeLoadingBg(nextscene);
            RandomSetTooltipText();
            StartCoroutine(LoadSceneCoroutine());
        }
    }

    public static void SetLoadScene(string _sceneName) // 로딩씬을 실행시키는 함수
    {
        nextscene = _sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator LoadSceneCoroutine()
    {
        isLoading = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextscene); //다음 씬을 비동기식 동작
        operation.allowSceneActivation = false; //씬 동작을 비활성화

        while (!operation.isDone)
        {
            yield return null;
            loadingTime += Time.deltaTime;
            if ( loadingTime < 5f || operation.progress < .9f)
            {
                theLoadingSlider.fillAmount = Mathf.Clamp(loadingTime, 0f,.99f);
                loadingText.text = Mathf.RoundToInt(theLoadingSlider.fillAmount * 100) + "%";
                print("로딩중, Loading Time : " + (int)loadingTime);
            }
            else
            {
                theLoadingSlider.fillAmount = Mathf.Clamp01(1f);
                loadingText.text = ((int)theLoadingSlider.fillAmount * 100) + "%";
                if (theLoadingSlider.fillAmount == 1.0f)  //만약 로딩바가 100%가 되면
                {
                    yield return new WaitForSeconds(2f);
                    operation.allowSceneActivation = true; //씬 동작을 활성화
                    loadingTime = 0.0f;
                    print("NextScene Activate");
                    isLoading = false;
                    yield break;
                }
            }
        }

        isLoading = false;
    }

    //다음 로드하는 씬에 따라, 배경 이미지가 달라지게 만들어주는 함수입니다.
    private void ChangeLoadingBg(string sceneName)
    {
        Image backgroundImage = transform.Find("Background").GetComponent<Image>();
        if(sceneName == "MenuScene")
        {
            print("샨트리 배경");
            backgroundImage.sprite = Resources.Load<Sprite>("loading0");
        }
        else if(sceneName == "InGameScene")
        {
            int rand = UnityEngine.Random.Range(0, 2);
            if(rand == 0)
            {
                print("던전로딩 배경");
                backgroundImage.sprite = Resources.Load<Sprite>("loading1");
            }
            else
            {
                print("던전로딩 배경2");
                backgroundImage.sprite = Resources.Load<Sprite>("loading2");
            }

        }
    }

    //로딩 화면으로 넘어올 때마다 랜덤하게 툴팁을 출력시켜주는 함수입니다.
    private void RandomSetTooltipText()
    {
        int rand = UnityEngine.Random.Range(1, 7);
        string contents = string.Empty;
        switch (rand)
        {
            case 1:
                contents = "잭 오 할로윈은 너무 뜨거운 나머지 옮길 수 없습니다.";
                break;
            case 2:
                contents = "캔디바 타일은 어떤 타일과도 매칭시킬 수 있는 특수한 타일입니다.";
                break;
            case 3:
                contents = "데바스타르에게 봉인된 타일은 옮길 수 없습니다. 대신 스킬을 적극적으로 활용하세요.";
                break;
            case 4:
                contents = "상대가 무력화 상태일때는, 10배만큼 강한 데미지를 줄 수 있습니다.";
                break;
            case 5:
                contents = "키보드 1,2,3,4 혹은 스킬 아이콘을 클릭하여, 해당 스킬을 사용할 수 있습니다.";
                break;
            default:
                contents = "개발자는 피곤해서 잠을 자고 싶습니다.";
                break;
        }
        toolTipText.text = "※Tip. " + contents;
    }
}