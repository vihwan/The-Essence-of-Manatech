using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    //스테이지를 관리하는 스크립트입니다. 
    //처음 게임 시작 시, UI가 슬라이드 되는 애니메이션을 실행하게 됩니다.

    private TMP_Text readyGoText;


    private BoardManager boardManager;
    private BoardManagerMonster boardManagerMonster;
    private GUIManager guiManager;
    private ScoreManager scoreManager;
    private CreateBackTiles createBoard;
    private CreateBackTilesMonster createBoardMonster;
    private ComboSystem comboSystem;
    private SkillManager skillManager;
    private MonsterAI monsterAI;
    private ExternalFuncManager exFuncManager;

    private Animator animator;

    private AnimatorStateInfo info;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        readyGoText = transform.Find("GUIManagerCanvas/ReadyGoText/Text").GetComponent<TMP_Text>();

        PlayUISlide();
        SoundManager.instance.PlayBGMWithCrossFade("resting_place_for_extinction_p1");

        GameManager.instance.GameState = GameState.READY;
    }

    private void PlayUISlide()
    {
        animator.SetTrigger("Slide");
    }

    private void GetReady()
    {
        animator.SetBool("UseLibrary", true);
        SoundManager.instance.PlayCV("wz_ancient_library");
        SoundManager.instance.PlaySE("cherry_blossom_sunlight");
        //고대의 도서관 효과 적용
        GUIManager.instance.LimitTime += SkillManager.instance.PasSkillDic["고대의 도서관"].EigenValue;
    }

    private void PlayReadySound()
    {
        SoundManager.instance.PlaySE("game_ready");
    }

    private void SetGoText()
    {
        readyGoText.text = "Start!";
        SoundManager.instance.PlaySE("game_start");
        //스타트 사운드 실행
    }

    private void PlayVersusSoundEffect()
    {
        SoundManager.instance.PlaySE("prey_phase_clear_thunder");
        SoundManager.instance.PlaySE("versus");
        Invoke(nameof(PlayDevaMeetSound), 1f);
        
    }

    private void PlayDevaMeetSound()
    {
        int randNum = UnityEngine.Random.Range(1, 4);
        if (randNum == 1)
        {
            SoundManager.instance.PlayMonV("devastar_meet_01");
        }
        else if (randNum == 2)
        {
            SoundManager.instance.PlayMonV("devastar_meet_02");
        }
        else if (randNum == 3)
        {
            SoundManager.instance.PlayMonV("devastar_meet_03");
        }
        else
            return;
    }

    private void ChangeGameStateStart()
    {
        GameManager.instance.GameState = GameState.PLAYING;
        BoardManager.instance.currentState = PlayerState.MOVE;
    }

    private void FindObjects()
    {
        boardManagerMonster = FindObjectOfType<BoardManagerMonster>();
        if (boardManagerMonster != null)
            boardManagerMonster.Init();

        boardManager = FindObjectOfType<BoardManager>();
        if (boardManager != null)
            boardManager.Init();

        createBoard = FindObjectOfType<CreateBackTiles>();
        if (createBoard != null)
            createBoard.Init();

        createBoardMonster = FindObjectOfType<CreateBackTilesMonster>();
        if (createBoardMonster != null)
            createBoardMonster.Init();

        guiManager = FindObjectOfType<GUIManager>();
        if (guiManager != null)
            guiManager.Init();

        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
            scoreManager.Init();

        comboSystem = FindObjectOfType<ComboSystem>();
        if (comboSystem != null)
            comboSystem.Init();

        skillManager = FindObjectOfType<SkillManager>();
        if (skillManager != null)
            skillManager.Init();

        monsterAI = FindObjectOfType<MonsterAI>();
        if (monsterAI != null)
            monsterAI.Init();

        exFuncManager = FindObjectOfType<ExternalFuncManager>();
        if (exFuncManager != null)
            exFuncManager.Init();


        info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime * 10 > info.length * 0.9f)
        {
            Invoke(nameof(GetReady), 3f);
        }
    }

    private void PlaySECameraMove()
    {
        SoundManager.instance.PlaySE("cinematic_pass_whoosh");
    }
}
