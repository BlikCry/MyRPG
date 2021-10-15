using UnityEngine;

[CreateAssetMenu(fileName = "new Weapon", menuName = "Weapon", order = -1)]
public class WeaponTemplate: ItemTemplate
{
    [SerializeField]
    private float damage;

    public float Damage => damage;
}