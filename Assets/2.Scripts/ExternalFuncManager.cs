using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//InGameScene에서, 일시정지 메뉴 및 게임 결과 화면 출력을 담당하는 캔버스를 관리하는 컴포넌트입니다.
public class ExternalFuncManager : MonoBehaviour
{
    //싱글톤
    public static ExternalFuncManager Instance;

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
    }

    public void OpenPauseMenu()
    {
        pauseMenu.CallMenu();
    }

    //게임 오버 전에 대기 시간을 주는 코루틴
    //BoardState가 MOVE가 될때 까지 기다림
    public IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => BoardManager.instance.currentState == PlayerState.MOVE);
       // yield return new WaitUntil(() => MonsterAI.instance.Action == MonsterState.MOVE);
        yield return new WaitForSeconds(1f);
        resultPage.GameOverPanel(); //GUI GameOver Panel
    }
}
