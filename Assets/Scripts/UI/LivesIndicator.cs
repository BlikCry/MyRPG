using System;
using System.Collections.Generic;
using Misc;
using UnityEngine;
using UnityEngine.UI;

internal class LivesIndicator: MonoBehaviour
{
    [SerializeField]
    private MortalBody body;
    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite halfHeart;
    [SerializeField]
    private Sprite emptyHeart;
    [SerializeField]
    private Transform container;
    [SerializeField]
    private Color outlineColor;

    private readonly List<Image> icons = new List<Image>();

    private void Start()
    {
        RecreateIcons();
    }

    private void Update()
    {
        if (container.childCount != body.MaxHealth)
            RecreateIcons();

        for (var i = 1; i <= body.MaxHealth; i++)
        {
            Sprite icon;
            if (body.Health > i)
                icon = fullHeart;
            else if (body.Health < i - 1)
                icon = emptyHeart;
            else
            {
                var ost = body.Health - i + 1;
                if (ost < 0.25)
                    icon = emptyHeart;
                else if (ost > 0.75)
                    icon = fullHeart;
                else
                    icon = halfHeart;
            }
            icons[i - 1].sprite = icon;
        }
    }

    private void RecreateIcons()
    {
        icons.Clear();
        container.RemoveChildren();
        for (var i = 0; i < body.MaxHealth; i++)
        {
            var icon = new GameObject();
            icon.transform.SetParent(container);
            var image = icon.AddComponent<Image>();
            icon.transform.localScale = Vector3.one;
            image.sprite = fullHeart;
            image.preserveAspect = true;
            var outline = icon.AddComponent<Outline>();
            outline.effectDistance = new Vector2(5, 0);
            outline.effectColor = outlineColor;
            icons.Add(image);
        }
    }
}