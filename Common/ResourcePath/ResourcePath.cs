using System;
using System.Collections.Generic;
using Systems;
using Systems.UI;

namespace Utility
{
    public static class ResourcePath
    {
        public readonly static Dictionary<Type, string> Path = new Dictionary<Type, string>()
        {
            {typeof(GlobalCoroutineBehaviour), "Prefab/Global/GlobalCoroutineBehaviour"},
            {typeof(BattleManager), "Prefab/Global/BattleManager"},
            {typeof(UIController), "Prefab/Global/UIController"},
            {typeof(BackGroundManager), "Prefab/Global/BackGroundManager"},
        };

        public static string GetPath(this Type type)
        {
            if (!Path.ContainsKey(type))
            {
                LogManager.LogError("No Path Data : {0}".Format(type));
                return "";
            }

            return Path[type];
        }
    }
}