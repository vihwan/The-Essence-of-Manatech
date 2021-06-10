using UnityEngine;
using UnityEngine.UI;

public class GameQuitMenu : MonoBehaviour
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
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(OnClickClose);
        }
    }

    private void OnClickStart()
    {
        this.gameObject.SetActive(false);
#if UNITY_EDITOR
        //만약 사용자가 에디터 모드라면 재생 버튼을 꺼준다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
            //빌드된 파일이라면 (기기에서 실행되고 있는 상태라면)
            Application.Quit(); //실행 종료
#endif
        UISound.ClickButton();
    }

    private void OnClickClose()
    {
        this.gameObject.SetActive(false);
        UISound.ClickButton();
    }
}
