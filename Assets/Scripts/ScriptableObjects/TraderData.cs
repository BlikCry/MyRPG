using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "new Trader", menuName = "TraderData", order = -201)]
    internal class TraderData: ScriptableObject
    {
        public string TraderName;
        public Sprite Icon;

        public string WelcomeMessage = "Welcome to my store. What do you want?";
        public string NoMoneyMessage = "You have not enough money.";
        public string ThankYouMessage = "Thank you!";

        public ItemTemplate[] Items;
    }
}
