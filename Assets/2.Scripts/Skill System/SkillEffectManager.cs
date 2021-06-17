﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillEffectType
{
    Chain,
    Flapper,
    Ice,
    Halloween
}


//플레이어 스킬의 이펙트 애니메이션 컨트롤을 관리하는 컴포넌트입니다.
//Animation Event를 활용하여, 특정 시간대에 Event를 발생시키고, 그에 맞는 함수를 실행합니다.

public class SkillEffectManager : MonoBehaviour
{
    //Animator Components
    private Animator animChainFluore;
    private Animator animFlapper;

    private HintManager hintManager;

    public void Init()
    {
        animChainFluore = transform.Find("ChainFluore").GetComponent<Animator>();
        if (animChainFluore == null)
            Debug.LogWarning("체인플로레 Animator가 참조되지 않았습니다.");

        animFlapper = transform.Find("Flapper").GetComponent<Animator>();
        if (animFlapper == null)
            Debug.LogWarning("변이파리채 Animator가 참조되지 않았습니다.");

        hintManager = FindObjectOfType<HintManager>();
        if(hintManager == null)
            Debug.LogWarning("HintManager 가 참조되지 않았습니다.");
    }


    internal void ActiveSkillEffect(SkillEffectType type)
    {
        switch (type)
        {
            case SkillEffectType.Chain:
                animChainFluore.SetTrigger("Active");
                break;

            case SkillEffectType.Flapper:
                animFlapper.SetTrigger("Active");
                break;

            case SkillEffectType.Ice:
                break;

            case SkillEffectType.Halloween:
                break;
        }
    }

    //Animation Event - 1 Skill
    private void ChainFluore()
    {
        BoardManager.instance.currentState = PlayerState.USESKILL;
        hintManager.MarkHint();
    }


    //Animation Event - 2 Skill
    private void ChangePlutoTile()
    {
        //플레이어는 스킬 사용중
        BoardManager.instance.currentState = PlayerState.USESKILL;
        BoardManager.instance.ChangePlutoTile();
    }
}
