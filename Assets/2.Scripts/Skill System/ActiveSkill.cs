using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ActiveSkill
{
    private string name; //스킬 이름
    private string description; //스킬 설명
    private int level; // 스킬 레벨
    private int mana; //필요한 마나양
    private float coolTime; // 스킬 사용 쿨타임
    private int eigenValue = 0; // 스킬 고유 변수값?

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public int Level { get => level; set => level = value; }
    public int Mana { get => mana; set => mana = value; }
    public float CoolTime { get => coolTime; set => coolTime = value; }
    public int EigenValue { get => eigenValue; set => eigenValue = value; }

    public ActiveSkill(string name, string description, int level, int mana, float coolTime, int eigenValue)
    {
        this.Name = name;
        this.Description = description;
        this.Level = level;
        this.Mana = mana;
        this.CoolTime = coolTime;
        this.EigenValue = eigenValue;
    }
}