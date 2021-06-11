using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum DevastarState
{
    Meet, //3
    Skill_One, //1
    Skill_One_Berserk, //1
    HumanDead,
    Transform, //2
    Skill_Two, //3
    Skill_Two_Next,//3
    Skill_Two_Final,//1
    Skill_Three, //3
    Skill_Three_Berserk, // 2
    Skill_Three_ChaosFusion,//1
    Skill_Three_Final,//2
    GroggyHuman, // human 1 
    GroggyDevil, // devil 3
    Victory, // human 2
    Dead // 1
}

//데바스타르의 목소리 출력을 설정하고, 그에 따라 Notify의 텍스트가 달라지는 함수를 처리하는 스크립트입니다.

/* 데바스타르 목소리
 * 
 * - 조우 시 (3개) 
- 1스킬 (1개) 

- 1스킬 광폭화 (1개)

- 변신 (2개)

- 2스킬 (3개) / 2스킬 진행이후 (3개) / 마무리 공격 (1개)

- 3스킬 (3개)

- 3스킬 광폭화 (2개) / 마무리(1개) / 웃음(2개)

- 그로기 (인간 1개, 악마 3개)

- 승리 (인간 2개)3

- 사망 (1개)

- 경직 (인간 1개, 악마 3개)
 * 
 * **/

public class SetDevastarSoundandNotify : MonoBehaviour
{
    private MonsterNotify notify;

    private Dictionary<int, string> meetVoiceDic = new Dictionary<int, string>();
    private Dictionary<int, string> transformDic = new Dictionary<int, string>();
    private Dictionary<int, string> skill2Dic = new Dictionary<int, string>();
    private Dictionary<int, string> skill3Dic = new Dictionary<int, string>();
    private Dictionary<int, string> skill3_BerserkDic = new Dictionary<int, string>();
/*    private Dictionary<int, string> devilGroggyDic = new Dictionary<int, string>();
    private Dictionary<int, string> victoryDic = new Dictionary<int, string>();*/



    public void Initailze()
    {
        notify = GameObject.Find("StageManager/GUIManagerCanvas/MonsterUI/Notify").GetComponent<MonsterNotify>();
        if (notify != null)
        {
            notify.init();
            notify.gameObject.SetActive(false);
        }

        DictionarySetting();
    }

    private void DictionarySetting()
    {

        for (int i = 1; i <= 3; i++)
        {
            meetVoiceDic.Add(i, "devastar_meet_0" + i);
        }

        for (int i = 1; i <= 2; i++)
        {
            transformDic.Add(i, "");
        }

        for (int i = 1; i <= 3; i++)
        {
            skill2Dic.Add(i, "devastar_devil_skill_08_1_" + i);
        }

        for (int i = 1; i <= 3; i++)
        {
            skill3Dic.Add(i, "devastar_devil_skill_09_1_" + i);
        }

        for (int i = 1; i <= 2; i++)
        {
            skill3_BerserkDic.Add(i, "devastar_devil_skill_09_3_" + i);
        }
    }


