namespace Utility
{
    public class Singleton<T> where T : Singleton<T>, new ()
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new T();
                    mInstance.Init();
                }

                return mInstance;
            }
        }

        /// <summary>
        /// When Singleton Called
        /// </summary>
        protected virtual void Init()
        {
        }
    
        public virtual void Reset()
        {
            mInstance = null;
        }
    }
}