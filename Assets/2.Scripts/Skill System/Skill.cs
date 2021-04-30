using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    [SerializeField] private string name; //스킬 이름
    [SerializeField] private string description; //스킬 설명
    [SerializeField] private Image icon; //스킬 아이콘 이미지
    [SerializeField] private int level; // 스킬 레벨
    [SerializeField] private float necessaryMana; //필요한 마나양
    [SerializeField] private float coolTime; // 스킬 사용 쿨타임

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Image Icon { get => icon; set => icon = value; }
    public int Level { get => level; set => level = value; }
    public float NecessaryMana { get => necessaryMana; set => necessaryMana = value; }
    public float CoolTime { get => coolTime; set => coolTime = value; }
}