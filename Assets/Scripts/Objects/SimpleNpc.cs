using System;
using QuantumTek.QuantumDialogue;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Objects
{
    internal class SimpleNpc: MonoBehaviour, ISaveDataProvider
    {
        private const float MinFlipTime = 1f;
        private const float MaxFlipTime = 4f;

        [SerializeField]
        private QD_Dialogue dialogue;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private float flipTime = 1f;

        private bool isInteracted;

        private void Awake()
        {
            Flip();
        }

        private void Flip()
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            Invoke(nameof(Flip), Random.Range(MinFlipTime, MaxFlipTime * flipTime));
        }

        public void Interact()
        {
            DialogueUI.Instance.PlayDialogue(dialogue, isInteracted);
            isInteracted = true;
        }

        public byte[] SaveState()
        {
            return new ByteBuffer().WriteBoolean(isInteracted).ToArray();
        }

        public void LoadState(byte[] data)
        {
            isInteracted = new ByteBuffer(data).ReadBoolean();
        }
    }
}
