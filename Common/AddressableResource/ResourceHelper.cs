using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utility;
using Object = UnityEngine.Object;

namespace Addressable
{
    public static class ResourceHelper
    {
        private static List<Object> managedObjects = new List<Object>();

        public static T Load<T>(AssetReference assetReference) where T : Object
        {
            var op = Addressables.LoadAssetAsync<T>(assetReference);
            op.WaitForCompletion();
            managedObjects.Add(op.Result);
            return op.Result;
        }

        public static void Load<T>(AssetReference assetReference, Action<T> callBack) where T : Object
        {
            Addressables.LoadAssetAsync<T>(assetReference).Completed += (op) =>
            {
                managedObjects.Add(op.Result);
                callBack?.Invoke(op.Result);
            };
        }

        public static T Load<T>(string path) where T : Object
        {
            var op = Addressables.LoadAssetAsync<T>(path);
            op.WaitForCompletion();
            managedObjects.Add(op.Result);
            return op.Result;
        }

        public static void Load<T>(string path, Action<T> callBack) where T : Object
        {
            Addressables.LoadAssetAsync<T>(path).Completed += (op) =>
            {
                managedObjects.Add(op.Result);
                callBack?.Invoke(op.Result);
            };
        }

        public static void Release<T>(this T release) where T : Object
        {
            if (managedObjects.Contains(release))
            {
                managedObjects.Remove(release);
            }
            Addressables.Release(release);
        }

        public static void ReleaseAll()
        {
            var releaseObjects = managedObjects.FindAll(x => x != null);
            releaseObjects.ForEach(Addressables.Release);
            managedObjects.Clear();
        }

        public static GameObject LoadInstantiate(AssetReference assetReference)
        {
            var op = Addressables.InstantiateAsync(assetReference);
            op.WaitForCompletion();
            return op.Result;
        }

        public static GameObject LoadInstantiate(string path)
        {
            var op = Addressables.InstantiateAsync(path);
            op.WaitForCompletion();
            return op.Result;
        }

        public static T LoadInstantiate<T>(AssetReference assetReference) where T : Component
        {
            var op = Addressables.InstantiateAsync(assetReference);
            op.WaitForCompletion();
            var gameObj = op.Result;
            return gameObj.GetComponent<T>();
        }

        public static void LoadInstantiate<T>(AssetReference assetReference, Action<T> callBack) where T : Component
        {
            Addressables.InstantiateAsync(assetReference).Completed += op =>
            {
                var component = op.Result.GetComponent<T>();
                callBack?.Invoke(component);
            };
        }

        public static T LoadInstantiate<T>(string path) where T : Component
        {
            var op = Addressables.InstantiateAsync(path);
            op.WaitForCompletion();
            var gameObj = op.Result;
            return gameObj.GetComponent<T>();
        }

        public static void LoadInstantiate<T>(string path, Action<T> callBack) where T : Component
        {
            Addressables.InstantiateAsync(path).Completed += op =>
            {
                var component = op.Result.GetComponent<T>();
                callBack?.Invoke(component);
            };
        }

        public static void UnLoadInstantiate(GameObject obj)
        {
            Addressables.ReleaseInstance(obj);
        }

        public static void UnLoad(Object obj)
        {
            if (managedObjects.Contains(obj))
            {
                managedObjects.Remove(obj);
            }
            Addressables.Release(obj);
        }

        public static void UnLoadAll()
        {
            managedObjects.ForEach(Addressables.Release);
            managedObjects.Clear();
        }
    }
}