using UnityEngine;

namespace Misc
{
    public static class Extensions
    {
        public static void RemoveChildren(this Transform transform)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static T GetRandomItem<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static T GetRandomItem<T>(this T[] array, out int index)
        {
            return array[index = Random.Range(0, array.Length)];
        }
    }
}
