using UnityEngine;

public abstract class ItemTemplate : ScriptableObject
{
    [SerializeField]
    private string title;
    
    public string Title => title;
}