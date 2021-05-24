using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum AttackColor
{
    Fire,
    Ice,
    Light,
    Dark
}

public class MissileEffect : MonoBehaviour
{


    private AttackColor attackColor;

    private Vector3 startPos;
    private Vector3 endPos;
    public float elapsedTime = 0;
    public float targetTime = 1;
    [SerializeField] private float speed;

    [SerializeField] private Transform target;

    private bool isUpdate;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<MonsterStatusController>().gameObject.GetComponentInChildren<Image>().transform;
        image = GetComponent<Image>();
        startPos = transform.position;
        endPos = target.position;
        elapsedTime = 0;
        speed = Random.Range(0.8f, 1.3f);
        isUpdate = true;
        SetAttackColor();
    }

    private void SetAttackColor()
    {
        if (image.sprite.name == "2잭프로스트")
        {
            attackColor = AttackColor.Ice;
        }
        else if (image.sprite.name == "1플로레")
        {
            attackColor = AttackColor.Light;
        }
        else if (image.sprite.name == "3잭오랜턴")
        {
            attackColor = AttackColor.Fire;
        }
        else if (image.sprite.name == "4플루토")
        {
            attackColor = AttackColor.Dark;
        }
        else
            attackColor = AttackColor.Ice;
    }

    // Update is called once per frame
    void Update()
    {

        if (isUpdate)
        {
            elapsedTime += Time.deltaTime / speed;
            Vector3 pos = Vector3.zero;
            pos.x = MathHelper.Linear(startPos.x, endPos.x, elapsedTime);
            pos.y = MathHelper.EaseInQuart(startPos.y, endPos.y, elapsedTime);
            pos.z = 0;

            transform.position = pos;
            transform.Rotate(0, 0, 5f * elapsedTime);

            if (elapsedTime >= targetTime)
            {
                pos.x = MathHelper.Linear(startPos.x, endPos.x, 1f);
                pos.y = MathHelper.EaseInQuart(startPos.y, endPos.y, 1f);
                pos.z = 0;
                transform.position = pos;
                elapsedTime = 0;

                CreateBombEffect(attackColor, transform.position);

                //데미지 주기
                MonsterStatusController mon = FindObjectOfType<MonsterStatusController>();
                if (mon != null) { 
                    mon.DecreaseHP(4);
                    if (MonsterAI.instance.currentState == MonsterState.GROGGY)
                        mon.DecreaseHP(200f);
                }

                this.gameObject.GetComponent<Image>().enabled = false;
                isUpdate = false;
            }
        }
    }

    public void CreateBombEffect(AttackColor attackColor, Vector3 position)
    {
        GameObject bombEffect = ObjectPool.GetBombEffectObject(transform);
        bombEffect.transform.position = position;
        bombEffect.GetComponent<BombEffect>().RemoveEffect();

        switch (attackColor)
        {
            case AttackColor.Fire:
                bombEffect.GetComponent<Animator>().SetTrigger("active_fire");
                return;
            case AttackColor.Ice:
                bombEffect.GetComponent<Animator>().SetTrigger("active_ice");
                return;
            case AttackColor.Light:
                bombEffect.GetComponent<Animator>().SetTrigger("active_light");
                return;
            case AttackColor.Dark:
                bombEffect.GetComponent<Animator>().SetTrigger("active_dark");
                return;
        }
    }
}
