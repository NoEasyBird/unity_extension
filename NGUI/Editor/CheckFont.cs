using UnityEditor;
using UnityEngine;

/// UILabel에서 사용하고 있는 Font를 체크하기 위한 에디터 기능
/// 사용법 
/// 1. 유니티 상단 메뉴에 NGUI Extension - Check - CheckFont 클릭
/// 2. Font를 눌러서 폰트 선택
/// 3. Hierachy에서 원하는 오브젝트 선택 후 Find 버튼 클릭
/// 4. 오브젝트 및 하위 오브젝트에서 UILabel 에서 Font 사용중이면 로그로 남겨진다.
public class CheckFont : EditorWindow
{
    private static Font font;

    private static int totalComponent;

    private static int checkObjectCount;

    [MenuItem("NGUI Extension/Check/CheckFont")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CheckFont));
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            ComponentSelector.Draw("Font", font, OnSelectAtlas, true, GUILayout.MinWidth(80f));
        }
        GUILayout.EndHorizontal();

        if (font != null)
        {
            if (GUILayout.Button(string.Format("Find UILabel has [{0}]", font)))
            {
                SelectedCheckFont();
            }
        }
    }

    private void SelectedCheckFont()
    {
        GameObject[] go = Selection.gameObjects;


        foreach (GameObject g in go)
        {
            totalComponent = 0;
            checkObjectCount = 0;
            CheckInChild(g);
            string resultStr = "Check Font in " + checkObjectCount + " Object : " + g.name;
            if (totalComponent == 0)
            {
                resultStr += " - There is no different font : " + font;
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
            if (components[i] is UILabel)
            {
                UILabel label = components[i] as UILabel;
                if (label.trueTypeFont != font)
                {
                    string s = parent.name;
                    Transform t = parent.transform;
                    while (t.parent != null) 
                    {
                        s = t.parent.name +"/"+s;
                        t = t.parent;
                    }

                    totalComponent++;
                    if (label.trueTypeFont != null)
                    {
                        Debug.LogWarning(label.trueTypeFont + " in position: \r\n" + s, parent);
                    }
                    else
                    {
                        Debug.LogWarning(label.name + " in position: \r\n" + s, parent);
                    }
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
            if (obj is Font)
            {
                font = obj as Font;
            }
        }
    }
}
