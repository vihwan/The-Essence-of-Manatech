using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool Instance;
    [SerializeField] private GameObject poolingObjectPrefab;

    //폭발 이펙트들을 담는 큐
  //  Queue<GameObject> flashEffect_OP = new Queue<GameObject>();
    //힌트 이펙트들을 담는 큐
    Queue<GameObject> flashEffectMon_OP = new Queue<GameObject>();
    Queue<GameObject> bombEffects_OP = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        Initialize(20, 10);
    }

    private void Initialize(int count,int count2)
    {
        for (int i = 0; i < count; i++)
        {
            //flashEffect_OP.Enqueue(CreateNewObject<FlashEffectParticle>("FlashEffect"));
            flashEffectMon_OP.Enqueue(CreateNewObject<FlashEffectMonster>("FlashEffect_Devastar"));
        }
        for (int i = 0; i < count2; i++)
        {
            bombEffects_OP.Enqueue(CreateNewObject<BombEffect>("BombEffect"));
        }
    }


    private GameObject CreateNewObject<T>(string objectPath) where T : class
    {
        GameObject newobj = Instantiate(Resources.Load<GameObject>(objectPath.ToString()),
                                 transform.position,
                                 Quaternion.identity) as GameObject;
        newobj.SetActive(false);
        newobj.transform.SetParent(transform); 
        return newobj;
    }


    public static GameObject GetObjectPoolEffect<T>(Transform t, string objectPath) where T : class
    {
        Type classType = typeof(T);
        Queue<GameObject> tempQueue;

   /*     if (classType == typeof(FlashEffect))
        {
            tempQueue = Instance.flashEffect_OP;
        }
        else */
        if (classType == typeof(FlashEffectMonster))
        {
            tempQueue = Instance.flashEffectMon_OP;
        }
        else
            return null;


        if (tempQueue.Count > 0)
        {
            var obj = tempQueue.Dequeue();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(t);
            obj.GetComponent<Animator>().SetTrigger("active");
            return obj;
        }
        else
        //없으면 새로 만들어서 가져다쓰기
        {
            var newObj = Instance.CreateNewObject<T>(objectPath);
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(t);
            newObj.GetComponent<Animator>().SetTrigger("active");
            return newObj;
        }
    }



    //public static GameObject GetFlashEffectObject(Transform t)
    //{
    //    //오브젝트 풀에 잔여 오브젝트가 있으면
    //    //그대로 가져다 쓰기
    //    if (Instance.flashEffect_OP.Count > 0)
    //    {
    //        var obj = Instance.flashEffect_OP.Dequeue();
    //        obj.gameObject.SetActive(true);
    //        obj.transform.SetParent(t);
    //        obj.GetComponent<Animator>().SetTrigger("active");
    //        return obj;
    //    }
    //    else
    //    //없으면 새로 만들어서 가져다쓰기
    //    {
    //        var newObj = Instance.CreateNewObject<FlashEffect>("FlashEffect");
    //        newObj.gameObject.SetActive(true);
    //        newObj.transform.SetParent(t);
    //        newObj.GetComponent<Animator>().SetTrigger("active");
    //        return newObj;
    //    }
    //}


    //public static GameObject GetFlashEffectMonObject(Transform t)
    //{
    //    //오브젝트 풀에 잔여 오브젝트가 있으면
    //    //그대로 가져다 쓰기
    //    if (Instance.flashEffectMon_OP.Count > 0)
    //    {
    //        var obj = Instance.flashEffectMon_OP.Dequeue();
    //        obj.gameObject.SetActive(true);
    //        obj.transform.SetParent(t);
    //        obj.GetComponent<Animator>().SetTrigger("active");
    //        return obj;
    //    }
    //    else
    //    //없으면 새로 만들어서 가져다쓰기
    //    {
    //        var newObj = Instance.CreateNewObject<FlashEffectMonster>("FlashEffectMonster");
    //        newObj.gameObject.SetActive(true);
    //        newObj.transform.SetParent(t);
    //        newObj.GetComponent<Animator>().SetTrigger("active");
    //        return newObj;
    //    }
    //}

    public static GameObject GetBombEffectObject(Transform t)
    {
        //오브젝트 풀에 잔여 오브젝트가 있으면
        //그대로 가져다 쓰기
        if (Instance.bombEffects_OP.Count > 0)
        {
            var obj = Instance.bombEffects_OP.Dequeue();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(t);
            return obj;
        }
        else
        //없으면 새로 만들어서 가져다쓰기
        {
            var newObj = Instance.CreateNewObject<BombEffect>("BombEffect");
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(t);
            return newObj;
        }
    }



    public static void ReturnObjectToPool<T>(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);

        Type classType = typeof(T);

/*        if(classType == typeof(FlashEffect))
            Instance.flashEffect_OP.Enqueue(obj);
        else*/ 
        if(classType == typeof(FlashEffectMonster))
            Instance.flashEffectMon_OP.Enqueue(obj);
        else if(classType == typeof(BombEffect))
            Instance.bombEffects_OP.Enqueue(obj);
    }
    
}
