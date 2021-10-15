using System;
using System.Collections;
using Objects;
using UnityEngine;

public class CharacterBrain : MonoBehaviour
{
    private CharacterBody body;
    private OneBitAnimator obAnimator;

    private void Awake()
    {
        body = GetComponent<CharacterBody>();
        obAnimator = GetComponent<OneBitAnimator>();
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            yield return new WaitForSeconds(body.MoveDuration);
            
            var x = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            var y = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                var center = new Vector2(Screen.width, Screen.height) / 2;
                x = (touch.position.x < center.x / 2 ? -1 : 0) + (touch.position.x > center.x * 1.5f ? 1 : 0);
                y = (touch.position.y < center.y / 2 ? -1 : 0) + (touch.position.y > center.y * 1.5f ? 1 : 0);
            }

            if (x == 0 && y == 0)
                continue;
            var nextPosition = transform.position + new Vector3(x, y, 0);
            TryMovement(nextPosition);
        }
    }

    private void TryMovement(Vector3 nextPosition)
    {
        var tile = Physics2D.OverlapPoint(nextPosition);
        if (tile is null)
            transform.position = nextPosition;
        else if (tile.TryGetComponent<MortalBody>(out var tileBody))
        {
            tileBody.TakeDamage(body.Damage);
            RunAttackAnimation(nextPosition);
        }
        else if (tile.TryGetComponent<Chest>(out var chest))
        {
            chest.Open(body);
            RunAttackAnimation(nextPosition);
        }
        else if (tile.TryGetComponent<Door>(out var door))
        {
            door.Go();
        }
        else if (tile.TryGetComponent<SimpleNpc>(out var npc))
        {
            npc.Interact();
        }
    }

    private void RunAttackAnimation(Vector3 nextPosition)
    {
        obAnimator.MakeAttack(nextPosition - transform.position, body.MoveDuration);
    }
}
