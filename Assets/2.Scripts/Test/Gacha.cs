using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha : MonoBehaviour
{
    [Range(0.0f, 100)]
    public float commonProb = 55f;
    [Range(0.0f, 100)]
    public float uncommonProb = 30f;
    [Range(0.0f, 100)]
    public float rareProb = 10f;
    [Range(0.0f, 100)]
    public float uniqueProb = 4f;
    [Range(0.0f, 100)]
    public float legendaryProb = 1f;

    public int commonCount;
    public int uncommonCount;
    public int rareCount;
    public int uniqueCount;
    public int legendaryCount;

    private DevaSkill1 devaSkill1;

    public void TestGacha()
    {
        commonCount = 0;
        uncommonCount = 0;
        rareCount = 0;
        uniqueCount = 0;
        legendaryCount = 0;

        for (int i = 0; i < 100; i++)
        {
            float r = Random.Range(0.0f, 100.1f);

            if (r >= commonProb)
            {
                GetCommon();
            }
            else if (r >= uncommonProb)
            {
                GetUnCommon();
            }
            else if (r >= rareProb)
            {
                GetRare();
            }
            else if (r >= uniqueProb)
            {
                GetUnique();
            }
            else if (r >= legendaryProb)
            {
                GetLegendary();
            }     
        }

        devaSkill1 = FindObjectOfType<DevaSkill1>();
        if (devaSkill1 != null)
        {
            devaSkill1.go_List.Clear();
        }

        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>().isSealed = false;
            }
        }
    }

    public void GetCommon()
    {
        commonCount++;
    }

    public void GetUnCommon()   
    {
        uncommonCount++;
    }

    public void GetRare()
    {
        rareCount++;
    }

    public void GetUnique()
    {
        uniqueCount++;
    }

    public void GetLegendary()
    {
        legendaryCount++;
    }
}
