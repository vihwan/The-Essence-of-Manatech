using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    [SerializeField] private int id;
    [SerializeField] private string name; //스킬 이름
    [SerializeField] private string description; //스킬 설명
    [SerializeField] private Sprite icon; //스킬 아이콘 이미지
    [SerializeField] private int level; // 스킬 레벨
    [SerializeField] internal int necessaryMana; //필요한 마나양
    [SerializeField] internal float cooldownTime; // 스킬 사용 쿨타임
}