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
    }
}
