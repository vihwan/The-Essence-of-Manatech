using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    //스테이지를 관리하는 스크립트입니다. 
    //처음 게임 시작 시, UI가 슬라이드 되는 애니메이션을 실행하게 됩니다.

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

    // Start is called before the first frame update
    public void Init()
    {
        animator = GetComponent<Animator>();

        /*        guiCanvas = transform.Find("GUIManagerCanvas").gameObject;
                backgroundCanvas = transform.Find("BackgroundCanvas").gameObject;
                tileCanvas = transform.Find("TileCanvas").gameObject;
                backTileCanvas = transform.Find("BackTileCanvas").gameObject;
                skillEffectCanavs = transform.Find("SkillEffectCanvas").gameObject;*/

        PlayUISlide();
    }

    private void PlayUISlide()
    {
        animator.SetTrigger("Slide");
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if(info.normalizedTime > info.length)
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
        }
    }
}
