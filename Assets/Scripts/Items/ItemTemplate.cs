using UnityEngine;

public abstract class ItemTemplate : ScriptableObject
{
    [SerializeField]
    private string title;
    [SerializeField]
    private int cost;

    public string Title => title;
    public int Cost => cost;
}