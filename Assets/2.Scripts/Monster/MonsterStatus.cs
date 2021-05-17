using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatus
{

    private int hp;
    private int mp;

    public int Hp { get => hp; set => hp = value; }
    public int Mp { get => mp; set => mp = value; }

    public MonsterStatus(int hp, int mp)
    {
        this.Hp = hp;
        this.Mp = mp;
    }

}
