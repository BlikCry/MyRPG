using System;
using Items;
using UnityEngine;

[Serializable]
public class Item
{
    public readonly string Name;

    public bool IsActive { get; private set; }

    public virtual string Description => $"item \"{Name}\"";

    protected Item(ItemTemplate template)
    {
        Name = template.Title;
    }

    public void ToggleActive() => IsActive = !IsActive;

    public static Item GetItem(ItemTemplate template)
    {
        if (template is WeaponTemplate weaponTemplate)
            return new Weapon(weaponTemplate);
        if (template is ArmorTemplate armorTemplate)
            return new Armor(armorTemplate);
        if (template is AidTemplate aidTemplate)
            return new Aid(aidTemplate);

        return new Item(template);
    }
}