using UnityEngine;

internal class EquipmentUI : MonoBehaviour
{
    private const string EmptyFieldName = "empty";

    [SerializeField]
    private CharacterBody body;
    [SerializeField]
    private TextFormatter weaponString;
    [SerializeField]
    private TextFormatter armorString;
    [SerializeField]
    private TextFormatter defenseString;
    [SerializeField]
    private TextFormatter damageString;

    private void Update()
    {   
        weaponString.Format(body.Weapon?.Name ?? EmptyFieldName);
        armorString.Format(body.Armor?.Name ?? EmptyFieldName);
        defenseString.Format(body.Armor?.RawArmor ?? 0);
        damageString.Format(body.Damage);
    }
}