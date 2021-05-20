using UnityEngine;


/*//효과음 Enum
public enum Clip { 
    Select, 
    Swap, 
    Clear 
};*/

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
    public AudioSource audioSourceBGM; //배경음악은 하나만 있어도 되니 배열이 아님
    public Sound[] effectSounds;  //효과음 클립들을 저장하는 배열
    public Sound[] bgmSounds;     //배경음 클립들을 저장하는 배열
    public Sound[] voiceSounds;

    public string[] playSoundName; //현재 실행중인 클립의 이름들을 보관하는 배열


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }

    /*    public void PlaySE(Clip audioClip)
        {
            audioSourceEffect[(int)audioClip].Play();
        }*/

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
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (bgmSounds[i].name == _name)
            {
                if (!audioSourceBGM.isPlaying)
                {
                    audioSourceBGM.clip = bgmSounds[i].clip;
                    audioSourceBGM.Play();
                    return;
                }
                Debug.Log("배경음 AudioSource가 이미 사용중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager Sound[] bgmSounds에 등록되어있지 않습니다.");
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
        audioSourceBGM.Stop();
    }
}
