using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapNavigation : MonoBehaviour
{
    public static readonly Vector2Int[] Ways = new Vector2Int[] {Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up};

    [SerializeField]
    private Tilemap solidTilemap;

    
    [SerializeField]
    private Transform player;

    private readonly List<(Transform transform, bool isEnemy)> entityList = new List<(Transform transform, bool isEnemy)>();

    public static MapNavigation Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        entityList.Add((player, false));
    }

    public int GetMonsterCount()
    {
        return entityList.Count(e => e.transform && e.isEnemy);
    }

    public void AddEntity(Transform entity, bool isEnemy)
    {
        entityList.Add((entity, isEnemy));
    }

    public void RemoveEntity(Transform entity)
    {
        entityList.RemoveAt(entityList.FindIndex(e => e.transform == entity));
    }

    public Vector2Int GetCellPosition(Vector2 position)
    {
        return (Vector2Int) solidTilemap.WorldToCell(position);
    }

    public Vector3 GetWorldPosition(Vector2Int position)
    {
        return solidTilemap.CellToWorld((Vector3Int) position) + Vector3.one / 2;
    }

    public Vector2Int GetPlayerPosition()
    {
        return GetCellPosition(player.position);
    }

    public Vector2Int? GetClosestEnemyPosition(Vector2 position, int radius)
    {
        var sqrRadius = Mathf.Pow(radius, 2);
        var list = entityList
            .Select(e => (((Vector2) e.transform.position - position).sqrMagnitude, e))
            .Where(e => e.e.isEnemy && e.sqrMagnitude < sqrRadius)
            .OrderBy(e => e.sqrMagnitude)
            .ToList();

        if (list.Count > 0)
            return GetCellPosition(list[0].e.transform.position);
        return null;
    }

    public Vector2Int? GetPathPointToPlayer(Vector2Int startCell, int seekDistance)
    {
        return GetPathPoint(startCell, GetPlayerPosition(), seekDistance);
    }

    public Vector2Int? GetPathPoint(Vector2Int startCell, Vector2Int endCell, int seekDistance)
    {
        var q = new Queue<(Vector2Int, int)>();
        var d = new Dictionary<Vector2Int, Vector2Int>();
        q.Enqueue((startCell, 0));
        d.Add(startCell, startCell);

        while (q.Count > 0)
        {
            var (point, distance) = q.Dequeue();
            if (distance > seekDistance)
                continue;
            distance++;
            foreach (var way in Ways)
            {
                var nextPoint = way + point;
                if (nextPoint == endCell)
                {
                    q.Clear();
                    d[endCell] = point;
                    break;
                }

                if (d.ContainsKey(nextPoint))
                    continue;
                if (!IsVoid(nextPoint))
                    continue;
                d[nextPoint] = point;
                q.Enqueue((nextPoint, distance));
            }
        }

        if (!d.ContainsKey(endCell))
            return null;

        while (d[endCell] != startCell)
        {
            endCell = d[endCell];
        }

        return IsVoid(endCell) ? endCell : startCell;
    }

    public bool IsVoid(Vector2Int cellPosition)
    {
        if (solidTilemap.HasTile((Vector3Int) cellPosition))
            return false;

        return !entityList.Exists(t => GetCellPosition(t.transform.position) == cellPosition);
    }
}
