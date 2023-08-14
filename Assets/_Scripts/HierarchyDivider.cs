using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyDivider
{
    static HierarchyDivider() {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject != null && gameObject.CompareTag("HierarchyDivider")) {
            EditorGUI.DrawRect(selectionRect, Color.gray);
            EditorGUI.LabelField(selectionRect, gameObject.name);
        }
    }
}
