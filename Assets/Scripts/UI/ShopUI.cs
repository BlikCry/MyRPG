using System;
using System.Linq;
using Misc;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    internal class ShopUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject blockPanel;

        [SerializeField]
        private GameObject itemsUI;
        [SerializeField]
        private GameObject itemPrefab;
        [SerializeField]
        private Transform itemContainer;
        [SerializeField]
        private int itemInPage;
        [SerializeField]
        private Button nextButton;
        [SerializeField]
        private Button prevButton;
        [SerializeField]
        private SureUI sureUI;

        [SerializeField]
        private GameObject dialogueUI;
        [SerializeField]
        private Image speakerImage;
        [SerializeField]
        private TMP_Text speakerNameText;
        [SerializeField]
        private TMP_Text dialogueText;
        [SerializeField]
        private GameObject choiceOptionPrefab;
        [SerializeField]
        private Transform choiceContainer;

        public static ShopUI Instance { get; private set; }

        private TraderData traderData;
        
        private int page;
        private int MaxPage => (getItemsToShow().Length - 1) / itemInPage;

        private Func<Item[]> getItemsToShow;
        private Action<Item> itemClickAction;
        private Func<Item, bool> checkItemAction;

        private void Awake()
        {
            Instance = this;
            nextButton.onClick.AddListener(NextPage);
            prevButton.onClick.AddListener(PrevPage);
            HideUI();
        }

        public void ShowUI(TraderData data)
        {
            Time.timeScale = 0;
            blockPanel.SetActive(true);
            dialogueUI.SetActive(true);
            sureUI.HideUI();
            itemsUI.SetActive(false);
            traderData = data;
            speakerImage.sprite = data.Icon;
            speakerNameText.text = data.TraderName;
            ShowMainScreen();
        }

        private void HideUI()
        {
            Time.timeScale = 1;
            blockPanel.SetActive(false);
            dialogueUI.SetActive(false);
            sureUI.HideUI();
            itemsUI.SetActive(false);
        }

        private void ShowMainScreen()
        {
            sureUI.HideUI();
            itemsUI.SetActive(false);
            dialogueText.text = traderData.WelcomeMessage;
            choiceContainer.RemoveChildren();
            AddOption("Buy", () => ShowScreen(ShowBuyScreen));
            AddOption("Sell", () => ShowScreen(ShowSellScreen));
            AddOption("Exit", () => ShowScreen(HideUI));
        }

        private void ShowScreen(Action showScreenMethod)
        {
            dialogueText.text = "";
            choiceContainer.RemoveChildren();
            AddOption("Exit", ShowMainScreen);
            page = 0;
            itemsUI.SetActive(true);
            showScreenMethod();
        }

        private void ShowBuyScreen()
        {
            getItemsToShow = () => traderData.Items.Select(Item.GetItem).ToArray();
            itemClickAction = BuyItem;
            checkItemAction = item =>
            {
                if (CharacterBody.Instance.Coins < item.Cost)
                {
                    dialogueText.text = traderData.NoMoneyMessage;
                    return false;
                }
                dialogueText.text = "";
                return true;
            };
            ShowScreenPage();
        }
        
        private void ShowSellScreen()
        {
            getItemsToShow = () => CharacterBody.Instance.Items;
            itemClickAction = SellItem;
            checkItemAction = _ => true;
            ShowScreenPage();
        }

        private void NextPage()
        {
            page++;
            ShowScreenPage();
        }

        private void PrevPage()
        {
            page--;
            ShowScreenPage();
        }

        private void ShowScreenPage()
        {
            var itemsToShow = getItemsToShow();
            page = Mathf.Clamp(page, 0, MaxPage);
            prevButton.gameObject.SetActive(page > 0);
            nextButton.gameObject.SetActive(page < MaxPage);
            itemContainer.RemoveChildren();
            Enumerable.Range(page * itemInPage, itemInPage).ToList().ForEach(itemId => { if (itemId < itemsToShow.Length) AddItem(itemsToShow[itemId]); });
        }

        private void OnItemClick(Item item)
        {
            if (!checkItemAction(item))
                return;

            prevButton.interactable = false;
            nextButton.interactable = false;
            sureUI.ShowUI((b =>
            {
                prevButton.interactable = true;
                nextButton.interactable = true;
                if (!b) return;
                itemClickAction?.Invoke(item);
                ShowScreenPage();
            }));
        }

        private void SellItem(Item item)
        {
            CharacterBody.Instance.RemoveItem(item);
            CharacterBody.Instance.AddCoins(item.Cost);
        }

        private void BuyItem(Item item)
        {
            if (CharacterBody.Instance.AddCoins(-item.Cost))
                CharacterBody.Instance.AddItem(item);
            else
                dialogueText.text = traderData.NoMoneyMessage;
        }

        private void AddItem(Item item)
        {
            var row = Instantiate(itemPrefab, itemContainer);
            var itemUI = row.GetComponent<ItemUI>();
            itemUI.ItemObject = item;
            itemUI.UpdateUI();
            row.GetComponent<Button>().onClick.AddListener(() => OnItemClick(item));
        }

        private void AddOption(string text, UnityAction action)
        {
            var option = Instantiate(choiceOptionPrefab, choiceContainer);
            var button = option.GetComponent<Button>();

            button.onClick.AddListener(action);
            ((TMP_Text) button.targetGraphic).text = text;
        }
    }
}
