using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterAI : MonoBehaviour
{
    
    private MonsterStatusController monsterStatusController;
    [SerializeField] private GameObject notify;
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    public void Init()
    {
        monsterStatusController = FindObjectOfType<MonsterStatusController>();
        notify.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        UseSkill();
    }

    private void UseSkill()
    {
        if (monsterStatusController.images_Gauge[MonsterStatusController.MP].fillAmount == 1f)
        {
            notify.SetActive(true);
            monsterStatusController.DecreaseMp((int)monsterStatusController.MaxMp);
            animator.SetTrigger("active");
            //목소리
        }
    }
}