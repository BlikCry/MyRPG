using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Entities;
using Items;
using UnityEngine;

public class CharacterBody : MortalBody, IUniqueSaveDataProvider
{
    [SerializeField]
    private float baseDamage;
    [SerializeField]
    private float moveDuration;
    [SerializeField]
    private List<ItemTemplate> startupItems;

    private Weapon weapon;
    private Armor armor;
    public Weapon Weapon => weapon;
    public Armor Armor => armor;

    public float Damage => (weapon?.Damage ?? baseDamage) * LevelSystem.AttackMultiplier;
    public float MoveDuration => moveDuration / LevelSystem.SpeedMultiplier;
    protected override float HealthMultiplier => LevelSystem.HealthMultiplier;

    public int ItemCount => items.Count;

    private List<Item> items;
    private int coins;

    public int Coins => coins;
    public Item[] Items => items.ToArray();

    public static CharacterBody Instance;

    public ExperienceContainer LevelSystem { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (items is null)
        {
            items = new List<Item>();
            startupItems.ForEach(item => items.Add(Item.GetItem(item)));
        }

        if (LevelSystem is null)
        {
            LevelSystem = new ExperienceContainer(1, 0);
        }
    }

    private void Start()
    {
        if (!(Door.CurrentPoint is { } point)) return;
        Door.CurrentPoint = null;
        transform.position = point;
    }

    public Item GetItem(int id)
    {
        return items[id];
    }

    public bool AddCoins(int coinsDelta)
    {
        if (coins + coinsDelta >= 0)
        {
            coins += coinsDelta;
            return true;
        }

        return false;
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        if (item.IsActive)
            ToggleEquip(item, out _);
        items.Remove(item);
    }

    public void ToggleEquip(Item item, out bool isLost)
    {
        isLost = false;
        if (item is Weapon itemWeapon)
            Equip(ref weapon, itemWeapon);
        if (item is Armor itemArmor)
            Equip(ref  armor, itemArmor);
        if (item is Aid aid)
        {
            RestoreHealth(aid.AidValue);
            items.Remove(item);
            isLost = true;
        }
    }

    private void Equip<T>(ref T currentItem, T newItem) where T : Item
    {
        if (newItem?.IsActive ?? false)
            newItem = null;
        currentItem?.ToggleActive();
        newItem?.ToggleActive();
        currentItem = newItem;
    }

    protected override float CalculateDamage(float rawDamage)
    {
        rawDamage -= armor?.Block ?? 0;
        rawDamage *= armor?.Decrease ?? 1;
        return Mathf.Max(rawDamage, 0);
    }

    public override byte[] SaveState()
    {
        var formatter = new BinaryFormatter();

        using var stream = new MemoryStream();
        formatter.Serialize(stream, items);
        var data = stream.ToArray();

        var buffer = new ByteBuffer();
        buffer.WriteInt32(LevelSystem.Level);
        buffer.WriteInt32(LevelSystem.Experience);

        buffer.WriteInt32(coins);
        buffer.WriteInt32(data.Length);
        buffer.WriteBytes(data);

        buffer.WriteBytes(base.SaveState());
        return buffer.ToArray();
    }

    public override void LoadState(byte[] data)
    {
        var buffer = new ByteBuffer(data);
        var level = buffer.ReadInt32();
        var exp = buffer.ReadInt32();

        LevelSystem = new ExperienceContainer(level, exp);

        coins = buffer.ReadInt32();
        var itemsData = buffer.ReadBytes(buffer.ReadInt32());

        using var stream = new MemoryStream(itemsData);
        var formatter = new BinaryFormatter();
        items = (List<Item>)formatter.Deserialize(stream);

        items.ForEach(item =>
        {
            if (!item.IsActive) return;
            item.ToggleActive();
            ToggleEquip(item, out _);
        });

        base.LoadState(buffer.ReadBytes());
    }

    public string UniqueId => "Player";
}
