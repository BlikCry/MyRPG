using System;
using ScriptableObjects;
using UI;
using UnityEngine;

namespace Objects
{
    internal class TraderNpc: BaseNpc
    {
        [SerializeField]
        private TraderData traderData;

        protected override void OnStart()
        {
            spriteRenderer.sprite = traderData.Icon;
        }

        protected override void OnInteract(bool _)
        {
            ShopUI.Instance.ShowUI(traderData);
        }
    }
}
