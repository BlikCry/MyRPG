using System;
using QuantumTek.QuantumDialogue;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace UI
{
    internal class DialogueUI: MonoBehaviour
    {
        [SerializeField]
        private GameObject ui;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private TMP_Text speakerNameText;
        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private GameObject buttonPrefab;
        [SerializeField]
        private Transform buttonContainer;

        private QD_Conversation currentConversation;
        private QD_Message currentMessage;
        private QD_Dialogue currentDialogue;
        private QD_Choice currentChoice;

        private QD_Choice currentQuestNode;
        private bool ignoreBaseTap;

        public static DialogueUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            ui.SetActive(false);
        }

        public void PlayDialogue(QD_Dialogue dialogue, bool isAgain)
        {
            Time.timeScale = 0;
            currentChoice = null;
            currentQuestNode = null;
            ignoreBaseTap = false;

            currentDialogue = dialogue;
            currentConversation = (isAgain 
                ? dialogue.GetConversation(QD_Conversation.ConversationType.Again)
                : dialogue.GetConversation(QD_Conversation.ConversationType.Startup)) ?? dialogue.GetConversation(QD_Conversation.ConversationType.Base);
            currentMessage = dialogue.GetMessage(currentConversation.FirstMessage);
            ui.SetActive(true);
            NextMessage();
        }

        public void NextMessage()
        {
            if (ignoreBaseTap)
                return;

            for (var i = 0; i < buttonContainer.childCount; i++)
            {
                Destroy(buttonContainer.GetChild(i).gameObject);
            }

            if (currentMessage is null)
            {
                if (currentConversation?.Type == QD_Conversation.ConversationType.Startup ||
                    currentConversation?.Type == QD_Conversation.ConversationType.Again)
                {
                    currentConversation = currentDialogue.GetConversation(QD_Conversation.ConversationType.Base);
                    currentMessage = currentDialogue.GetMessage(currentConversation.FirstMessage);
                    NextMessage();
                    return;
                }
                Time.timeScale = 1;
                ui.SetActive(false);
                return;
            }

            text.text = currentMessage.MessageText;
            var speaker = currentDialogue.GetSpeaker(currentMessage.Speaker);
            icon.sprite = speaker.Icon;
            speakerNameText.text = speaker.Name;
            currentChoice = currentDialogue.GetChoice(currentMessage.NextMessage);
            
            if (currentChoice != null)
            {
                if (currentChoice.IsQuestNode)
                {
                    switch (currentChoice.QuestData.QuestState)
                    {
                        case QuestData.State.IsNotAccepted:
                            currentQuestNode = currentChoice;
                            currentMessage = currentDialogue.GetMessage(currentChoice.NotAcceptedNode);
                            break;
                        case QuestData.State.IsAccepted:
                            currentMessage = currentDialogue.GetMessage(currentChoice.AcceptedNode);
                            break;
                        case QuestData.State.IsDone:
                            currentMessage = currentDialogue.GetMessage(currentChoice.DoneNode);
                            currentChoice.QuestData.Done();
                            break;
                        case QuestData.State.IsComplete:
                            currentMessage = currentDialogue.GetMessage(currentChoice.CompleteNode);
                            break;
                    }
                }
                else
                {
                    ignoreBaseTap = true;
                    for (var i = 0; i < currentChoice.Choices.Count; i++)
                    {
                        var button = Instantiate(buttonPrefab, buttonContainer);
                        var buttonButton = button.GetComponentInChildren<Button>();
                        ((TMP_Text) buttonButton.targetGraphic).text = currentChoice.Choices[i];
                        var i1 = i;
                        buttonButton.onClick.AddListener(() =>
                        {
                            currentMessage = currentDialogue.GetMessage(currentChoice.NextMessages[i1]);
                            ignoreBaseTap = false;
                            NextMessage();
                        });
                    }
                }
            }
            else
            {
                currentMessage = currentDialogue.GetMessage(currentMessage.NextMessage);
                if (currentMessage is null && currentQuestNode != null)
                {
                    ignoreBaseTap = true;
                    var acceptButton = Instantiate(buttonPrefab, buttonContainer);
                    var acceptButtonComponent = acceptButton.GetComponentInChildren<Button>();
                    ((TMP_Text) acceptButtonComponent.targetGraphic).text = currentQuestNode.AcceptMessage;
                    acceptButtonComponent.onClick.AddListener(() =>
                    {
                        currentQuestNode.QuestData.Activate();
                        currentMessage = null;
                        currentConversation = null;
                        ignoreBaseTap = false;
                        NextMessage();
                    });

                    var denyButton = Instantiate(buttonPrefab, buttonContainer);
                    var denyButtonComponent = denyButton.GetComponentInChildren<Button>();
                    ((TMP_Text) denyButtonComponent.targetGraphic).text = currentQuestNode.DenyMessage;
                    denyButtonComponent.onClick.AddListener(() =>
                    {
                        currentMessage = null;
                        currentConversation = null;
                        ignoreBaseTap = false;
                        NextMessage();
                    });
                }
            }
        }
    }
}
