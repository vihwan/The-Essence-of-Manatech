using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//InGameScene에서, 일시정지 메뉴 및 게임 결과 화면 출력을 담당하는 캔버스를 관리하는 컴포넌트입니다.
public class ExternalFuncManager : MonoBehaviour
{
    //싱글톤
    public static ExternalFuncManager Instance;

    private PauseMenu pauseMenu;

    // Start is called before the first frame update
    public void Init()
    {
        Instance = this;

        pauseMenu = GetComponentInChildren<PauseMenu>(true);
        if (pauseMenu != null)
            pauseMenu.Init();
    }

    public void OpenPauseMenu()
    {
        pauseMenu.CallMenu();
    }
}
