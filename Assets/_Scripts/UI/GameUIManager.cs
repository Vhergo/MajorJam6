using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button exitToMenuButton;

    private void Start() {
        if (exitToMenuButton != null) exitToMenuButton.onClick.AddListener(OnExitToMenuButtonClick);
    }

    public void TurnOnSettings() {
        settingsPanel.SetActive(true);
    }

    public void TurnOffSettings() {
        settingsPanel.SetActive(false);
    }

    public void GameSceneUnpause() {
        MySceneManager.Instance.UnpauseGame();
    }

    private void OnExitToMenuButtonClick() {
        MySceneManager.Instance.SwitchScene(SceneEnum.MainMenuScene);
    }

}
