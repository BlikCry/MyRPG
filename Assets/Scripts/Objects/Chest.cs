using System;
using System.Collections.Generic;
using UnityEngine;

internal class Chest: MonoBehaviour, ISaveDataProvider
{
    [SerializeField]
    private List<ItemTemplate> items;
    [SerializeField]
    private int coins;

    private bool opened;

    private void Start()
    {
        MapNavigation.Instance.AddEntity(transform, false);
    }

    public void Open(CharacterBody character)
    {
        items.ForEach(template => character.AddItem(Item.GetItem(template)));
        character.AddCoins(coins);
        MapNavigation.Instance.RemoveEntity(transform);
        opened = true;
        Destroy(gameObject);
    }

    public byte[] SaveState()
    {
        return new[] {Convert.ToByte(opened)};
    }

    public void LoadState(byte[] data)
    {
        opened = Convert.ToBoolean(data[0]);
        if (opened)
            Destroy(gameObject);
    }
}