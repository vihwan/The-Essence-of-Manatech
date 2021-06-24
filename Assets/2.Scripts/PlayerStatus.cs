using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus 
{
    // Start is called before the first frame update
    private int hp;
    private int mp;


    public int Hp { get => hp; set => hp = value; }
    public int Mp { get => mp; set => mp = value; }

    public PlayerStatus(int hp, int mp)
    {
        this.Hp = hp;
        this.Mp = mp;    
    }
}
