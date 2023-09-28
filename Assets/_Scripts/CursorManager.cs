using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public Texture2D cursor;
    public Texture2D cursorEnemy;
    public Texture2D cursorUI;
    private Texture2D currentCursor;
    private Vector2 cursorHotspot;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }else {
            Destroy(gameObject);
            return;
        }
    }

    void Start() { 
        currentCursor = cursor;
        cursorHotspot = new Vector2(currentCursor.width / 2, currentCursor.height / 2);
        UpdateCursor();
    }

    void Update() {
        if (MySceneManager.Instance.currentScene == SceneEnum.MainMenuScene) return;

        if (EventSystem.current.IsPointerOverGameObject()) {
            CursorUIDetection(true);
        }else {
            CursorUIDetection(false);
        }
    }

    void UpdateCursor() {
        Cursor.SetCursor(currentCursor, cursorHotspot, CursorMode.Auto);
    }

    public void CursorEnemyDetection(bool onEnemy) {
        currentCursor = onEnemy ? cursorEnemy : cursor;
        UpdateCursor();
    }

    public void CursorUIDetection(bool onUI) {
        currentCursor = onUI ? cursorUI : cursor;
        UpdateCursor();
    }
}
