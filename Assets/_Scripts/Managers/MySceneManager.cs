using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private float loadingScreenDuration = 2f;
    private bool isPaused;

    [Header("Scene Input")]
    [SerializeField] private KeyCode pauseKey = KeyCode.O;
    [SerializeField] private KeyCode restartKey = KeyCode.P;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(loadingScreen);
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        HandleSceneInput();
    }

    public void SwitchScene(SceneEnum scene, bool withLoadingScreen = false) {
        if (withLoadingScreen) {
            StartCoroutine(LoadSceneWithLoadingScreen(scene));
        } else {
            SceneManager.LoadScene(scene.ToString());
        }
    }

    private IEnumerator LoadSceneWithLoadingScreen(SceneEnum scene) {
        Time.timeScale = 0;
        loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.ToString());
        yield return new WaitForSecondsRealtime(loadingScreenDuration);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        loadingScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGameToggle() {
        if (SceneManager.GetActiveScene().name != "GameScene") return;

        if (isPaused) {
            Time.timeScale = 1;
            GameUIManager.Instance.TurnOffSettings();
            isPaused = false;
        }else {
            Time.timeScale = 0;
            GameUIManager.Instance.TurnOnSettings();
            isPaused = true;
        }
    }

    public void QuitGame() {
        Debug.Log("Quit Game");
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void HandleSceneInput() {
        if (Input.GetKeyDown(restartKey)) {
            RestartScene();
        }

        if (Input.GetKeyDown(pauseKey) || Input.GetKeyDown(KeyCode.Escape)) {
            PauseGameToggle();
        }
    }

    public bool LoadingScreenIsNotActive() {
        return !loadingScreen.activeSelf;
    }
}

public enum SceneEnum
{
    MainMenuScene,
    GameScene
}
