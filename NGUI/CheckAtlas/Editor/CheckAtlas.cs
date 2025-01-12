using System.Collections.Generic;
using System.IO;
using ES_TK.Global;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// 하위 오브젝트 중에 특정한 아틀라스를 사용중인 오브젝트를 찾기 위한 기능
/// 1. 유니티 상단 메뉴의 NGUI Extension 에 Check 에 CheckAtlas 클릭
/// 2. 아틀라스 선택
/// 3. Hierachy 에서 찾고싶은 오브젝트 선택
/// 4. Find 버튼 클릭
/// 5. 사용중인 아틀라스가 있을 경우 로그로 남겨짐
public class CheckAtlas : EditorWindow
{
    private static INGUIAtlas atlas;

    private static int totalComponent;

    private static int checkObjectCount;

    [MenuItem("NGUI Extension/Check/CheckAtlas")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CheckAtlas));
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            ComponentSelector.Draw("Atlas", atlas, OnSelectAtlas, true, GUILayout.MinWidth(80f));
        }
        GUILayout.EndHorizontal();

        if (atlas != null)
        {
            if (GUILayout.Button(string.Format("Find UISprite has [{0}]", atlas)))
            {
                SelectedCheckAtlas();
            }

            if (GUILayout.Button("Atlas Refresh"))
            {
                RefreshAtlas();
            }
        }
    }

    private void SelectedCheckAtlas()
    {
        GameObject[] go = Selection.gameObjects;


        foreach (GameObject g in go)
        {
            totalComponent = 0;
            checkObjectCount = 0;
            CheckInChild(g);
            string resultStr = "Check Atlas in " + checkObjectCount + " Object : " + g.name;
            if (totalComponent == 0)
            {
                resultStr += " - There is no atlas : " + atlas;
            }
            Debug.Log(resultStr);
        }
    }

    private static void CheckInChild(GameObject parent)
    {
        checkObjectCount++;
        Component[] components = parent.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] is UISprite)
            {
                UISprite sprite = components[i] as UISprite;
                if (sprite.atlas == atlas)
                {
                    string s = parent.name;
                    Transform t = parent.transform;
                    while (t.parent != null) 
                    {
                        s = t.parent.name +"/"+s;
                        t = t.parent;
                    }

                    totalComponent++;
                    Debug.LogWarning(sprite.atlas + " in position: \r\n" + s, parent);
                }
            }
        }

        for (var i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            CheckInChild(child);
        }
    }

    private void OnSelectAtlas(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            if (obj is NGUIAtlas)
            {
                atlas = obj as NGUIAtlas;
            }
        }
    }

    [MenuItem("Kingdom/Refresh Atlas")]
    public static void RefreshAtlas()
    {
        SceneView.RepaintAll();
        List<string> dependencies = new List<string>();
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_LobbyUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_EventUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_BattleUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_CharacterBody_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_CharacterIllust_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_IconAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_SummonAtlas_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_WorldAtlas_01.asset"));
        // dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/UI"));
        // dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_LobbyUIAtlas.asset"));
        // dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/TK_Object/UI"));
        // dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_LobbyUIAtlas.asset"));
        // dependencies.Add("Assets/Scenes/Common/LobbyScene_New.unity");
        for (var i = 0; i < dependencies.Count; i++)
        {
            if (dependencies[i].Contains(".shader") || dependencies[i].Contains(".cs"))
            {
                continue;
            }

            NGUIAtlasImporter.isImportAtlas = true;
            AssetDatabase.ImportAsset(dependencies[i], ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);

            if (dependencies[i].Contains(".asset"))
            {
                var nguiAtlas = AssetDatabase.LoadAssetAtPath<NGUIAtlas>(dependencies[i]);
                nguiAtlas.MarkAsChanged();
                
                foreach (var obj in FindObjectsOfType(typeof(UIPanel)))
                {
                    var panel = obj as UIPanel;
                    if (panel != null)
                    {
                        panel.Refresh();
                    }
                }
            }
        }
        SceneView.RepaintAll();
        // AssetDatabase.ImportAsset("Assets/Resources_moved/Object/Atlas", ImportAssetOptions.ImportRecursive);
        // AssetDatabase.ImportAsset("Assets/Scenes/Common", ImportAssetOptions.ImportRecursive);
        // AssetDatabase.ImportAsset("Assets/Scenes/EternalGames", ImportAssetOptions.ImportRecursive);
        // AssetDatabase.ImportAsset("Assets/Resources_moved/Object/UI", ImportAssetOptions.ImportRecursive);
    }

    [MenuItem("Kingdom/Refresh Atlas All")]
    public static void RefreshAtlas_All()
    {
        var sceneName = EditorSceneManager.GetActiveScene().path;
        if (!sceneName.Contains("EmptyScene"))
        {
            bool loadScene = EditorUtility.DisplayDialog("아틀라스 업데이트",
                "EmptyScene 에서 진행해주세요. Load 누르면 EmptyScene 이 로드 됩니다." +
                "\n저장후 진행해주세요",
                "LoadScene", "Cancel");
            if (loadScene)
            {
                EditorSceneManager.OpenScene("Assets/Scenes/Common/EmptyScene.unity", OpenSceneMode.Single);
            }
            return;
        }

        SceneView.RepaintAll();
        List<string> dependencies = new List<string>();
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_LobbyUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_EventUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_BattleUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_CharacterBody_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_CharacterIllust_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_IconAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_SummonAtlas_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_WorldAtlas_01.asset"));
        for (var i = 0; i < dependencies.Count; i++)
        {
            if (dependencies[i].Contains(".shader") || dependencies[i].Contains(".cs"))
            {
                continue;
            }

            NGUIAtlasImporter.isImportAtlas = true;
            AssetDatabase.ImportAsset(dependencies[i], ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);

            if (dependencies.Contains(".asset"))
            {
                var nguiAtlas = AssetDatabase.LoadAssetAtPath<NGUIAtlas>(dependencies[i]);

                if (nguiAtlas != null)
                {
                    nguiAtlas.MarkAsChanged();
                }
            }
        }
        
        NGUIAtlasImporter.isImportAtlas = true;
        AssetDatabase.ImportAsset("Assets/Resources_moved/Object/Atlas", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.ImportAsset("Assets/Resources_moved/TK_Object/UI", ImportAssetOptions.ImportRecursive| ImportAssetOptions.ForceUpdate| ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.ImportAsset("Assets/Resources_moved/Object/UI", ImportAssetOptions.ImportRecursive| ImportAssetOptions.ForceUpdate| ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.ImportAsset("Assets/Scenes/Common", ImportAssetOptions.ImportRecursive| ImportAssetOptions.ForceUpdate);

        //EditorSceneManager.OpenScene(sceneName, OpenSceneMode.Single);
        SceneView.RepaintAll();
    }

    public static void RefreshAtlas_NoDialog()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Common/EmptyScene.unity", OpenSceneMode.Single);
        while (!scene.isLoaded)
        {
        }
        SceneView.RepaintAll();
        List<string> dependencies = new List<string>();
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_LobbyUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_EventUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_BattleUIAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_CharacterBody_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_CharacterIllust_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_IconAtlas.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_SummonAtlas_01.asset"));
        dependencies.AddRange(AssetDatabase.GetDependencies("Assets/Resources_moved/Object/Atlas/EG_WorldAtlas_01.asset"));
        for (var i = 0; i < dependencies.Count; i++)
        {
            if (dependencies[i].Contains(".shader") || dependencies[i].Contains(".cs"))
            {
                continue;
            }

            NGUIAtlasImporter.isImportAtlas = true;
            AssetDatabase.ImportAsset(dependencies[i], ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        }
        SceneView.RepaintAll();
        // AssetDatabase.ImportAsset("Assets/Resources_moved/Object/Atlas", ImportAssetOptions.ImportRecursive);
        // AssetDatabase.ImportAsset("Assets/Scenes/Common", ImportAssetOptions.ImportRecursive);
        // AssetDatabase.ImportAsset("Assets/Scenes/EternalGames", ImportAssetOptions.ImportRecursive);
        // AssetDatabase.ImportAsset("Assets/Resources_moved/Object/UI", ImportAssetOptions.ImportRecursive);
    }
}

