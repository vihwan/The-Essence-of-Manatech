using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum GameState{
    TITLE,
    TOWN,
    READY,
    PLAYING,
    PAUSE,
    END
}


public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;


    private string currentScene;
    public bool isGameOver = false;
    private bool isReturning = false; // 게임 메뉴로 돌아갈 때 돌아가는 중에, 다시 돌아가는 기능을 실행할 수 없도록 생성한 변수

    public float fadeSpeed = .02f;
    private Color fadeTransparency = new Color(0, 0, 0, .04f);
    public GameObject faderObj;
    public Image faderImg;

    private float resolutionScale = 1.0f;
    private const float ReferScreenWidth = 1920f;

    private AsyncOperation async;

    [SerializeField] private GameState gameState;
    [SerializeField] private PlayerState playerState;

    //제어하는 컴포넌트
    private SaveAndLoad saveAndLoad;
    private SkillData skillData;

    public GameState GameState { get => gameState; set => gameState = value; }
    public PlayerState PlayerState { get => playerState; private set => playerState = value; }

    public float ResolutionScale { 

        get => resolutionScale;
    }

    private GUIStyle guiStyle = new GUIStyle();

    public void SetResolutionScale(float resolutionWidth)
    {
       this.resolutionScale = resolutionWidth / ReferScreenWidth;
    }

    private void Awake()
    {
        // Only 1 Game Manager can exist at a time
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        GameState = GameState.READY;
        saveAndLoad = FindObjectOfType<SaveAndLoad>();
        if(saveAndLoad != null)
        {
            saveAndLoad.Init();

        }

        skillData = FindObjectOfType<SkillData>();
        if(skillData != null)
        {
            skillData.Initailize();
        }

        saveAndLoad.TestDataLoad();

        instance.SetResolutionScale(Screen.width);
    }

    private void Update()
    {
        if (GameState == GameState.PLAYING)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnToMenu();
            }

            if (!isGameOver && GUIManager.instance.LimitTime <= 0)
            {
                isGameOver = true;
                GameLose();
            }
        }
    }


    //디버그용 
    private void OnGUI()
    {
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "GameState : " + instance.gameState.ToString(), guiStyle);
        if(SceneManager.GetActiveScene().name == "InGameScene")
        {
            if (BoardManager.instance == null || MonsterAI.instance == null)
                return;

            GUI.Label(new Rect(10, 40, 100, 20), "PlayerState : " + BoardManager.instance.currentState.ToString(), guiStyle);
            GUI.Label(new Rect(10, 70, 100, 20), "MonsterState : " + MonsterAI.instance.Action.ToString(), guiStyle);
            GUI.Label(new Rect(10, 100, 100, 20), "IsControlTileState : " + BoardManager.instance.IsCanControlTile.ToString(), guiStyle);
        }     
    }

    internal void GameWin()
    {
        GameState = GameState.END;
        PlayerState = PlayerState.WIN;
        StartCoroutine(ExternalFuncManager.Instance.WaitForShifting());
    }

    internal void GameLose()
    {
        GameState = GameState.END;
        PlayerState = PlayerState.LOSE;
        StartCoroutine(ExternalFuncManager.Instance.WaitForShifting());;
    }

    // Load a scene with a specified string name
    public void LoadScene(string sceneName)
    {
        StopAllCoroutines();
        instance.StartCoroutine(FadeOut(instance.faderObj, instance.faderImg, sceneName));
        //instance.StartCoroutine(Load(sceneName)); Obsolete
    }

    // Reload the current scene
    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        instance.StartCoroutine(FadeIn(instance.faderObj, instance.faderImg));
    }

    //Iterate the fader transparency to 100%
    private IEnumerator FadeOut(GameObject faderObject, Image fader, string sceneName)
    {
        faderObject.SetActive(true);
        while (fader.color.a < 1)
        {
            fader.color += fadeTransparency;
            yield return new WaitForSeconds(fadeSpeed);
        }
        LoadingSceneManager.SetLoadScene(sceneName);
    }

    // Iterate the fader transparency to 0%
    private IEnumerator FadeIn(GameObject faderObject, Image fader)
    {
        while (fader.color.a > 0)
        {
            fader.color -= fadeTransparency;
            yield return new WaitForSeconds(fadeSpeed);
        }
        faderObject.SetActive(false);
    }


    [System.Obsolete("This is an obsolete method. LoadingSceneManager Class의 SetLoadScene을 사용하세요.")]
    private IEnumerator Load(string sceneName)
    {
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        yield return async;
        isReturning = false;
    }

    // Get the current scene name
    public string CurrentSceneName
    {
        get
        {
            return currentScene;
        }
    }


    public void ExitGame()
    {
        // 빌드된 게임이라면
#if UNITY_STANDALONE
        // Quit the application
        Application.Quit();
#endif

        // 에디터에서 실행중인 것이라면
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void ReturnToMenu()
    {
        if (isReturning)
        {
            return;
        }

        if (CurrentSceneName != "MenuScene")
        {
            StopAllCoroutines();
            LoadScene("MenuScene");
            isReturning = true;
        }
    }
}