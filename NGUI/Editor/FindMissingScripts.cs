#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// MissingScripts 가 있는 오브젝트들 찾거나 제거하기 위한 기능
public class FindMissingScripts : EditorWindow
{
    private static int prefabCount = 0;
    private static int go_count = 0;
    private static int components_count = 0;
    private static int missing_count = 0;
    
    [MenuItem("NGUI Extension/Check/FindMissingScripts")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScripts));
    }
 
    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected prefabs"))
        {
            FindInSelected();
        }
        
        if (GUILayout.Button("Remove Missing Scripts in selected prefabs"))
        {
            RemoveMissingInSelected();
        }
    }

    private void RemoveMissingInSelected()
    {
        var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
        int compCount = 0;
        int goCount = 0;
        foreach (var o in deepSelection)
        {
            if (o is GameObject go)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (count > 0)
                {
                    Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    compCount += count;
                    goCount++;
                }
                Debug.Log( string.Format("Remove {0} missing scripts.", count));
            }
        }
    }

    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        prefabCount = 0;
        go_count = 0;
        components_count = 0;
        missing_count = 0;


        foreach (GameObject g in go)
        {
            prefabCount++;
            FindInChild(g);
        }
 
        Debug.Log(string.Format("Searched {0} Prefabs, {1} GameObjects, {2} components, found {3} missing",
            prefabCount, go_count, components_count, missing_count));
    }

    private static void FindInChild(GameObject parent)
    {
        go_count++;
        Component[] components = parent.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = parent.name;
                Transform t = parent.transform;
                while (t.parent != null) 
                {
                    s = t.parent.name +"/"+s;
                    t = t.parent;
                }
                Debug.Log (s + " has an empty script attached in position: " + i, parent);
            }
        }
        
        for (var i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            FindInChild(child);
        }
    }
}
#endif
