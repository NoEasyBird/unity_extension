using System;
using UnityEngine;

namespace Utility
{
    public class CallBackMonoBehaviour : MonoBehaviour
    {
        public Action<GameObject> onAwakeAction;

        public Action<GameObject> onStartAction;
        
        public Action<GameObject> onDisableAction;

        public Action<GameObject> onEnableAction;

        public Action<GameObject> onDestroyAction;

        public Action<GameObject> onUpdateAction;
        
        public Action<GameObject> onFixedUpdateAction;

        protected virtual void Awake()
        {
            onAwakeAction?.Invoke(gameObject);
        }

        protected virtual void Start()
        {
            onStartAction?.Invoke(gameObject);
        }

        protected virtual void OnEnable()
        {
            onEnableAction?.Invoke(gameObject);
        }

        protected virtual void OnDisable()
        {
            onDisableAction?.Invoke(gameObject);
        }

        protected virtual void OnDestroy()
        {
            onDestroyAction?.Invoke(gameObject);
        }

        protected virtual void Update()
        {
            onUpdateAction?.Invoke(gameObject);
        }

        protected virtual void FixedUpdate()
        {
            onFixedUpdateAction?.Invoke(gameObject);
        }
    }
}