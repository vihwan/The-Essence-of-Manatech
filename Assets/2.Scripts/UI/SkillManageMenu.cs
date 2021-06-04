using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 스킬 관리 정보창을 관리하는 컴포넌트입니다.
// 기본적으로 UI 닫기 버튼, 액티브 스킬창 보기, 패시브 스킬창 보기를 담당합니다.
public class SkillManageMenu : MonoBehaviour
{

    //Button 컴포넌트
    private Button exitBtn;
    private Button openActBtn;
    private Button openPassBtn;

    //외부 컴포넌트
    private ActivePage activePage;
    private PassivePage passivePage;

    private SkillData skillData;
    //private SkillLevelSystem skillLevelSystem;

    public void Init()
    {
        exitBtn = transform.Find("Background/ExitButton").GetComponent<Button>();
        if (exitBtn != null)
            exitBtn.onClick.AddListener(CloseSkillMenu);

        openActBtn = transform.Find("Background/ButtonAct").GetComponent<Button>();
        if (openActBtn != null)
            openActBtn.onClick.AddListener(OpenActiveSkillPage);

        openPassBtn = transform.Find("Background/ButtonPass").GetComponent<Button>();
        if (openPassBtn != null)
            openPassBtn.onClick.AddListener(OpenPassiveSkillPage);

        //임시 설정
        activePage = GetComponentInChildren<ActivePage>(true);
        if(activePage != null)
        {
            activePage.Init();
        }
        passivePage = GetComponentInChildren<PassivePage>(true);
        if(passivePage != null)
        {
            passivePage.Init();
        }
        else
        {
            Debug.Log("패시브 페이지를 참조하지 못했습니다.");
        }

        skillData = FindObjectOfType<SkillData>();
        if(skillData == null)
        {
            Debug.Log("스킬 데이터를 참조하지 못했습니다.");
        }

/*        skillLevelSystem = this.gameObject.GetComponent<SkillLevelSystem>();
        if (skillLevelSystem != null)
        {
            skillLevelSystem.Init();
        }*/

        CloseSkillMenu();
    }

    private void OpenActiveSkillPage()
    {
        CloseAllSkillPage();
        activePage.gameObject.SetActive(true);
    }

    private void OpenPassiveSkillPage()
    {
        CloseAllSkillPage();
        passivePage.gameObject.SetActive(true);
    }

    private void CloseAllSkillPage()
    {
        activePage.gameObject.SetActive(false);
        passivePage.gameObject.SetActive(false);
    }

    public void OpenSkillMenu()
    {
        this.gameObject.SetActive(true);
    }

    public void CloseSkillMenu()
    {
        //Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }
}
