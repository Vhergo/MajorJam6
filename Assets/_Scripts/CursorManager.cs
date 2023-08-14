using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public Texture2D cursor;
    public Texture2D cursorEnemy;
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

    void UpdateCursor() {
        Cursor.SetCursor(currentCursor, cursorHotspot, CursorMode.Auto);
    }

    public void CursorEnemyDetection(bool onEnemy = false) {
        currentCursor = onEnemy ? cursorEnemy : cursor;
        UpdateCursor();
    }
}
