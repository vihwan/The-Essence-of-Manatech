using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name; //곡의 이름
    public AudioClip clip; //곡의 소스
}

public class SoundManager : MonoBehaviour
{
    //Singleton 기법 사용
    //사운드 매니저는 반드시 '하나만' 존재해야한다.
    public static SoundManager instance;

    public AudioSource[] audioSourceEffects; //여러 효과음을 출력하는 소스들을 저장하는 배열
    public AudioSource audioSourceBGM1; //배경음악은 하나만 있어도 되니 배열이 아님
    public AudioSource audioSourceBGM2; //또다른 배경음악소스. 배경음의 변경이 자유롭게 이루어지도록 만들기 위함.

    public Sound[] effectSounds;  //효과음 클립들을 저장하는 배열
    public Sound[] bgmSounds;     //배경음 클립들을 저장하는 배열
    public Sound[] voiceSounds;   //인게임 내 보이스 소리를 저장하는 배열
    public Sound[] npcSounds;     //메인메뉴에서 npc들의 목소리를 저장하는 배열

    public string[] playSoundName; //현재 실행중인 클립의 이름들을 보관하는 배열

    private bool isPlayingPrevMusic = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            audioSourceBGM1 = this.gameObject.AddComponent<AudioSource>();
            audioSourceBGM2 = this.gameObject.AddComponent<AudioSource>();
            audioSourceBGM1.outputAudioMixerGroup = Resources.Load<AudioMixerGroup>("MasterMixer");
            audioSourceBGM2.outputAudioMixerGroup = Resources.Load<AudioMixerGroup>("MasterMixer");
            audioSourceBGM1.loop = true;
            audioSourceBGM2.loop = true;

            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];

    }

    //일단 임시로 메소드 오버로딩
    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (effectSounds[i].name == _name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(name + "사운드가 SoundManager에 등록되어있지 않습니다.");
    }

    public void PlayCV(string _name)
    {
        for (int i = 0; i < voiceSounds.Length; i++)
        {
            if (voiceSounds[i].name == _name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = voiceSounds[i].name;
                        audioSourceEffects[j].clip = voiceSounds[i].clip;

                        float clipLength = audioSourceEffects[j].clip.length;

                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(name + "사운드가 SoundManager에 등록되어있지 않습니다.");
    }

    public void PlayNPCV(string _name)
    {
        for (int i = 0; i < npcSounds.Length; i++)
        {
            if (npcSounds[i].name == _name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = npcSounds[i].name;
                        audioSourceEffects[j].clip = npcSounds[i].clip;

                        float clipLength = audioSourceEffects[j].clip.length;

                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(name + "사운드가 SoundManager에 등록되어있지 않습니다.");
    }


    //배경음 재생
    public void PlayBGM(string _name)
    {
        AudioSource audioSource = (isPlayingPrevMusic) ? audioSourceBGM1 : audioSourceBGM2;

        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (bgmSounds[i].name == _name)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = bgmSounds[i].clip;
                    audioSource.Play();
                    return;
                }
                Debug.Log("배경음 AudioSource가 이미 사용중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager Sound[] bgmSounds에 등록되어있지 않습니다.");
    }

    public void PlayBGMWithFade(string _name ,float transitionTime = 1.0f)
    {
        AudioSource activeSource = (isPlayingPrevMusic) ? audioSourceBGM1 : audioSourceBGM2;

        StartCoroutine(UpdateMusicWithFade(activeSource, _name, transitionTime));
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, string _name, float transitionTime)
    {
        if (!activeSource.isPlaying)
            activeSource.Play();

        //Fade out
        for (float t = 0.0f;  t< transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (bgmSounds[i].name == _name)
            {
                if (!activeSource.isPlaying)
                {
                    activeSource.clip = bgmSounds[i].clip;
                    activeSource.Play();
                    yield break;
                }
                Debug.Log("배경음 AudioSource가 이미 사용중입니다.");
                yield break;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager Sound[] bgmSounds에 등록되어있지 않습니다.");
    }

    public void PlayBGMWithCrossFade(string _name, float transitionTime = 1.0f)
    {
        AudioSource activeSource = (isPlayingPrevMusic) ? audioSourceBGM1 : audioSourceBGM2;
        AudioSource newSource = (isPlayingPrevMusic) ? audioSourceBGM2 : audioSourceBGM1;

        isPlayingPrevMusic = !isPlayingPrevMusic;


        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (bgmSounds[i].name == _name)
            {
                if (!newSource.isPlaying)
                {
                    newSource.clip = bgmSounds[i].clip;
                    newSource.Play();
                    break;
                }
                else
                    Debug.Log("배경음 AudioSource가 이미 사용중입니다.");
            }
        }
        
        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));
    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource ,float transitionTime)
    {
        //Fade out
        for (float t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            original.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }

        original.Stop();
    }

    //모든 효과음 재생을 멈추게 함.
    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    //지정된 하나의 효과음 재생을 멈추게 함.
    public void StopSE(string name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == name)
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
        Debug.Log("재생 중인" + name + "사운드가 없습니다.");
    }

    public void StopBGM()
    {
        audioSourceBGM1.Stop();
    }
}
