using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ActiveSkill
{
    [SerializeField] public string name; //스킬 이름
    [SerializeField] public string description; //스킬 설명
    [SerializeField] public Image icon; //스킬 아이콘 이미지
    [SerializeField] public int level; // 스킬 레벨
    [SerializeField] public int necessaryMana; //필요한 마나양
    [SerializeField] public float cooldownTime; // 스킬 사용 쿨타임
}