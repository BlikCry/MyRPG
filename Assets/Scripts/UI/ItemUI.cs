using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class ItemUI: MonoBehaviour
{
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private GameObject flag;

    public Item ItemObject;

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (ItemObject is null)
            return;
        titleText.text = ItemObject.Name;
        flag.SetActive(ItemObject.IsActive);
    }
}