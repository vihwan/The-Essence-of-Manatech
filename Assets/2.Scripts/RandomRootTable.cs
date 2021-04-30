using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomRootTable : MonoBehaviour
{
    public Dictionary<string, float> itemDic = new Dictionary<string, float>();

    public float total = 100f;
    public float randomNumber;

    private void Start()
    {
        itemDic.Add("sword", 10f);
        itemDic.Add("bow", 10f);
        itemDic.Add("spear", 10f);
        itemDic.Add("ingredient", 69f);
        itemDic.Add("Rare Item", 1f);

        foreach (KeyValuePair<string, float> item in itemDic)
        {
            total -= item.Value;
        }

        if(total != 0)
        {
            Debug.Log("아이템 확률의 총합이 100%가 아닙니다.");
        }

        randomNumber = Random.Range(1, 100);

        foreach (KeyValuePair<string, float> weight in itemDic)
        {
            if( randomNumber <= weight.Value)
            {
                Debug.Log("Award : " + weight.Key);
            }
            else
            {
                randomNumber -= weight.Value;
            }
        }
    }

}
