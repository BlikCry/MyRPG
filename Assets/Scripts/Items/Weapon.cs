using System;

[Serializable]
public class Weapon : Item
{
    public readonly float Damage;

    public override string Description => $"damage: {Damage}";

    public Weapon(WeaponTemplate template) : base(template)
    {
        Damage = template.Damage;
    }
}