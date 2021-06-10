using UnityEngine;
using UnityEngine.UI;

public class ConfirmMenu : MonoBehaviour
{
    private Button confirmBtn;
    private Button closeBtn;

    // Start is called before the first frame update
    public void Init()
    {
        confirmBtn = transform.Find("confirmBtn").GetComponent<Button>();
        if (confirmBtn != null)
        {
            confirmBtn.onClick.AddListener(OnClickStart);
        }

        closeBtn = transform.Find("closeBtn").GetComponent<Button>();
        if(closeBtn != null)
        {
            closeBtn.onClick.AddListener(OnClickClose);
        }
    }

    private void OnClickStart()
    {
        this.gameObject.SetActive(false);
        MainMenu.instance.StartGame();
    }

    private void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }
}
