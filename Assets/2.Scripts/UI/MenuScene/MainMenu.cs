﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IPointerClickHandler
{
    // Canvas / Panel에 있는 스크립트
    public static MainMenu instance;


    private float ambTime = 30f;
    private float elapsedTime = 0f;

    private NPC_DruidMia mia;
    private NPC_Ikki ikki;
    private NPC_Michelle michelle;

    private GameObject go_Mia;
    private GameObject go_Ikki;
    private GameObject go_Michelle;

    private PopupMenu mia_Popup;
    private PopupMenu ikki_Popup;
    private PopupMenu michelle_Popup;

    private ConfirmMenu confirm_Menu;
    private Animator startAni;

    private Fungus.SayDialog sayDialog;
    private SkillManageMenu skillManageMenu;

    public ConfirmMenu Confirm_Menu { get => confirm_Menu;}
    public SkillManageMenu SkillManageMenu { get => skillManageMenu;}

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        sayDialog = FindObjectOfType<Fungus.SayDialog>();
        if(sayDialog != null)
        {
            print("Success Refer SayDialog");
        }


        mia = GetComponentInChildren<NPC_DruidMia>();
        if (mia != null)
        {
            mia.Init();
        }

        michelle = GetComponentInChildren<NPC_Michelle>();
        if (michelle != null)
        {
            michelle.Init();
        }

        ikki = GetComponentInChildren<NPC_Ikki>();
        if (ikki != null)
        {
            ikki.Init();
        }

        go_Mia = GetComponentInChildren<NPC_DruidMia>().gameObject;
        if (go_Mia != null)
        {
            go_Mia.GetComponent<Button>().onClick.AddListener(OnClickMia);
        }

        go_Ikki = GetComponentInChildren<NPC_Ikki>().gameObject;
        if (go_Ikki != null)
        {
            go_Ikki.GetComponent<Button>().onClick.AddListener(OnClickIkki);
        }

        go_Michelle = GetComponentInChildren<NPC_Michelle>().gameObject;
        if (go_Michelle != null)
        {
            go_Michelle.GetComponent<Button>().onClick.AddListener(OnClickMichelle);
        }

        mia_Popup = go_Mia.transform.Find("PopupMenu").GetComponent<PopupMenu>();
        if(mia_Popup != null)
        {
            mia_Popup.Init();
        }

        ikki_Popup = go_Ikki.transform.Find("PopupMenu").GetComponent<PopupMenu>();
        if (ikki_Popup != null)
        {
            ikki_Popup.Init();
        }

        michelle_Popup = go_Michelle.transform.Find("PopupMenu").GetComponent<PopupMenu>();
        if (michelle_Popup != null)
        {
            michelle_Popup.Init();
        }

        startAni = transform.Find("StartUI").GetComponent<Animator>();

        confirm_Menu = transform.Find("Confirm_Popup").GetComponent<ConfirmMenu>();
        if(Confirm_Menu != null)
        {
            confirm_Menu.Init();
        }

        skillManageMenu = FindObjectOfType<SkillManageMenu>();
        if(skillManageMenu != null)
        {
            skillManageMenu.Init();
        }

        SoundManager.instance.PlayBGMWithCrossFade("샨트리");
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > ambTime)
        {
            mia.AMBVoice();
            elapsedTime = 0f;
        }

        //대화창이 켜진 상태에서는 NPC의 버튼이 동작하지 않는다.
        if (isActiveSayDialog())
            ActiveNPCButton(false);
        else
            ActiveNPCButton(true);
    }

    private bool isActiveSayDialog()
    {
        if (sayDialog.gameObject.activeSelf == true)
            return true;
        else
            return false;
    }

    private void ActiveNPCButton(bool state)
    {
        mia.GetComponent<Button>().enabled = state;
        ikki.GetComponent<Button>().enabled = state;
        michelle.GetComponent<Button>().enabled = state;
    }


    private void OnClickMia()
    {
        SetPopupMenuFalse();
        go_Mia.GetComponent<Button>().interactable = false;
        go_Mia.transform.Find("PopupMenu").gameObject.SetActive(true);

        mia.ClickVoice();
    }

    private void OnClickIkki()
    {
        SetPopupMenuFalse();
        go_Ikki.GetComponent<Button>().interactable = false;
        go_Ikki.transform.Find("PopupMenu").gameObject.SetActive(true);

        ikki.ClickVoice();
    }

    private void OnClickMichelle()
    {
        SetPopupMenuFalse();
        go_Michelle.GetComponent<Button>().interactable = false;
        go_Michelle.transform.Find("PopupMenu").gameObject.SetActive(true);

        michelle.ClickVoice();
    }

    internal void SetPopupMenuFalse()
    {
        go_Mia.transform.Find("PopupMenu").gameObject.SetActive(false);
        go_Michelle.transform.Find("PopupMenu").gameObject.SetActive(false);
        go_Ikki.transform.Find("PopupMenu").gameObject.SetActive(false);

        go_Mia.GetComponent<Button>().interactable = true;
        go_Michelle.GetComponent<Button>().interactable = true;
        go_Ikki.GetComponent<Button>().interactable = true;
    }


    //Debug 용 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        print(eventData.pointerCurrentRaycast.gameObject);
    }

    public void StartGame()
    {
        print("게임 시작 카운트다운");
        startAni.gameObject.SetActive(true);
    }


    //AnimatorEvent
    public void LoadingForStartGame()
    {
        GameManager.instance.LoadScene("InGameScene"); //로딩 씬을 실행
    }

    //Fungus
    public int RandomSelectNum()
    {
        int rand = UnityEngine.Random.Range(0, 3);
        return rand;
    }
}
