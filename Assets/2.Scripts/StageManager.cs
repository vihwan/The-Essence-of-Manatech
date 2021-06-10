﻿using System;
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

    private Animator animator;

    private AnimatorStateInfo info;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        readyGoText = transform.Find("GUIManagerCanvas/ReadyGoText/Text").GetComponent<TMP_Text>();


        PlayUISlide();
        SoundManager.instance.PlayBGMWithCrossFade("데바스타르");
    }

    private void PlayUISlide()
    {
        animator.SetTrigger("Slide");
    }

    private void GetReady()
    {
        animator.SetBool("UseLibrary", true);
        SoundManager.instance.PlayCV("Player_Ancient_Library");
        SoundManager.instance.PlaySE("PlayerCasting");
    }

    private void PlayReadySound()
    {
        SoundManager.instance.PlaySE("GameReady");
    }

    private void SetGoText()
    {
        readyGoText.text = "Start!";
        SoundManager.instance.PlaySE("GameStart");
        //스타트 사운드 실행


        //버튼 활성화
        GUIManager.instance.OnInitPauseButton();
    }

    private void PlayVersusSoundEffect()
    {
        SoundManager.instance.PlaySE("Thunder");
        SoundManager.instance.PlaySE("Versus");
    }

    private void ChangeGameStateStart()
    {
        GameManager.instance.GameState = GameState.PLAYING;
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


        info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime * 10 > info.length * 0.9f)
        {
            Invoke(nameof(GetReady), 3f);
        }
    }


}
