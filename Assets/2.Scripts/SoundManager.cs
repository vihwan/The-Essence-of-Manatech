using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

/*[System.Serializable]
public class Sound
{
    public string name; //곡의 이름
    public AudioClip clip; //곡의 소스
}*/

//딕셔너리를 직렬화시켜, 에디터에서 딕셔너리를 확인하거나 수정할 수 있습니다.
[System.Serializable]
public class ClipDictionary : SerializeDictionary<string, AudioClip> { }
//Param => string : 곡 이름, AudioClip : 곡 소스

public class SoundManager : MonoBehaviour
{
    //Singleton 기법 사용
    //사운드 매니저는 반드시 '하나만' 존재해야한다.
    public static SoundManager instance;

    public AudioSource[] audioSourceEffects; //여러 효과음을 출력하는 소스들을 저장하는 배열
    public AudioSource audioSourceBGM1; //배경음악은 하나만 있어도 되니 배열이 아님
    public AudioSource audioSourceBGM2; //또다른 배경음악소스. 배경음의 변경이 자유롭게 이루어지도록 만들기 위함.

    #region Deprecate variables
    /*    public Sound[] effectSounds;  //효과음 클립들을 저장하는 배열
        public Sound[] bgmSounds;     //배경음 클립들을 저장하는 배열
        public Sound[] monsterSounds;   //인게임 내 몬스터 보이스 소리를 저장하는 배열
        public Sound[] characterSounds; // 인게임 내 캐릭터 소리를 저장하는 배열
        public Sound[] npcSounds;     //메인메뉴에서 npc들의 목소리를 저장하는 배열*/
    #endregion

    public string[] playSoundName; //현재 실행중인 클립의 이름들을 보관하는 배열

    [SerializeField]
    private ClipDictionary clipDic = new ClipDictionary();

    private bool isPlayingPrevMusic = false; //Fade를 위한 전역변수. 이전에 재생되고 있는 음악이 있는지를 구별하기 위한 변수


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

