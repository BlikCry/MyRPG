using System;

[Serializable]
public class Armor: Item
{
    private const float BlockAmount = 0.1f;

    public readonly float RawArmor;
    public float Block => RawArmor * BlockAmount;
    public float Decrease => (100 - RawArmor) * 0.01f;

    public override string Description => $"armor: {RawArmor}";

    public Armor(ArmorTemplate template) : base(template)
    {
        RawArmor = template.Armor;
    }
}