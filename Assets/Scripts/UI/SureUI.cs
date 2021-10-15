using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    internal class SureUI: MonoBehaviour
    {
        [SerializeField]
        private GameObject ui;
        [SerializeField]
        private Button yesButton;
        [SerializeField]
        private Button noButton;

        private Action<bool> callback;

        private void Awake()
        {
            yesButton.onClick.AddListener(() => SendCallback(true));
            noButton.onClick.AddListener(() => SendCallback(false));
            ui.SetActive(false);
        }

        public void ShowUI(Action<bool> callbackAction)
        {
            callback = callbackAction;
            ui.SetActive(true);
        }

        public void HideUI()
        {
            ui.SetActive(false);
        }

        private void SendCallback(bool value)
        {
            callback?.Invoke(value);
            ui.SetActive(false);
        }
    }
}
