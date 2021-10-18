using System.Collections;
using Misc;
using UnityEngine;

namespace Entities
{
    internal class GuardBrain: MonoBehaviour
    {
        [SerializeField]
        private int overviewDistance;
        [SerializeField]
        private float thinkDuration;
        [SerializeField]
        private float damage;
        [SerializeField]
        private RectZone rect;

        private OneBitAnimator obAnimator;

        private void Awake()
        {
            obAnimator = GetComponent<OneBitAnimator>();
        }

        private void Start()
        {
            StartCoroutine(Movement());
            MapNavigation.Instance.AddEntity(transform, false);
            GetComponent<MortalBody>().OnDie += () => MapNavigation.Instance.RemoveEntity(transform);

        }

        // Update is called once per frame
        private IEnumerator Movement()
        {
            var wait = new WaitForSeconds(thinkDuration);
            while (true)
            {
                yield return wait;
                MakeMove();
            }
        }

        private void MakeMove()
        {
            var position = transform.position;
            var startCell = MapNavigation.Instance.GetCellPosition(position);
            var enemyPositionObject = MapNavigation.Instance.GetClosestEnemyPosition(position, overviewDistance);
            Vector2Int? nextPoint = null;
            if (enemyPositionObject is { } enemyPosition && rect.ClampToZone(enemyPosition) == enemyPosition)
            {
                var direction = enemyPosition - startCell;

                if (direction.sqrMagnitude < 1.01)
                {
                    Attack(MapNavigation.Instance.GetWorldPosition(enemyPosition));
                    obAnimator.MakeAttack(direction, thinkDuration);
                    return;
                }
                nextPoint = MapNavigation.Instance.GetPathPoint(startCell, enemyPosition, overviewDistance);
            }

            nextPoint ??= MapNavigation.Instance.GetPathPoint(startCell, rect.ClampToZone(RandomPoint(startCell)), 10) ?? startCell;
            if (nextPoint is { } v && MapNavigation.Instance.IsVoid(v))
                transform.position = MapNavigation.Instance.GetWorldPosition(v);
        }

        private Vector2Int RandomPoint(Vector2Int start)
        {
            var randomDirection = MapNavigation.Ways[Random.Range(0, 4)];
            return start + randomDirection;
        }

        private void Attack(Vector3 hostilePosition)
        {
            var tile = Physics2D.OverlapPoint(hostilePosition);
            if (tile == null)
                return;
            if (tile.TryGetComponent<MortalBody>(out var tileBody))
                tileBody.TakeDamage(damage, transform);
        }
    }
}
