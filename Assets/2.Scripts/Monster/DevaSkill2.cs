using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class DevaSkill2 : MonoBehaviour
{
    private List<Deva> deva2s = new List<Deva>();
    public List<GameObject> go_List2;

    [SerializeField] private float limitTime;
    [SerializeField] private float remainTime;

    public bool isRemainTimeUpdate = false;
    public bool isUsingSkill = false;
    internal bool isBerserk = false;

    private GameObject rootUI;
    private TMP_Text tmp_Text;
    private Image limitTimeImage;
    private PlayerStatusController player;

    // Start is called before the first frame update
    public void Init()
    {
        rootUI = transform.Find("RootUI").gameObject;
        if (rootUI != null)
            rootUI.SetActive(false);

        tmp_Text = GetComponentInChildren<TMP_Text>(true);

        limitTimeImage = transform.Find("RootUI/SkillLimitTimeSlide/BaseUI/Gauge").GetComponent<Image>();
        if (limitTimeImage != null)
            limitTimeImage.fillAmount = 1f;

        player = FindObjectOfType<PlayerStatusController>();

    }
    private void GaugeUpdate()
    {
        limitTimeImage.fillAmount = remainTime / limitTime;
        tmp_Text.text = remainTime + "s";
    }

    // Update is called once per frame
    public void Execute()
    {
        limitTime = 15f;
        remainTime = limitTime;
        GaugeUpdate();
        rootUI.SetActive(true);

        isUsingSkill = true;

        // 사운드 출력
        // 그 분을 대신하여
        SoundManager.instance.PlayCV("Devil_Skill1");

        deva2s.Clear();

        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                if (x == 0 || x == BoardManager.instance.width - 1 || y == 0 || y == BoardManager.instance.height - 1)
                    continue;

                Tile tile = BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>();

                deva2s.Add(new Deva() { row = tile.Row, col = tile.Col });
            }
        }

        for (int i = 0; i < 3; i++)
        {
            int rIndex = UnityEngine.Random.Range(0, deva2s.Count);

            int x = deva2s[rIndex].row;
            int y = deva2s[rIndex].col;
            deva2s.RemoveAt(rIndex);

            Tile tile = BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>();
            tile.isActiveNen = true;

            NenEffect nen = Instantiate(Resources.Load<NenEffect>("NenEffect")
                    , tile.transform.position
                    , Quaternion.identity);
            nen.transform.SetParent(tile.transform);
            go_List2.Add(nen.gameObject);
        }

        isUsingSkill = false;
        isRemainTimeUpdate = true;
    }

    private void Update()
    {
        if (isRemainTimeUpdate)
        {
            remainTime -= Time.deltaTime;

            GaugeUpdate();

            if(player.IsInvincible == false)
            {
                if (go_List2.Count == 0)
                {
                    player.IsInvincible = true;
                    Debug.Log("패턴을 파훼하여 무적 상태입니다.");
                }
            }

            if (remainTime <= 0)
            {
                remainTime = 0f;
                limitTimeImage.fillAmount = 0f;

                if (MonsterAI.instance.Action == MonsterState.CASTING && isBerserk == false
                    && BoardManager.instance.currentState == PlayerState.MOVE)
                {
                    SkillBerserk();
                    isRemainTimeUpdate = false;
                }
            }
        }
    }

    private void SkillBerserk()
    {
        isBerserk = true;

        // 너희들을 심판한다! 컨빅션
        MonsterAI.instance.Notify.SetText("너희들을 심판한다!");
        MonsterAI.instance.Notify.PlayAnim();
        SoundManager.instance.PlayCV("Devil_Skill1_Berserk");

        for (int i = 0; i < go_List2.Count; i++)
        {
            //넨가드 이펙트 제거
            Destroy(go_List2[i].gameObject);
        }

        //캐릭터 타일 넨가드 해제
        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                Tile tile = BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>();

                if (tile.isActiveNen)
                {
                    tile.isActiveNen = false;
                }
            }
        }

        //광폭화시 플레이어가 무적이 아니라면 데미지를 입는다.
        if (player.IsInvincible == false)
        {
            player.DecreaseHP(300);
        }
        else
        { 
            //무적이라면 무적을 해제한다.
            //넨가드 이펙트를 없앤다.
            player.IsInvincible = false;
        }

        go_List2.Clear();
        rootUI.SetActive(false);
        isBerserk = false;
        MonsterAI.instance.Action = MonsterState.MOVE;
    }
}
