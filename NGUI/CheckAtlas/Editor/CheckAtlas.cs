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
}

