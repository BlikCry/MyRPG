using UnityEditor;
using UnityEngine;

namespace Misc
{
    internal static class Helpers
    {
        # if UNITY_EDITOR
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var a = new T[guids.Length];
            for(var i =0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
 
            return a;
        }
        #endif
    }
}
