using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPC_Michelle : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().material.SetFloat("_OutlineEnabled", 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().material.SetFloat("_OutlineEnabled", 0f);
    }
}
