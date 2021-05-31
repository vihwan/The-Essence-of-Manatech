using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IPointerClickHandler
{
    // Canvas / Panel에 있는 스크립트
    public static MainMenu instance;

    private GameObject go_Mia;
    private GameObject go_Ikki;
    private GameObject go_Michelle;

    private PopupMenu mia_Popup;
    private PopupMenu ikki_Popup;
    private PopupMenu michelle_Popup;

    private ConfirmMenu confirm_Menu;

    private Animator startAni;

    public ConfirmMenu Confirm_Menu { get => confirm_Menu;}

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        go_Mia = GetComponentInChildren<NPC_DruidMia>().gameObject;
        if (go_Mia != null)
        {
            go_Mia.GetComponent<Button>().onClick.AddListener(OnClickMia);
        }

        go_Ikki = GetComponentInChildren<NPC_Ikki>().gameObject;
        if (go_Ikki != null)
        {
            go_Ikki.GetComponent<Button>().onClick.AddListener(OnClickIkki);
        }

        go_Michelle = GetComponentInChildren<NPC_Michelle>().gameObject;
        if (go_Michelle != null)
        {
            go_Michelle.GetComponent<Button>().onClick.AddListener(OnClickMichelle);
        }

        mia_Popup = go_Mia.transform.Find("PopupMenu").GetComponent<PopupMenu>();
        if(mia_Popup != null)
        {
            mia_Popup.Init();
        }

        ikki_Popup = go_Ikki.transform.Find("PopupMenu").GetComponent<PopupMenu>();
        if (ikki_Popup != null)
        {
            ikki_Popup.Init();
        }

        michelle_Popup = go_Michelle.transform.Find("PopupMenu").GetComponent<PopupMenu>();
        if (michelle_Popup != null)
        {
            michelle_Popup.Init();
        }

        startAni = transform.Find("StartUI").GetComponent<Animator>();

        confirm_Menu = transform.Find("Confirm_Popup").GetComponent<ConfirmMenu>();
        if(Confirm_Menu != null)
        {
            confirm_Menu.Init();
        }
    }


    private void OnClickMia()
    {
        SetPopupMenuFalse();
        go_Mia.GetComponent<Button>().interactable = false;
        go_Mia.transform.Find("PopupMenu").gameObject.SetActive(true);
    }

    private void OnClickIkki()
    {
        SetPopupMenuFalse();
        go_Ikki.GetComponent<Button>().interactable = false;
        go_Ikki.transform.Find("PopupMenu").gameObject.SetActive(true);
    }

    private void OnClickMichelle()
    {
        SetPopupMenuFalse();
        go_Michelle.GetComponent<Button>().interactable = false;
        go_Michelle.transform.Find("PopupMenu").gameObject.SetActive(true);
    }

    internal void SetPopupMenuFalse()
    {
        go_Mia.transform.Find("PopupMenu").gameObject.SetActive(false);
        go_Michelle.transform.Find("PopupMenu").gameObject.SetActive(false);
        go_Ikki.transform.Find("PopupMenu").gameObject.SetActive(false);

        go_Mia.GetComponent<Button>().interactable = true;
        go_Michelle.GetComponent<Button>().interactable = true;
        go_Ikki.GetComponent<Button>().interactable = true;
    }


    //Debug 용 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        print(eventData.pointerCurrentRaycast.gameObject);
    }

    public void startGame()
    {
        print("게임 시작 카운트다운");
        startAni.gameObject.SetActive(true);
    }

    public void LoadingForStartGame()
    {
        LoadingSceneManager.SetLoadScene("InGameScene"); //로딩 씬을 실행
    }
}
