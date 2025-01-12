using Addressable;
using UnityEngine;

namespace Utility
{
    public class MonoSingleton<T> : CallBackMonoBehaviour where T : CallBackMonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = ResourceHelper.Load<T>(typeof(T).GetPath());
                }

                return instance;
            }
        }

        public bool dontDestroyOnLoad;

        protected override void Awake()
        {
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
            base.Awake();
        }
    }
}