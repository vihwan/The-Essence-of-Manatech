using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject faderObj;
    public Image faderImg;
    public bool isGameOver = false;

    public float fadeSpeed = .02f;

    private Color fadeTransparency = new Color(0, 0, 0, .04f);
    private string currentScene;
    private AsyncOperation async;


    //제어하는 컴포넌트
    private BoardManager boardManager;
    private GUIManager guiManager;
    private ScoreManager scoreManager;

    void Awake()
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
        boardManager = FindObjectOfType<BoardManager>();
        if (boardManager != null)
            boardManager.Init();

        guiManager = FindObjectOfType<GUIManager>();
        if (guiManager != null)
            guiManager.Init();

        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
            scoreManager.Init();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }

        if (!isGameOver && guiManager.LimitTime <= 0)
        {
            isGameOver = true;
            GameOver();
        }
    }

    private void GameOver()
    {
        StartCoroutine(guiManager.WaitForShifting());
        SoundManager.instance.StopAllSE();
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE("DungeonResult");
        boardManager = null; //보드 매니저 비활성화
    }

    // Load a scene with a specified string name
    public void LoadScene(string sceneName)
    {
        instance.StartCoroutine(Load(sceneName));
        instance.StartCoroutine(FadeOut(instance.faderObj, instance.faderImg));
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
    IEnumerator FadeOut(GameObject faderObject, Image fader)
    {
        faderObject.SetActive(true);
        while (fader.color.a < 1)
        {
            fader.color += fadeTransparency;
            yield return new WaitForSeconds(fadeSpeed);
        }
        ActivateScene(); //Activate the scene when the fade ends
    }

    // Iterate the fader transparency to 0%
    IEnumerator FadeIn(GameObject faderObject, Image fader)
    {
        while (fader.color.a > 0)
        {
            fader.color -= fadeTransparency;
            yield return new WaitForSeconds(fadeSpeed);
        }
        faderObject.SetActive(false);
    }

    // Begin loading a scene with a specified string asynchronously
    IEnumerator Load(string sceneName)
    {
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        yield return async;
        isReturning = false;
    }

    // Allows the scene to change once it is loaded
    public void ActivateScene()
    {
        async.allowSceneActivation = true;
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
        // If we are running in a standalone build of the game
#if UNITY_STANDALONE
        // Quit the application
        Application.Quit();
#endif

        // If we are running in the editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private bool isReturning = false;
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