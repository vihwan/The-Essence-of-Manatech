using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPC_Ikki : NPC_Preset, IPointerExitHandler, IPointerEnterHandler
{

    private Dictionary<int, string> ambVoiceDic = new Dictionary<int, string>();
    private Dictionary<int, string> tkVoiceDic = new Dictionary<int, string>();
    private Dictionary<int, string> fwVoiceDic = new Dictionary<int, string>();

    public void Init()
    {

        for (int i = 1; i <= 3; i++)
        {
            ambVoiceDic.Add(i, "ikki_amb_0" + i);
            tkVoiceDic.Add(i, "ikki_tk_0" + i);
            fwVoiceDic.Add(i, "ikki_fw_0" + i);
        }
    }


    //일정 시간마다 혼잣말을 하는 함수
    public override void AMBVoice()
    {
        //주어진 소리는 총 3가지
        //이 중 랜덤하게 하나를 골라서 출력한다
        int randNum = Random.Range(1, 4);
        if (ambVoiceDic.TryGetValue(randNum, out string randomVoice))
        {
            SoundManager.instance.PlayNPCV(randomVoice);
        }
    }

    //클릭 했을 때의 소리를 출력하는 함수
    public override void ClickVoice()
    {
        int randNum = Random.Range(1, 4);
        if (tkVoiceDic.TryGetValue(randNum, out string randomVoice))
        {
            SoundManager.instance.PlayNPCV(randomVoice);
        }
    }

    //특수 메뉴를 끝마쳤을 때의 소리를 출력하는 함수
    public override void EndTalkVoice()
    {
        int randNum = Random.Range(1, 4);
        if (fwVoiceDic.TryGetValue(randNum, out string randomVoice))
        {
            SoundManager.instance.PlayNPCV(randomVoice);
        }
    }


    // Start is called before the first frame update
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().material.SetFloat("_OutlineEnabled", 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().material.SetFloat("_OutlineEnabled", 0f);
    }
}
