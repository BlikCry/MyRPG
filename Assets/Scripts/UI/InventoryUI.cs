using System;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class InventoryUI: MonoBehaviour
{
    private const int Rows = 6;

    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private CharacterBody body;
    [SerializeField]
    private Transform itemContainer;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button prevButton;
    [SerializeField]
    private TMP_Text infoName;
    [SerializeField]
    private TMP_Text infoDescription;

    private int page;
    private int MaxPage => (body.ItemCount + Rows - 1) / Rows;

    private Item selectedItem;

    private void Start()
    {
        nextButton.onClick.AddListener(() =>
        {
            page++;
            ShowPage();
        });
        prevButton.onClick.AddListener(() =>
        {
            page--;
            ShowPage();
        });
    }

    public void ShowInventory()
    {
        Time.timeScale = 0;
        ui.SetActive(true);
        selectedItem = null;
        infoName.text = "";
        infoDescription.text = "";
        page = 0;
        ShowPage();
    }

    public void HideInventory()
    {
        Time.timeScale = 1;
        ui.SetActive(false);
    }

    private void ShowPage()
    {
        prevButton.gameObject.SetActive(page > 0);
        nextButton.gameObject.SetActive(page < MaxPage - 1);

        page = Mathf.Clamp(page, 0, MaxPage - 1);

        itemContainer.RemoveChildren();

        var startId = Rows * page;
        var endId = Math.Min(startId + Rows, body.ItemCount);

        for (var i = startId; i < endId; i++)
        {
            var item = body.GetItem(i);
            var itemRow = Instantiate(itemPrefab, itemContainer);
            var itemObj = itemRow.GetComponent<ItemUI>();
            itemObj.ItemObject = item;
            itemObj.UpdateUI();
            itemRow.GetComponent<Button>().onClick.AddListener(() => ProcessItemClick(item));
        }
    }

    private void ProcessItemClick(Item item)
    {
        if (selectedItem == item)
        {
            body.ToggleEquip(item, out var isLost);
            if (isLost)
            {
                ShowPage();
                infoName.text = "";
                infoDescription.text = "";
            }

            return;
        }
        
        selectedItem = item;
        infoName.text = selectedItem.Name;
        infoDescription.text = selectedItem.Description;
    }
}