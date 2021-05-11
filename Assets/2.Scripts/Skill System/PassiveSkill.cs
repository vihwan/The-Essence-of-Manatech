using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveSkill
{
    private string name; //스킬 이름
    private string description; //스킬 설명
    private int level; // 스킬 레벨
    private float eigenValue; //스킬마다 가지는 고유값

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public int Level { get => level; set => level = value; }
    public float EigenValue { get => eigenValue; set => eigenValue = value; }

    public PassiveSkill(string name, string description, int level, float eigenValue)
    {
        this.Name = name;
        this.Description = description;
        this.Level = level;
        this.eigenValue = eigenValue;
    }
}