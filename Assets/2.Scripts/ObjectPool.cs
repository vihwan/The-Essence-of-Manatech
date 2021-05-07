using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool instance;

    [SerializeField] private GameObject poolingObjectPrefab;

    //폭발 이펙트들을 담는 큐
    Queue<FlashEffect> FlashEffect_OP = new Queue<FlashEffect>();


    private void Awake()
    {
        instance = this;

        Initialize(20);
    }

    private void Initialize(int count)
    {
        for (int i = 0; i < count; i++)
        {
           // FlashEffect_OP.Enqueue(CreateNewObject());
        }
    }

}
