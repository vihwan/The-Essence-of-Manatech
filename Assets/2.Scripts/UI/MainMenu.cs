using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    private GameObject go_Mia;
    private GameObject go_Ikki;
    private GameObject go_Michelle;

    // Start is called before the first frame update
    void Start()
    {
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
    }


    private void OnClickMia()
    {
        SetPopupMenuFalse();
        go_Mia.transform.Find("PopupMenu").gameObject.SetActive(true);
    }

    private void OnClickIkki()
    {
        SetPopupMenuFalse();
        go_Ikki.transform.Find("PopupMenu").gameObject.SetActive(true);
    }

    private void OnClickMichelle()
    {
        SetPopupMenuFalse();
        go_Michelle.transform.Find("PopupMenu").gameObject.SetActive(true);
    }

    private void SetPopupMenuFalse()
    {
        go_Michelle.transform.Find("PopupMenu").gameObject.SetActive(false);
        go_Ikki.transform.Find("PopupMenu").gameObject.SetActive(false);
        go_Mia.transform.Find("PopupMenu").gameObject.SetActive(false);
    }
}
