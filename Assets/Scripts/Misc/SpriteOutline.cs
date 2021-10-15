using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
internal class SpriteOutline: MonoBehaviour
{
    private void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var icon = new GameObject("fill");
        icon.transform.SetParent(transform);
        icon.transform.localPosition = Vector3.zero;
        icon.transform.localScale = Vector3.one * 1.2f;
        var newRenderer = icon.AddComponent<SpriteRenderer>();
        newRenderer.sprite = spriteRenderer.sprite;
        newRenderer.color = Color.black;
        newRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        Destroy(this);
    }
}