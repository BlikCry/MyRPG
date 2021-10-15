using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "new Aid", menuName = "Aid", order = -1)]
    public class AidTemplate: ItemTemplate
    {
        [SerializeField]
        private float aidValue;

        public float AidValue => aidValue;
    }
}
