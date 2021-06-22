using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//InGameScene에서, 일시정지 메뉴 및 게임 결과 화면 출력을 담당하는 캔버스를 관리하는 컴포넌트입니다.
public class ExternalFuncManager : MonoBehaviour
{
    //싱글톤
    public static ExternalFuncManager Instance;

    private Animator gameEndUIAnim;

    private PauseMenu pauseMenu;
    private ResultPage resultPage;


    // Start is called before the first frame update
    public void Init()
    {
        Instance = this;

        pauseMenu = GetComponentInChildren<PauseMenu>(true);
        if (pauseMenu != null)
            pauseMenu.Init();

        resultPage = GetComponentInChildren<ResultPage>(true);
        if (resultPage != null)
            resultPage.Init();

        gameEndUIAnim = transform.Find("GameEndUI").GetComponent<Animator>();
        if (gameEndUIAnim == null)
            Debug.LogWarning(gameEndUIAnim.name + "가 참조되지 않았습니다.");
    }


    public void OpenPauseMenu()
    {
        pauseMenu.CallMenu();
    }

    //게임 오버 전에 대기 시간을 주는 코루틴
    //BoardState가 MOVE가 될때 까지 기다림
    public IEnumerator WaitForShifting()
    {
        //일단 임시로 5초를 기다림
        //5초면 충분히 나머지 모든 처리가 끝날 수 있다고 생각

        yield return new WaitForSeconds(1f);

        if(GameManager.instance.PlayerState == PlayerState.LOSE)
            PlayerSound.DeadVoice();

        yield return new WaitForSeconds(1f);

        if (GameManager.instance.PlayerState == PlayerState.WIN)
        {
            gameEndUIAnim.SetTrigger("Win");
            SoundManager.instance.PlaySE("aradpvp_result_reward_success");
        }
        else if (GameManager.instance.PlayerState == PlayerState.LOSE)
        {
            gameEndUIAnim.SetTrigger("Lose");
            SoundManager.instance.PlaySE("aradpvp_result_lose");

        }
        
        yield return new WaitForSeconds(3f);
        resultPage.GameOverPanel(); //GUI GameOver Panel
    }
}
