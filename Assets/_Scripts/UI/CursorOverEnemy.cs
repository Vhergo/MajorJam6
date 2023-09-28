using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorOverEnemy : MonoBehaviour
{
    void OnMouseEnter() {
        CursorManager.Instance.CursorEnemyDetection(true);
    }

    void OnMouseExit() {
        CursorManager.Instance.CursorEnemyDetection(false);
    }
}
