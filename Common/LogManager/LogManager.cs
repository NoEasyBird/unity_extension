using System;
using UnityEngine;

namespace Utility
{
    public enum LogType
    {
        None = 0,
        Live = 1,
        Server = 2,
    }
    
    public static class LogManager
    {
        public static void Log( string text, LogType logType = LogType.None)
        {
            switch (logType)
            {
                case LogType.Live:
                    Debug.Log(text);
                    break;
                case LogType.Server:
                    break;
                default:
#if DEBUG_LOG
                    Debug.Log(text);
#endif
                    break;
            }
        }
        
        public static void LogWarning(string text, LogType logType = LogType.None)
        {
            switch (logType)
            {
                case LogType.Live:
                    Debug.LogWarning(text);
                    break;
                case LogType.Server:
                    break;
                default:
#if DEBUG_LOG
                    Debug.LogWarning(text);
#endif
                    break;
            }
        }
        
        public static void LogError(string text, LogType logType = LogType.None)
        {
            switch (logType)
            {
                case LogType.Live:
                    Debug.LogError(text);
                    break;
                case LogType.Server:
                    break;
                default:
#if DEBUG_LOG
                    Debug.LogError(text);
#endif
                    break;
            }
        }
        
        public static void Log(this GameObject gameObject, string text, LogType logType = LogType.None)
        {
            switch (logType)
            {
                case LogType.Live:
                    Debug.Log(text, gameObject);
                    break;
                case LogType.Server:
                    break;
                default:
#if DEBUG_LOG
                    Debug.Log(text);
#endif
                    break;
            }
        }
        
        public static void LogWarning(this GameObject gameObject, string text, LogType logType = LogType.None)
        {
            switch (logType)
            {
                case LogType.Live:
                    Debug.LogWarning(text, gameObject);
                    break;
                case LogType.Server:
                    break;
                default:
#if DEBUG_LOG
                    Debug.LogWarning(text);
#endif
                    break;
            }
        }
        
        public static void LogError(this GameObject gameObject, string text, LogType logType = LogType.None)
        {
            switch (logType)
            {
                case LogType.Live:
                    Debug.LogError(text, gameObject);
                    break;
                case LogType.Server:
                    break;
                default:
#if DEBUG_LOG
                    Debug.LogError(text);
#endif
                    break;
            }
        }

        public static void LogErrorNoPrefab(string prefabName, LogType logType = LogType.None)
        {
            LogError("Not Exist [{0}] Prefab!".Format(prefabName), logType);
        }

        public static void LogErrorData(string dataName, LogType logType = LogType.None)
        {
            LogError("Data Error : [{0}]".Format(dataName), logType);
        }
    }
}