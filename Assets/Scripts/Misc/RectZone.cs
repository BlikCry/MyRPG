using UnityEngine;

namespace Misc
{
    [RequireComponent(typeof(RectTransform))]
    internal class RectZone: MonoBehaviour
    {
        private RectTransform rectTransform;

        public Vector2 Center => rectTransform.anchoredPosition;
        public Vector2 HalfSize => rectTransform.sizeDelta / 2;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public Vector2Int ClampToZone(Vector2Int point)
        {
            var worldPoint = ClampToZone(MapNavigation.Instance.GetWorldPosition(point));
            return MapNavigation.Instance.GetCellPosition(worldPoint);
        }

        public Vector2 ClampToZone(Vector2 point)
        {
            var min = Center - HalfSize;
            var max = Center + HalfSize;
            return new Vector2(Mathf.Clamp(point.x, min.x, max.x), Mathf.Clamp(point.y, min.y, max.y));
        }
    }
}
