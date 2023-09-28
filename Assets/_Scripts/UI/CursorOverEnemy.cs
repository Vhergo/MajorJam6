using UnityEngine;

public class CursorOverEnemy : MonoBehaviour
{
    void OnMouseEnter() {
        CursorManager.Instance.OnPointerOverEnemy(true);
    }

    void OnMouseExit() {
        CursorManager.Instance.OnPointerOverEnemy(false);
    }
}
