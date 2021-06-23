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
    private bool isActive = false;
    internal bool isBerserk = false;

    private GameObject rootUI;
    private TMP_Text remainTimeText;
    private Image limitTimeImage;
    private PlayerStatusController player;

    private GameObject NenBarrierPrefab;
    private GameObject go_NenBarrier;

    private Animator convictionAni;

    public bool IsActive { get => isActive; set => isActive = value; }

    // Start is called before the first frame update
    public void Init()
    {
        rootUI = transform.Find("RootUI").gameObject;
        if (rootUI != null)
            rootUI.SetActive(false);

        remainTimeText = GetComponentInChildren<TMP_Text>(true);

        limitTimeImage = transform.Find("RootUI/SkillLimitTimeSlide/BaseUI/Gauge").GetComponent<Image>();
        if (limitTimeImage != null)
            limitTimeImage.fillAmount = 1f;

        player = FindObjectOfType<PlayerStatusController>();

        convictionAni = transform.Find("Conviction").GetComponent<Animator>();

        NenBarrierPrefab = Resources.Load<GameObject>("NenBarrier");
        if (NenBarrierPrefab == null)
        {
            Debug.LogWarning(NenBarrierPrefab.name + "이 참조되지 않았습니다.");
        }
    }
    private void GaugeUpdate()
    {
        limitTimeImage.fillAmount = remainTime / limitTime;
        remainTimeText.text = Mathf.RoundToInt(remainTime) + "s";
    }

    // Update is called once per frame
    public void Execute()
    {
        limitTime = 15f;
        remainTime = limitTime;
        GaugeUpdate();
        rootUI.SetActive(true);

        IsActive = true;

        // 사운드 출력
        // 그 분을 대신하여 (스킬 2)
        MonsterAI.instance.SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_Two);
        SoundManager.instance.PlayMonV("devastar_devil_conviction_start");
        deva2s.Clear();

        //타일을 랜덤하게 3개를 선택 한 후, 그 타일에 넨 이펙트를 생성한다.
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

            NenEffect nen = Instantiate(Resources.Load<NenEffect>("DevaSkill_2_NenEffect")
                    , tile.transform.position
                    , Quaternion.identity);
            nen.transform.SetParent(tile.transform);
            go_List2.Add(nen.gameObject);
        }

        IsActive = false;
        isRemainTimeUpdate = true;
    }

    private void Update()
    {
        if (GameManager.instance.GameState != GameState.PLAYING)
            return;

        if (MonsterAI.instance.IsHolding == true)
            return;

        if (isRemainTimeUpdate)
        {
            remainTime -= Time.deltaTime;

            GaugeUpdate();

            if (player.IsInvincible == false)
            {
                if (go_List2.Count == 0)
                {
                    player.IsInvincible = true;
                    Debug.Log("패턴을 파훼하여 무적 상태입니다.");
                    SoundManager.instance.PlayCV("wz_shield");

                    go_NenBarrier = Instantiate(NenBarrierPrefab
                        , GameObject.Find("BackgroundCanvas/BoardRoot/BoardImagePlayer").transform.position
                        , Quaternion.identity
                        , this.transform);
                    go_NenBarrier.transform.SetParent(GUIManager.instance.transform, true);
                }
            }

            if (remainTime <= 0)
            {
                remainTime = 0f;
                limitTimeImage.fillAmount = 0f;

                if (MonsterAI.instance.Action == MonsterState.CASTING && isBerserk == false
                    && BoardManager.instance.currentState == PlayerState.MOVE)
                {
                    if (!isBerserk)
                    {
                        StartCoroutine(SkillBerserk());
                        isRemainTimeUpdate = false;
                        MonsterAI.instance.isUsingSkill = false;
                    }
                }
            }
        }
    }

    private IEnumerator SkillBerserk()
    {
        isBerserk = true;

        // 너희들을 심판한다!
        MonsterAI.instance.Action = MonsterState.BERSERK;
        MonsterAI.instance.SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_Two_Next);

        rootUI.SetActive(false);

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

        //임시
        yield return new WaitForSeconds(1.5f);

        convictionAni.SetTrigger("Active");
    }

    public void ActiveConviction()
    {
        //컨빅션
        MonsterAI.instance.SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_Two_Final);
        PlaySoundSpearExplosion();

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
            PlayerSound.PlayPhaseShift();
        }

        go_List2.Clear();
        if(go_NenBarrier != null)
            Destroy(go_NenBarrier);

        MonsterAI.instance.Action = MonsterState.MOVE;
        isBerserk = false;
    }

    private void PlaySoundSpearCreate()
    {
        SoundManager.instance.PlayMonV("devastar_devil_conviction_l_spear_create");
    }

    private void PlaySoundLandingSpear()
    {
        SoundManager.instance.PlayMonV("devastar_devil_conviction_l_spear_smash");
    }

    private void PlaySoundSpearExplosion()
    {
        SoundManager.instance.PlayMonV("devastar_devil_conviction_l_spear_exp");
    }
}
