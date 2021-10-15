using TMPro;
using UnityEngine;

namespace UI
{
    internal class CoinsUI: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text coinsText;

        private void Update()
        {
            coinsText.text = CharacterBody.Instance.Coins.ToString();
        }
    }
}
