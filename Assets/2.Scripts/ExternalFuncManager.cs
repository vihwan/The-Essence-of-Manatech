using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//InGameScene에서, 일시정지 메뉴 및 게임 결과 화면 출력을 담당하는 캔버스를 관리하는 컴포넌트입니다.
public class ExternalFuncManager : MonoBehaviour
{
    //싱글톤
    public static ExternalFuncManager Instance;

    private Image fadebg;
    private Color fadeTransparency = new Color(0, 0, 0, .04f);

    public GameObject monsterkillParticle;
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

        fadebg = transform.Find("GameEndUI/fadebg").GetComponent<Image>();
        if (fadebg == null)
            Debug.LogWarning(fadebg.name + "가 참조되지 않았습니다.");

/*        monsterkillParticle = Resources.Load<GameObject>("MonsterKillParticle");
        if (monsterkillParticle != null)
        {
            GameObject go = Instantiate(monsterkillParticle, new Vector2(460, 20), Quaternion.identity, this.transform);
            go.SetActive(false);
        }*/

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
        monsterkillParticle.SetActive(true);
        monsterkillParticle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1f);

        if (GameManager.instance.PlayerState == PlayerState.LOSE)
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


    public void fadebgOut()
    {
        StartCoroutine(IEfadebgOut());
    }

    private IEnumerator IEfadebgOut()
    {
        while (fadebg.color.a < 0.4f)
        {
            fadebg.color += fadeTransparency;
            yield return new WaitForSeconds(.02f);
        }
    }

}
