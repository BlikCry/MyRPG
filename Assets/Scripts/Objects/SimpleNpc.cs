using System;
using QuantumTek.QuantumDialogue;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Objects
{
    internal class SimpleNpc: BaseNpc
    {
        [SerializeField]
        private QD_Dialogue dialogue;

        protected override void OnInteract(bool isInteracted)
        {
            DialogueUI.Instance.PlayDialogue(dialogue, isInteracted);
        }
    }
}
