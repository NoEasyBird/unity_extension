using System.Linq;
using AnimationOrTween;
using ES_TK.UI.Common;
using UnityEditor;
using UnityEngine;

namespace ES_TK.Editor
{
    [CustomEditor(typeof(UITweenGroup), true)]
    public class UITweenGroupInspector : UnityEditor.Editor
    {
        public bool isEditorPlaying = false;
        
        public double editorTime;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetTweenGroup = target as UITweenGroup;

            if (targetTweenGroup == null)
            {
                return;
            }

            if (GUILayout.Button("Play"))
            {
                if (isEditorPlaying)
                {
                    OnFinished();
                
                    targetTweenGroup.Tweens.ForEach(x =>
                    {
                        x.Finish();
                        x.ResetToBeginning();
                    });
                    return;
                }
                isEditorPlaying = true;
                editorTime = EditorApplication.timeSinceStartup;
                targetTweenGroup.FindTween();
                targetTweenGroup.Tweens.ForEach(x =>
                {
                    x.PlayForward();
                    x.Finish();
                    x.ResetToBeginning();
                });
                EditorApplication.update += EditorUpdate;
            }

            if (GUILayout.Button("Play Reverse"))
            {
                if (isEditorPlaying)
                {
                    OnFinished();
                
                    targetTweenGroup.Tweens.ForEach(x =>
                    {
                        x.Finish();
                        x.ResetToBeginning();
                    });
                    return;
                }
                isEditorPlaying = true;
                editorTime = EditorApplication.timeSinceStartup;
                targetTweenGroup.FindTween();
                targetTweenGroup.Tweens.ForEach(x =>
                {
                    x.PlayReverse();
                    x.Finish();
                    x.ResetToBeginning();
                });
                EditorApplication.update += EditorUpdate;
            }

            if (GUILayout.Button("Reset"))
            {
                OnFinished();
                
                targetTweenGroup.Tweens.ForEach(x =>
                {
                    x.Finish();
                    x.ResetToBeginning();
                });
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
        
        private void EditorUpdate()
        {
            if (!isEditorPlaying)
            {
                return;
            }
            
            var targetTweenGroup = target as UITweenGroup;

            if (targetTweenGroup == null || targetTweenGroup.Tweens == null)
            {
                isEditorPlaying = false;
                return;
            }
            
            if (targetTweenGroup.gameObject != Selection.activeGameObject)
            {
                isEditorPlaying = false;
                return;
            }

            bool isPlaying = false;
            var delta = EditorApplication.timeSinceStartup - editorTime; 
            editorTime = EditorApplication.timeSinceStartup;
            
            targetTweenGroup.Tweens.ForEach(x =>
            {
                if (x != null)
                {
                    x.EditorUpdate((float) delta, (float) editorTime);
                    bool play = (x.tweenFactor < 1f && x.direction == Direction.Forward) || (x.tweenFactor > 0f && x.direction == Direction.Reverse);
                    if (play || x.style != UITweener.Style.Once)
                    {
                        isPlaying = true;
                    }
                }
            });

            if (!isPlaying)
            {
                OnFinished();
            }
        }

        private void OnFinished()
        {
            EditorApplication.update -= EditorUpdate;
            isEditorPlaying = false;
        }
    }
}