    //Notify는 몬스터 컷신
    internal void SetVoiceAndNotify(DevastarState state)
    {
        notify.gameObject.SetActive(true);
        int randNum = Random.Range(1, 4);

        switch (state)
        {
            case DevastarState.Meet:
                {
                    randNum = Random.Range(1, 4);
                    if (meetVoiceDic.TryGetValue(randNum, out string randomVoice))
                    {
                        SoundManager.instance.PlayMonV(randomVoice);
                    }
                }
                break;

            case DevastarState.Skill_One:
                {
                    SoundManager.instance.PlayMonV("devastar_Human_Skill");
                    notify.SetText("서로를 옭아매는 어리석은 인간들이여");
                    notify.PlayAnim();
                }
                break;

            case DevastarState.Skill_One_Berserk:
                {
                    SoundManager.instance.PlayMonV("devastar_skill_08_3");
                    notify.SetText("파멸하라!");
                    notify.PlayAnim();
                }
                break;

            case DevastarState.HumanDead:
                {
                    SoundManager.instance.PlayMonV("devastar_skill_02_3");
                    notify.SetText("크윽.. 방해하는 자에게 고통을!!");
                    notify.PlayAnim();
                }
                break;

            case DevastarState.Transform:
                {
                    SoundManager.instance.PlayMonV("devastar_skill_02_5");
                    notify.NotifyImage.sprite = Resources.Load<Sprite>("notify2");
                    notify.SetText("진정한 혼돈의 힘을 보여주마!!!");
                    notify.PlayAnim();
                }
                break;

            case DevastarState.Skill_Two: //3개
                {
                    randNum = Random.Range(1, 4);
                    if (skill2Dic.TryGetValue(randNum, out string randomVoice))
                    {
                        SoundManager.instance.PlayMonV(randomVoice);
                        if (randNum == 1)
                        {
                            notify.SetText("그 분의 뜻을 받들어");
                        }
                        else if (randNum == 2)
                        {
                            notify.SetText("그 분을 대신하여");
                        }
                        else if (randNum == 3)
                        {
                            notify.SetText("그 분의 의지대로");
                        }
                        else
                            return;

                        notify.PlayAnim();
                    }
                }
                break;

            case DevastarState.Skill_Two_Next:
                {
                    if (randNum == 1)
                    {
                        SoundManager.instance.PlayMonV("devastar_devil_skill_08_2_1");                       
                        notify.SetText("너희들을 심판한다!");
                    }
                    else if (randNum == 2)
                    {
                        SoundManager.instance.PlayMonV("devastar_devil_skill_08_2_2");
                        notify.SetText("파멸을 선사하마!");
                    }
                    else if (randNum == 3)
                    {
                        SoundManager.instance.PlayMonV("devastar_devil_skill_08_2_3");
                        notify.SetText("그릇된 정의를 부순다!");
                    }
                    else
                        return;

                    notify.PlayAnim();
                }
                break;

            case DevastarState.Skill_Two_Final:
                {
                    //컨빅션!!
                    SoundManager.instance.PlayMonV("devastar_devil_skill_08_3");
                }
                break;

            case DevastarState.Skill_Three: // 3개
                {
                    if (skill3Dic.TryGetValue(randNum, out string randomVoice))
                    {            
                        SoundManager.instance.PlayMonV(randomVoice);
                        if (randNum == 1)
                        {
                            notify.SetText("혼돈의 힘은 무한하다!");
                        }
                        else if (randNum == 2)
                        {
                            notify.SetText("묵시록의 빛이여!");
                        }
                        else if (randNum == 3)
                        {
                            notify.SetText("거짓된 영웅들이여!");
                        }
                        else
                            return;

                        notify.PlayAnim();
                    }
                }
                break;

            case DevastarState.Skill_Three_Berserk: //2개
                {
                    randNum = Random.Range(1, 3);
                    if (skill3_BerserkDic.TryGetValue(randNum, out string randomVoice))
                    {
                        SoundManager.instance.PlayMonV(randomVoice);
                        if (randNum == 1)
                        {
                            notify.SetText("혼돈속에 처박혀라!!!");
                        }
                        else if (randNum == 2)
                        {
                            notify.SetText("오만한 정의에 파멸을!!!");
                        }   
                        else
                            return;

                        notify.PlayAnim();
                    }
                }
                break;

            case DevastarState.Skill_Three_ChaosFusion:
                {
                    //카오스 퓨전!
                    SoundManager.instance.PlayMonV("devastar_devil_skill_09_4");
                }
                break;

            case DevastarState.Skill_Three_Final:
                {
                    //웃음소리
                    SoundManager.instance.PlayMonV("devastar_devil_skill_09_5_2");
                }
                break;

            case DevastarState.GroggyHuman:
                {
                    SoundManager.instance.PlayMonV("devastar_groggy");
                    notify.SetText("크윽...!");
                    notify.PlayAnim();
                }
                break;

            case DevastarState.GroggyDevil://2개
                {
                    randNum = Random.Range(1, 3);
                    if (randNum == 1)
                    {
                        SoundManager.instance.PlayMonV("devastar_devil_skill_09_2");
                        notify.SetText("이럴 수가..!!");
                    }
                    else if (randNum == 2)
                    {
                        SoundManager.instance.PlayMonV("devastar_devil_groggy_01");
                        notify.SetText("내가.. 수세에 몰리다니..!");
                    }
                    else
                        return;

                    notify.PlayAnim();
                }
                break;

            case DevastarState.Victory:
                {
                    SoundManager.instance.PlayMonV("devastar_skill_01_2");
                }
                break;

            case DevastarState.Dead:
                {
                    SoundManager.instance.PlayMonV("devastar_devil_die_01");
                }
                break;

            default:
                break;
        }
    }

}
