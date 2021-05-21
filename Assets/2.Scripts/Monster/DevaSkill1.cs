﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


internal class Deva1
{
    public int row;
    public int col;
}


public class DevaSkill1 : MonoBehaviour
{
    /*
     * 데바스타르 (인간) 스킬
     * 
     * 플레이어 타일 중 랜덤하게 5개를 골라 움직이지 못하게 하는 마법진을 설치합니다.
     * 제한 시간 내에 마법진을 모두 파괴하면, 데바스타르는 그로기 상태가 됩니다.
     * 모두 파괴하지 못하면, 광폭화 스킬이 발동되어 플레이어는 큰 데미지를 입습니다.
     * 
     * **/

    private List<Deva1> deva1s = new List<Deva1>();
    public List<GameObject> go_List;

    private GameObject rootUI;
    private TMP_Text tmp_Text;
    private Image limitTimeImage;

    [SerializeField] private float limitTime;
    [SerializeField] private float remainTime;
    public bool isRemainTimeUpdate = false;
    public bool isUsingSkill = false;

    private MonsterNotify notify;
    internal bool isBerserk = false;

    public void Init()
    {
        rootUI = transform.Find("RootUI").gameObject;
        if (rootUI != null)
            rootUI.SetActive(false);

        tmp_Text = GetComponentInChildren<TMP_Text>(true);

        limitTimeImage = transform.Find("RootUI/SkillLimitTimeSlide/BaseUI/Gauge").GetComponent<Image>();
        if (limitTimeImage != null)
            limitTimeImage.fillAmount = 1f;

        notify = FindObjectOfType<MonsterNotify>();
    }

    public void Execute()
    {
        limitTime = 60f;
        remainTime = limitTime;
        GaugeUpdate();
        rootUI.SetActive(true);

        //목소리 출력
        //"서로를 옭아매는 어리석은 인간이여"

        SoundManager.instance.PlayCV("Human_Skill");

        deva1s.Clear();
        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                if (x == 0 || x == BoardManager.instance.width - 1 || y == 0 || y == BoardManager.instance.height - 1)
                    continue;

                Tile tile = BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>();

                if (tile.CompareTag("Lolipop") || tile.CompareTag("Bomb"))
                    continue;

                deva1s.Add(new Deva1() { row = tile.Row, col = tile.Col });
            }
        }
        StartCoroutine(MakeMagicCircle());
    }

    private void Update()
    {
        if (isRemainTimeUpdate)
        {
            remainTime -= Time.deltaTime;

            GaugeUpdate();

            if (remainTime <= 0)
            {
                remainTime = 0f;
                limitTimeImage.fillAmount = 0f;

                if (BoardManager.instance.currentState == PlayerState.MOVE
                        && BoardManagerMonster.instance.currentState == MonsterState.USESKILL)
                {
                    SkillBerserk();
                    isRemainTimeUpdate = false;
                }
            }
            else
            {
                if (go_List.Count == 0)
                {
                    //패턴 파훼 성공
                    Debug.Log("<color=#0456F1>패턴 파훼</color> 성공!!");
                    //그로기 타임
                    BoardManagerMonster.instance.currentState = MonsterState.GROGGY;
                    SoundManager.instance.PlayCV("Human_Skill_Groggy");
                    rootUI.SetActive(false);
                    isRemainTimeUpdate = false;
                }
            }

        }
    }

    private void GaugeUpdate()
    {
        limitTimeImage.fillAmount = remainTime / limitTime;
        tmp_Text.text = remainTime + "s";
    }

    private void SkillBerserk()
    {
        isBerserk = true;
        // 플레이어가 제한시간 내에 패턴을 파훼하지 못했을 시 발동한다.
        // 400의 데미지를 줌
        // 보이스 출력 : 파멸하라
        // 알림 이미지 출력
        notify.SetText("파멸하라!");
        notify.PlayAnim();
        SoundManager.instance.PlayCV("Human_Skill_Berserk");

 

        for (int i = 0; i < go_List.Count; i++)
        {
            //마법진 폭발 이펙트 실행
            Destroy(go_List[i].gameObject);
        }

        //캐릭터 타일 봉인 해제
        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                if (BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>().isSealed)
                {
                    BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>().isSealed = false;
                }
                else
                    continue;
            }
        }

        PlayerStatusController playerStatusController = FindObjectOfType<PlayerStatusController>();
        playerStatusController.DecreaseHP(400);
        go_List.Clear();
        rootUI.SetActive(false);
        isBerserk = false;
    }

    private IEnumerator MakeMagicCircle()
    {
        isUsingSkill = true;
        for (int i = 0; i < 5; i++)
        {
            int rIndex = Random.Range(0, deva1s.Count);

            int x = deva1s[rIndex].row;
            int y = deva1s[rIndex].col;
            deva1s.RemoveAt(rIndex);

            Tile tile = BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>();
            tile.isSealed = true;

            SealedEffect seal = Instantiate(Resources.Load<SealedEffect>("circleC")
                    , tile.transform.position
                    , Quaternion.identity);
            seal.transform.SetParent(tile.transform);
            go_List.Add(seal.gameObject);

            yield return new WaitForSeconds(.5f);
        }
        isUsingSkill = false;
        isRemainTimeUpdate = true;
    }
}
