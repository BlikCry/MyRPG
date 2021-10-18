using System.Collections;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private int overviewDistance;
    [SerializeField]
    private float thinkDuration;
    [SerializeField]
    private float damage;
    [SerializeField]
    private int experience;

    private OneBitAnimator obAnimator;
    private MortalBody body;

    private Transform attackBody;

    private void Awake()
    {
        obAnimator = GetComponent<OneBitAnimator>();
        body = GetComponent<MortalBody>();
        body.OnDie += () =>
        {
            if (attackBody is null)
                CharacterBody.Instance.LevelSystem.AddExperience(experience);
        };
        body.OnDamageTaken += (t, _) => attackBody = t;
    }

    private void Start()
    {
        StartCoroutine(Movement());
        MapNavigation.Instance.AddEntity(transform, true);
        body.OnDie += () => MapNavigation.Instance.RemoveEntity(transform);
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
        var startCell = MapNavigation.Instance.GetCellPosition(transform.position);
        var hostilePosition = attackBody ? MapNavigation.Instance.GetCellPosition(attackBody.position) : MapNavigation.Instance.GetPlayerPosition();
        var direction = (hostilePosition - startCell);
        if (direction.sqrMagnitude < 1.01)
        {
            Attack(MapNavigation.Instance.GetWorldPosition(hostilePosition));
            obAnimator.MakeAttack(direction, thinkDuration);
            return;
        }

        var nextPoint = MapNavigation.Instance.GetPathPoint(startCell, hostilePosition, overviewDistance);
        if (nextPoint is null)
        {
            var randomDirection = MapNavigation.Ways[Random.Range(0, 4)];
            nextPoint = startCell + randomDirection;
        }
        if (nextPoint is { } v && MapNavigation.Instance.IsVoid(v))
                transform.position = MapNavigation.Instance.GetWorldPosition(v);
    }

    private void Attack(Vector3 hostilePosition)
    {
        var tile = Physics2D.OverlapPoint(hostilePosition);
        if (tile == null)
            return;
        if (tile.TryGetComponent<MortalBody>(out var tileBody))
            tileBody.TakeDamage(damage);
    }

    public void LoadState(byte[] data)
    {
        var buffer = new ByteBuffer(data);
        if (!buffer.ReadBoolean())
            return;
        transform.position = new Vector2(buffer.ReadSingle(), buffer.ReadSingle());
    }
}
