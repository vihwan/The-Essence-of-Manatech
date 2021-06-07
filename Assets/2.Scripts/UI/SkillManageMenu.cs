﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 스킬 관리 정보창을 관리하는 컴포넌트입니다.
// 기본적으로 UI 닫기 버튼, 액티브 스킬창 보기, 패시브 스킬창 보기를 담당합니다.
//SKillManageCanvas에 추가되어있음
public class SkillManageMenu : MonoBehaviour
{
    //Text 컴포넌트
    private static Text spText; //스킬 포인트

    //Button 컴포넌트
    private Button exitBtn;
    private Button openActBtn;
    private Button openPassBtn;

    //Image 컴포넌트
    private Image tapFocusAct;
    private Image tapFocusPass;

    //외부 컴포넌트
    private ActivePage activePage;
    private PassivePage passivePage;
    private SkillData skillData;


    public void Init()
    {
        exitBtn = transform.Find("Background/ExitButton").GetComponent<Button>();
        if (exitBtn != null)
            exitBtn.onClick.AddListener(CloseSkillMenu);

        openActBtn = transform.Find("Background/ButtonAct").GetComponent<Button>();
        if (openActBtn != null)
        {
            openActBtn.onClick.AddListener(OpenActiveSkillPage);       
        }

        openPassBtn = transform.Find("Background/ButtonPass").GetComponent<Button>();
        if (openPassBtn != null)
        {
            openPassBtn.onClick.AddListener(OpenPassiveSkillPage);        
        }

        //임시 설정
        activePage = GetComponentInChildren<ActivePage>(true);
        if (activePage != null)
        {
            activePage.Init();
        }
        passivePage = GetComponentInChildren<PassivePage>(true);
        if (passivePage != null)
        {
            passivePage.Init();
        }

        skillData = FindObjectOfType<SkillData>();
        if (skillData == null)
        {
            Debug.Log("스킬 데이터를 참조하지 못했습니다.");
        }

        spText = transform.Find("SPText").GetComponent<Text>();
        if (spText != null)
        {
            UpdateSpText();
        }

        tapFocusAct = transform.Find("Background/ButtonAct/TapFocus").GetComponent<Image>();
        tapFocusPass = transform.Find("Background/ButtonPass/TapFocus").GetComponent<Image>();

        CloseSkillMenu();
    }

    //스킬 페이지 창들을 초기화시킨다.
    private void InitPage()
    {
        //스킬 관리창을 열면
        //1. 액티브 스킬 버튼 창은 켜져 있어야한다.
        //2. 패시브 스킬 버튼 창은 꺼져 있어야한다.
        //3. 상세정보창은 꺼져 있어야 한다.
        //4. tapFocus는 액티브 스킬꺼가 켜져 있어야한다.

        activePage.gameObject.SetActive(true);
        passivePage.gameObject.SetActive(false);

        activePage.ExplainPage.gameObject.SetActive(false);
        passivePage.ExplainPage.gameObject.SetActive(false);

        tapFocusAct.gameObject.SetActive(true);
        tapFocusPass.gameObject.SetActive(false);
    }

    //액티브 스킬 창을 연다.
    private void OpenActiveSkillPage()
    {
        CloseAllSkillPage();
        InActiveAllTapFocus();
        activePage.gameObject.SetActive(true);
        tapFocusAct.gameObject.SetActive(true);
    }

    //패시브 스킬 창을 연다
    private void OpenPassiveSkillPage()
    {
        CloseAllSkillPage();
        InActiveAllTapFocus();
        passivePage.gameObject.SetActive(true);
        tapFocusPass.gameObject.SetActive(true);
    }

    //모든 스킬 페이지창을 닫는다.
    private void CloseAllSkillPage()
    {
        activePage.gameObject.SetActive(false);
        passivePage.gameObject.SetActive(false);
    }

    //스킬 관리창을 연다.
    public void OpenSkillMenu()
    {
        this.gameObject.SetActive(true);
    }

    //스킬 관리창을 닫는다.
    public void CloseSkillMenu()
    {
        InitPage();
        this.gameObject.SetActive(false);
    }

    private void InActiveAllTapFocus()
    {
        tapFocusAct.gameObject.SetActive(false);
        tapFocusPass.gameObject.SetActive(false);
    }

    public static void UpdateSpText()
    {
        spText.text = "스킬 포인트 : " + PlayerData.SkillPoint.ToString();
    }
}