    private void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }

    private AudioClip GetPoolingAudioClip(string _folderName, string _name)
    {

        //이미 사운드 배열 안에 클립이 저장되어 있다면 그 클립을 리턴
        if (clipDic.ContainsKey(_name))
        {
            return this.clipDic[_name];
        }
        else //없으면 Resources 폴더에서 새로 파일을 얻어온다.
        {

            AudioClip clip = Resources.Load<AudioClip>(Path.Combine("Audio", _folderName, _name));
            if (clip != null)
            {
                //딕셔너리[이름, 소스] 에 저장하고 소스를 반환한다.
                this.clipDic[_name] = clip;
                return clip;
            }
            else
            {
                Debug.LogWarning("존재하지 않는 오디오 로드 경로입니다.");
                return null;
            }
        }
    }

    //효과음을 재생
    public void PlaySE(string _name)
    {
        AudioClip audioClip = GetPoolingAudioClip("SFX", _name);
        if (audioClip != null)
        {
            for (int i = 0; i < audioSourceEffects.Length; i++)
            {
                if (!audioSourceEffects[i].isPlaying)
                {
                    playSoundName[i] = _name;
                    audioSourceEffects[i].clip = audioClip;
                    audioSourceEffects[i].Play();
                    return;
                }
            }
            Debug.Log("모든 가용 AudioSource가 사용중입니다.");
            return;
        }
        else
        {
            Debug.Log(_name + "사운드가 SoundManager에 등록되어있지 않습니다.");
        }
    }

    //몬스터 보이스 재생
    public void PlayMonV(string _name)
    {
        AudioClip audioClip = GetPoolingAudioClip("Monster/Devastar", _name);
        if (audioClip != null)
        {
            for (int i = 0; i < audioSourceEffects.Length; i++)
            {
                if (!audioSourceEffects[i].isPlaying)
                {
                    playSoundName[i] = _name;
                    audioSourceEffects[i].clip = audioClip;
                    audioSourceEffects[i].Play();
                    return;
                }
            }
            Debug.Log("모든 가용 AudioSource가 사용중입니다.");
            return;
        }
        else
        {
            Debug.Log(_name + "사운드가 SoundManager에 등록되어있지 않습니다.");
        }
    }

    //플레이어(캐릭터) 보이스 재생
    public void PlayCV(string _name)
    {
        AudioClip audioClip = GetPoolingAudioClip("CharacterVoice", _name);
        if (audioClip != null)
        {
            for (int i = 0; i < audioSourceEffects.Length; i++)
            {
                if (!audioSourceEffects[i].isPlaying)
                {
                    playSoundName[i] = _name;
                    audioSourceEffects[i].clip = audioClip;
                    audioSourceEffects[i].Play();
                    return;
                }
            }
            Debug.Log("모든 가용 AudioSource가 사용중입니다.");
            return;
        }
        else
        {
            Debug.Log(_name + "사운드가 SoundManager에 등록되어있지 않습니다.");
        }
    }

    //NPC 보이스 재생
    public void PlayNPCV(string _name)
    {
        AudioClip audioClip = GetPoolingAudioClip("NPCVoice", _name);
        if (audioClip != null)
        {
            for (int i = 0; i < audioSourceEffects.Length; i++)
            {
                if (!audioSourceEffects[i].isPlaying)
                {
                    playSoundName[i] = _name;
                    audioSourceEffects[i].clip = audioClip;
                    audioSourceEffects[i].Play();
                    return;
                }
            }
            Debug.Log("모든 가용 AudioSource가 사용중입니다.");
            return;
        }
        else
        {
            Debug.Log(_name + "사운드가 SoundManager에 등록되어있지 않습니다.");
        }
    }
    //배경음 재생
    public void PlayBGM(string _name)
    {
        AudioSource audioSource = (isPlayingPrevMusic) ? audioSourceBGM1 : audioSourceBGM2;
        AudioClip audioClip = GetPoolingAudioClip("Music", _name);
        if (audioClip != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
                return;
            }
            else
            {
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        else
        {
            Debug.Log(_name + "사운드가 SoundManager에 등록되어있지 않습니다.");
        }
    }

    public void PlayBGMWithFade(string _name, float transitionTime = 1.0f)
    {
        AudioSource activeSource = (isPlayingPrevMusic) ? audioSourceBGM1 : audioSourceBGM2;

        StartCoroutine(UpdateMusicWithFade(activeSource, _name, transitionTime));
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, string _name, float transitionTime)
    {
        if (!activeSource.isPlaying)
            activeSource.Play();

        //Fade out
        for (float t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();

        AudioClip clip = GetPoolingAudioClip("Music", _name);

        if (clip != null)
        {
            if (!activeSource.isPlaying)
            {
                activeSource.clip = clip;
                activeSource.Play();
                yield break;
            }
            Debug.Log("배경음 AudioSource가 이미 사용중입니다.");
            yield break;
        }
        Debug.Log(_name + "사운드가 SoundManager Sound[] bgmSounds에 등록되어있지 않습니다.");
    }

    public void PlayBGMWithCrossFade(string _name, float transitionTime = 1.0f)
    {
        AudioSource activeSource = (isPlayingPrevMusic) ? audioSourceBGM1 : audioSourceBGM2;
        AudioSource newSource = (isPlayingPrevMusic) ? audioSourceBGM2 : audioSourceBGM1;
        AudioClip audioClip = GetPoolingAudioClip("Music", _name);

        isPlayingPrevMusic = !isPlayingPrevMusic;

        if (audioClip != null)
        {
            if (!newSource.isPlaying)
            {
                newSource.clip = audioClip;
                newSource.Play();
            }
            else
            {
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }

        }
        else
        {
            Debug.Log(_name + "사운드가 SoundManager에 등록되어있지 않습니다.");
        }

        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));
    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
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
        audioSourceBGM2.Stop();
    }
}
