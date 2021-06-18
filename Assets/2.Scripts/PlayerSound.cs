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

    }


    //타일 파괴 시 사운드
    public static void PlayDestroyFamiliarTile(CharacterKinds kind)
    {
        

        switch (kind)
        {
            case CharacterKinds.Frost:

                break;

            case CharacterKinds.Pluto:

                break;

            case CharacterKinds.Lantern:

                break;

            case CharacterKinds.Fluore:

                break;

            case CharacterKinds.HammerSpanner:
                break;

            case CharacterKinds.Shururu:
                SoundManager.instance.PlayEffectSound("shu_die");
                break;

            case CharacterKinds.Bloom:
                break;

            case CharacterKinds.Bomb:
                break;

            case CharacterKinds.Lolipop:
                break;
        }
    }


    public static void PlayUseSkillVoice(SkillEffectType type)
    {
        switch (type)
        {
            case SkillEffectType.Chain: SoundManager.instance.PlayCV("wz_enhanced_missile"); 
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
                    if(rand == 1)
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
