using UnityEngine;

internal class OneBitAnimator: MonoBehaviour
{
    private const float Epsilon = 0.01f;
    private const float Distance = 0.5f;

    [SerializeField]
    private Transform body;

    private float thinkTime;

    private void Update()
    {
        if (body.localPosition.sqrMagnitude < Epsilon)
        {
            body.localPosition = Vector3.zero;
            return;
        }
        var direction = -body.localPosition.normalized;
        body.localPosition += direction * Distance / thinkTime * Time.deltaTime;
    }

    public void MakeAttack(Vector2 direction, float duration)
    {
        var position = direction.normalized * Distance;
        body.transform.localPosition = position;
        thinkTime = duration;
    }
}