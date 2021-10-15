using UnityEngine;

[CreateAssetMenu(fileName = "new Armor", menuName = "Armor", order = -1)]
public class ArmorTemplate: ItemTemplate
{
    [SerializeField]
    private float armor;

    public float Armor => armor;
}