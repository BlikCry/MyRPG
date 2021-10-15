using System;
using Saves;
using UnityEngine;

public class MortalBody : MonoBehaviour, ISaveDataProvider
{
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private float health;
    [SerializeField]
    private bool destroyOnDie;
    [Space]
    [SerializeField]
    private string logBodyName;

    public int MaxHealth => maxHealth;
    public float Health => health;
    public event Action OnDie;
    
    public void TakeDamage(float damage)
    {
        health -= CalculateDamage(damage);
        CheckHealth();
    }

    public void RestoreHealth(float value)
    {
        health = Mathf.Min(health + value, MaxHealth);
    }

    public void Die()
    {
        OnDie?.Invoke();
        GameLog.AddEntry($"Body {logBodyName} dies");
        if (destroyOnDie)
            Destroy(gameObject);
    }

    private void CheckHealth()
    {
        if (health < float.Epsilon)
            Die();
    }

    protected virtual float CalculateDamage(float rawDamage) => rawDamage;

    public virtual byte[] SaveState()
    {
        var buffer = new ByteBuffer().WriteSingle(health);

        if (this == null)
            return buffer.WriteBoolean(false).ToArray();
        var position = transform.position;
        return buffer.WriteBoolean(true).WriteSingle(position.x).WriteSingle(position.y).ToArray();
    }

    public virtual void LoadState(byte[] data)
    {
        var buffer = new ByteBuffer(data);
        health = buffer.ReadSingle();
        CheckHealth();

        if (!buffer.ReadBoolean())
            return;
        transform.position = new Vector2(buffer.ReadSingle(), buffer.ReadSingle());
    }
}
