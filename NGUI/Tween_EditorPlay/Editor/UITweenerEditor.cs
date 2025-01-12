//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2020 Tasharen Entertainment Inc
//-------------------------------------------------

//게임 플레이 안하고 UITweener들 사용해 보기위한 기능
//NGUI엔진 추가 - Play 안하고 에디터에서 트윈 해보기
using System;
//NGUI엔진 추가 - 끝
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UITweener), true)]
public class UITweenerEditor : Editor
{
//NGUI엔진 추가 - Play 안하고 에디터에서 트윈 해보기
	private bool isEditorPlaying = false;

	private double editorTime = 0f;
//NGUI엔진 추가 - 끝

	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(110f);
		base.OnInspectorGUI();
		DrawCommonProperties();
	}

	//NGUI엔진 추가 - Play 안하고 에디터에서 트윈 해보기
	private void EditorUpdate()
	{
		if (!isEditorPlaying)
		{
			return;
		}
		UITweener tw = target as UITweener;
		if (tw != null)
		{
			var delta = EditorApplication.timeSinceStartup - editorTime;
			editorTime = EditorApplication.timeSinceStartup;
			tw.EditorUpdate((float) delta, (float) editorTime);

			if (tw.tweenFactor >= 1f && tw.style == UITweener.Style.Once)
			{
				OnFinished();
			}
		}
		else
		{
			isEditorPlaying = false;
		}
	}
	//NGUI엔진 추가 - 끝

	protected void DrawCommonProperties ()
	{
		UITweener tw = target as UITweener;

		if (NGUIEditorTools.DrawHeader("Tweener"))
		{
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.SetLabelWidth(110f);

			GUI.changed = false;

			UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
			AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);

			GUILayout.BeginHorizontal();
			float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			var deff = (UITweener.DelayAffects)EditorGUILayout.EnumPopup("Delay Affects", tw.delayAffects);

			int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
			bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);
			bool fx = EditorGUILayout.Toggle("Use Fixed Update", tw.useFixedUpdate);

			//NGUI엔진 추가 - Play 안하고 에디터에서 트윈 해보기
			if (!isEditorPlaying)
			{
				if (GUILayout.Button("Play"))
				{
					if (isEditorPlaying)
					{
						OnFinished();
						return;
					}
					isEditorPlaying = true;
					editorTime = EditorApplication.timeSinceStartup;
					tw.ResetToBeginning();
					//tw.PlayForward();
					EditorApplication.update -= EditorUpdate;
					EditorApplication.update += EditorUpdate;
				}
			}
			else
			{
				if (GUILayout.Button("Stop"))
				{
					OnFinished();
					tw.Finish();
				}
			}

			if (GUILayout.Button("Reset"))
			{
				OnFinished();
				EditorApplication.update -= EditorUpdate;
				tw.Finish();
				tw.ResetToBeginning();
			}
			//NGUI엔진 추가 - 끝

			if (GUI.changed)
			{
				NGUIEditorTools.RegisterUndo("Tween Change", tw);
				tw.animationCurve = curve;
				//tw.method = method;
				tw.style = style;
				tw.ignoreTimeScale = ts;
				tw.tweenGroup = tg;
				tw.duration = dur;
				tw.delay = del;
				tw.delayAffects = deff;
				tw.useFixedUpdate = fx;
				NGUITools.SetDirty(tw);
			}
			NGUIEditorTools.EndContents();
		}

		NGUIEditorTools.SetLabelWidth(80f);
		NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
	}

	//NGUI엔진 추가 - Play 안하고 에디터에서 트윈 해보기
	private void OnFinished()
	{
		EditorApplication.update -= EditorUpdate;
		isEditorPlaying = false;
	}
	//NGUI엔진 추가 - 끝
}
