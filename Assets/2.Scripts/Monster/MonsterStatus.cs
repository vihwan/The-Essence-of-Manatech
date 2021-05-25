using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatus
{

    private int hp;
    private int mp;
    private int shield;

    public int Hp { get => hp; set => hp = value; }
    public int Mp { get => mp; set => mp = value; }
    public int Shield { get => shield; set => shield = value; }

    public MonsterStatus(int hp, int mp, int shield)
    {
        this.Hp = hp;
        this.Mp = mp;
        this.Shield = shield;
    }

}
