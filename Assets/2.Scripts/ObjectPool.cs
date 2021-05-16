﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool Instance;

    [SerializeField] private GameObject poolingObjectPrefab;

    //폭발 이펙트들을 담는 큐
    Queue<FlashEffect> flashEffect_OP = new Queue<FlashEffect>();
    //힌트 이펙트들을 담는 큐
    Queue<FlashEffectMonster> flashEffectMon_OP = new Queue<FlashEffectMonster>();
 

    private void Awake()
    {
        Instance = this;

        Initialize(20);
    }

    private void Initialize(int count)
    {
        for (int i = 0; i < count; i++)
        {
            flashEffect_OP.Enqueue(CreateNewFlashObject());
            flashEffectMon_OP.Enqueue(CreateNewFlashMonObject());
        }
    }


    //폭발 이펙트 생성
    private FlashEffect CreateNewFlashObject()
    {
        FlashEffect newObj = Instantiate(Resources.Load<FlashEffect>("FlashEffect"), 
                                 transform.position, 
                                 Quaternion.identity)
                     .GetComponent<FlashEffect>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    private FlashEffectMonster CreateNewFlashMonObject()
    {
        FlashEffectMonster newObj = Instantiate(Resources.Load<FlashEffectMonster>("FlashEffect_Devastar"),
                                transform.position,
                                Quaternion.identity)
                    .GetComponent<FlashEffectMonster>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }



    public static FlashEffect GetFlashEffectObject(Transform t)
    {
        //오브젝트 풀에 잔여 오브젝트가 있으면
        //그대로 가져다 쓰기
        if (Instance.flashEffect_OP.Count > 0)
        {
            var obj = Instance.flashEffect_OP.Dequeue();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(t);
            obj.GetComponent<Animator>().SetTrigger("active");
            return obj;
        }
        else
        //없으면 새로 만들어서 가져다쓰기
        {
            var newObj = Instance.CreateNewFlashObject();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(t);
            newObj.GetComponent<Animator>().SetTrigger("active");
            return newObj;
        }
    }


    public static FlashEffectMonster GetFlashEffectMonObject(Transform t)
    {
        //오브젝트 풀에 잔여 오브젝트가 있으면
        //그대로 가져다 쓰기
        if (Instance.flashEffectMon_OP.Count > 0)
        {
            var obj = Instance.flashEffectMon_OP.Dequeue();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(t);
            obj.GetComponent<Animator>().SetTrigger("active");
            return obj;
        }
        else
        //없으면 새로 만들어서 가져다쓰기
        {
            var newObj = Instance.CreateNewFlashMonObject();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(t);
            newObj.GetComponent<Animator>().SetTrigger("active");
            return newObj;
        }
    }


    public static void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.flashEffect_OP.Enqueue(obj.GetComponent<FlashEffect>());
    }

    public static void ReturnObjectMon(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.flashEffectMon_OP.Enqueue(obj.GetComponent<FlashEffectMonster>());
    }
}
