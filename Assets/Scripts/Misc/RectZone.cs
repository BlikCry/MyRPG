using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Misc
{
    [RequireComponent(typeof(RectTransform))]
    internal class RectZone: MonoBehaviour
    {
        private RectTransform rectTransform;

        public Vector2 Center => rectTransform.anchoredPosition;
        public Vector2 HalfSize => rectTransform.sizeDelta / 2;
        public Vector2 Min => Center - HalfSize;
        public Vector2 Max => Center + HalfSize;
        public Vector2Int MinCell => MapNavigation.Instance.GetCellPosition(Min);
        public Vector2Int MaxCell => MapNavigation.Instance.GetCellPosition(Max);

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public Vector2Int[] GetVoidCellPoint()
        {
            return (
                    from x in Enumerable.Range(MinCell.x, MaxCell.x - MinCell.x + 1)
                    from y in Enumerable.Range(MinCell.y, MaxCell.y - MinCell.y + 1)
                    where MapNavigation.Instance.IsVoid(new Vector2Int(x, y))
                    select new Vector2Int(x, y)
                ).ToArray();
        }

        public Vector2Int RandomIntPoint()
        {
            return MapNavigation.Instance.GetCellPosition(RandomPoint());
        }

        public Vector2 RandomPoint()
        {
            return new Vector2(Random.Range(Min.x, Max.x), Random.Range(Min.y, Max.y));
        }

        public Vector2Int ClampToZone(Vector2Int point)
        {
            var worldPoint = ClampToZone(MapNavigation.Instance.GetWorldPosition(point));
            return MapNavigation.Instance.GetCellPosition(worldPoint);
        }

        public Vector2 ClampToZone(Vector2 point)
        {
            return new Vector2(Mathf.Clamp(point.x, Min.x, Max.x), Mathf.Clamp(point.y, Min.y, Max.y));
        }
    }
}
