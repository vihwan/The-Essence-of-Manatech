using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    private Button talk_Btn;
    private Button speci_Btn;


    // Start is called before the first frame update
    public void Init()
    {
        talk_Btn = transform.Find("talkBtn").GetComponent<Button>();
        if(talk_Btn != null)
        {
            talk_Btn.onClick.AddListener(OnClickTalk);
        }

        speci_Btn = transform.Find("speBtn").GetComponent<Button>();
        if(speci_Btn != null)
        {
            speci_Btn.onClick.AddListener(OnClickSpecific);
        }
    }

    private void OnClickTalk()
    {
        MainMenu.instance.SetPopupMenuFalse();

        if (this.gameObject.transform.parent.name == "NPC_Druidmia")
        {
            //미아와 대화
            print("미아와 대화");
        }
        else if (this.gameObject.transform.parent.name == "NPC_Ikki")
        {
            //이키와 대화
            print("이키와 대화");
        }
        else if (this.gameObject.transform.parent.name == "NPC_Michelle")
        {
            //미쉘과 대화
            print("미쉘과 대화");
        }
    }


    private void OnClickSpecific()
    {
        MainMenu.instance.SetPopupMenuFalse();

        if (this.gameObject.transform.parent.name == "NPC_Druidmia")
        {
            //미아와 대화
            print("환경 설정");
        }
        else if (this.gameObject.transform.parent.name == "NPC_Ikki")
        {
            //이키와 대화
            print("스킬 설정");
        }
        else if (this.gameObject.transform.parent.name == "NPC_Michelle")
        {
            //미쉘과 대화
            MainMenu.instance.SetPopupMenuFalse();
            MainMenu.instance.Confirm_Menu.gameObject.SetActive(true);
        }

    }

}
