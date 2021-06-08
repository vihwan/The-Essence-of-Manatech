using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum GameState{
    BEGIN,
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

    private AsyncOperation async;

    [SerializeField] private GameState gameState;

    //제어하는 컴포넌트
    private SaveAndLoad saveAndLoad;
    private SkillData skillData;

    public GameState GameState { get => gameState; set => gameState = value; }


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

        GameState = GameState.BEGIN;
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
                GameOver();
            }
        }
    }

    internal void GameOver()
    {
        GameState = GameState.END;
        StartCoroutine(GUIManager.instance.WaitForShifting());
        SoundManager.instance.StopAllSE();
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE("DungeonResult");
        BoardManager.instance = null; //보드 매니저 비활성화
        BoardManagerMonster.instance = null;
    }

    // Load a scene with a specified string name
    public void LoadScene(string sceneName)
    {
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

        if (CurrentSceneName != "Menu")
        {
            StopAllCoroutines();
            LoadScene("Menu");
            isReturning = true;
        }
    }
}