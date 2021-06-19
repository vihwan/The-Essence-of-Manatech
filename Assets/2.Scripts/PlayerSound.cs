using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어(마도학자)의 사운드 출력을 관리하는 컴포넌트입니다.
//임시적으로 스킬 효과음도 관리합니다. (4개밖에 되지 않으므로)
public static class PlayerSound
{
    /* <관리하는 소리 종류>
     * 타일 이동 시 사운드
     * 타일 파괴 시 사운드
     * 스킬 사용 시 사운드 
     * 스킬 이펙트 사운드
     * 마나 혹은 쿨타임 사운드
     * 승리 사운드
     * 패배 사운드
     * 
     * **/

    public static void PlayMoveTile()
    {
        int rand = Random.Range(0, 3);
        if(rand == 0)
            SoundManager.instance.PlayCV("wz_atk_01");
        else if(rand == 1)
            SoundManager.instance.PlayCV("wz_atk_02");
        else 
            SoundManager.instance.PlayCV("wz_jump_atk");
    }

    public static void PlayFailTileMatch()
    {
        SoundManager.instance.PlayEffectSound("wz_dash_fall");
    }

    public static void PlayPhaseShift()
    {
        SoundManager.instance.PlayCV("wz_phase_shift");
    }

    //타일 파괴 시 사운드
    public static void PlayDestroyFamiliarTile(CharacterKinds kind)
    {
        int rand = Random.Range(1, 6);
        SoundManager.instance.PlayEffectSound("magic_light_bubble_0" + rand);

        switch (kind)
        {
            case CharacterKinds.HammerSpanner:
                SoundManager.instance.PlayEffectSound("fcraft_hammer_04");
                break;

            case CharacterKinds.Shururu:
                {
                    rand = Random.Range(0, 2);
                    if (rand == 0)
                        SoundManager.instance.PlayEffectSound("shu_die");
                    else
                        SoundManager.instance.PlayEffectSound("shu_dmg");
                }
                break;

            case CharacterKinds.Bloom:
                SoundManager.instance.PlayEffectSound("broom_spin_sweep_03");
                break;

            case CharacterKinds.Lolipop:
                SoundManager.instance.PlayEffectSound("sweetcandybar_create");
                break;

            default:
                break;
        }
    }


    public static void PlayUseSkillVoice(SkillEffectType type)
    {
        switch (type)
        {
            case SkillEffectType.Chain:
                SoundManager.instance.PlayCV("wz_enhanced_missile");
                break;

            case SkillEffectType.Flapper:
                {
                    int rand = Random.Range(1, 4);
                    if (rand == 1)
                        SoundManager.instance.PlayCV("wz_flapper");
                    else if (rand == 2)
                        SoundManager.instance.PlayCV("wz_lollipopcrush_01");
                    else
                        SoundManager.instance.PlayCV("wz_lollipopcrush_02");
                }
                break;

            case SkillEffectType.Ice:
                SoundManager.instance.PlayCV("wz_jackfrosticewater_01");
                break;

            case SkillEffectType.Halloween:
                {
                    int rand = Random.Range(1, 3);
                    if (rand == 1)
                        SoundManager.instance.PlayCV("wz_jackohalloween_01");
                    else
                        SoundManager.instance.PlayCV("wz_jackohalloween_02");
                }
                break;
        }
    }


    public static void EffectSkillSound(SkillEffectType type)
    {
        switch (type)
        {
            case SkillEffectType.Chain:
                break;
            case SkillEffectType.Flapper:
                break;
            case SkillEffectType.Ice:
                break;
            case SkillEffectType.Halloween:
                break;
        }
    }

    //마나 부족
    public static void PlayCooldownOrLackManaVoice()
    {
        SoundManager.instance.PlayCV("wz_nomana");
    }

    public static void PlayShuffleVoice()
    {
        //플루토 리버스
        SoundManager.instance.PlayCV("wz_antigravity_starter");
    }


    public static void WinVoice()
    {

    }

    public static void DeadVoice()
    {

    }

}
