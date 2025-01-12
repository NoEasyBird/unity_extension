using System;
using System.Collections.Generic;
using System.Linq;
using AnimationOrTween;
using UnityEngine;

namespace ES_TK.UI.Common
{
    /// Tweener 하위 오브젝트들을 한 번에 컨트롤 하기 위한 기능
    public class UITweenGroup : MonoBehaviour
    {
        public enum TweenType
        {
            SelfTween,
            ChildTween,
            Both,
        }

        public TweenType tweenType = TweenType.SelfTween;

        protected List<UITweener> tweens = new List<UITweener>();

        protected UITweener maxDurationTween;

        private Action onFinishedAction;

        private Action onUpdateAction;

        protected bool mCached;

        private bool lastPlayForward;

        public bool LastPlayForward => lastPlayForward;

        public float MaxDelayDuration
        {
            get
            {
                if (maxDurationTween == null)
                {
                    return 0.0f;
                }

                return maxDurationTween.duration + maxDurationTween.delay;
            }
        }

        public List<UITweener> Tweens => tweens;

        protected virtual void Awake()
        {
            if (!mCached)
            {
                FindTween();
            }
        }

        public virtual void FindTween()
        {
            tweens.Clear();
            var selfTweens = new List<UITweener>(GetComponents<UITweener>());
            var childTweens = new List<UITweener>(GetComponentsInChildren<UITweener>(true));
            childTweens.RemoveAll(x => selfTweens.Contains(x));
            switch (tweenType)
            {
                case TweenType.SelfTween:
                    tweens.AddRange(selfTweens);
                    break;
                case TweenType.ChildTween:
                    tweens.AddRange(childTweens);
                    break;
                case TweenType.Both:
                    tweens.AddRange(selfTweens);
                    tweens.AddRange(childTweens);
                    break;
            }
            
            tweens.ForEach(x =>
            {
                x.onFinished.Clear();
                if (maxDurationTween == null)
                {
                    maxDurationTween = x;
                }
                else if (maxDurationTween.duration + maxDurationTween.delay < x.duration + x.delay)
                {
                    maxDurationTween = x;
                }
            });

            if (maxDurationTween == null)
            {
                return;
            }
            maxDurationTween.SetOnFinished(OnFinished);
            EventDelegate.Add(maxDurationTween.onUpdated, OnUpdate);
            mCached = true;
        }

        public void SetOnFinished(Action action)
        {
            if (!mCached)
            {
                FindTween();
            }

            onFinishedAction = action;
        }

        public void AddOnFinished(Action action)
        {
            if (!mCached)
            {
                FindTween();
            }

            onFinishedAction += action;
        }

        public void SetOnUpdate(Action action)
        {
            if (!mCached)
            {
                FindTween();
            }

            onUpdateAction = action;
        }

        public void ResetOnFinished()
        {
            onFinishedAction = null;
        }

        public void ResetOnUpdate()
        {
            onUpdateAction = null;
        }

        public void PlayForward(bool doReset = false)
        {
            if (!mCached)
            {
                FindTween();
            }
            tweens.ForEach(x => x.Finish());
            tweens.ForEach(x =>
            {
                x.PlayForward();
                if (doReset)
                {
                    x.ResetToBeginning();
                }
            });
            lastPlayForward = true;
        }

        public void PlayReverse(bool doReset = false)
        {
            if (!mCached)
            {
                FindTween();
            }
            tweens.ForEach(x => x.Finish());
            tweens.ForEach(x =>
            {
                x.PlayReverse();
                if (doReset)
                {
                    x.ResetToBeginning();
                }
            });
            lastPlayForward = false;
        }

        protected void OnFinished()
        {
            onFinishedAction?.Invoke();
        }

        protected void OnUpdate()
        {
            onUpdateAction?.Invoke();
        }

        public void SetOnStart(bool isForward)
        {
            if (!mCached)
            {
                FindTween();
            }

            lastPlayForward = isForward;
            tweens.ForEach(x =>
            {
                x.tweenFactor = isForward ? 0f : 1f;
            });
        }

        public void ForceSetCurrentFactor(bool isFinished = false)
        {
            tweens.ForEach(x =>
            {
                x.Sample(x.tweenFactor, isFinished);
            });
        }

        public void ForceSetCurrentFactor(float factor)
        {
            tweens.ForEach(x =>
            {
                x.tweenFactor = factor;
                x.Sample(x.tweenFactor, factor >= 1f);
            });
        }

        public void ResetToBeginning()
        {
            tweens.ForEach(x =>
            {
                x.ResetToBeginning();
            });
        }

        public void Finished()
        {
            tweens.ForEach(x =>
            {
                x.Finish();
            });
        }

        public void ResetMaxTween()
        {
            if (maxDurationTween != null)
            {
                maxDurationTween.onFinished.Clear();
                maxDurationTween.onUpdated.Clear();
            }

            maxDurationTween = null;
            
            tweens.ForEach(x =>
            {
                x.onFinished.Clear();
                if (maxDurationTween == null)
                {
                    maxDurationTween = x;
                }
                else if (maxDurationTween.duration + maxDurationTween.delay < x.duration + x.delay)
                {
                    maxDurationTween = x;
                }
            });

            if (maxDurationTween == null)
            {
                return;
            }
            
            maxDurationTween.SetOnFinished(OnFinished);
            EventDelegate.Add(maxDurationTween.onUpdated, OnUpdate);
        }

        public bool IsFinished()
        {
            return tweens.TrueForAll(x =>
            {
                if (x.direction == Direction.Forward)
                {
                    return x.tweenFactor >= 1f;
                }
                else
                {
                    return x.tweenFactor <= 0f;
                }
            });
        }
    }
}
