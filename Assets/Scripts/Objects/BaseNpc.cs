using UnityEngine;

namespace Objects
{
    internal class BaseNpc: MonoBehaviour, ISaveDataProvider
    {
        private const float MinFlipTime = 1f;
        private const float MaxFlipTime = 4f;

        [SerializeField]
        protected SpriteRenderer spriteRenderer;
        [SerializeField]
        private float flipTime = 1f;

        private bool isInteracted;

        private void Awake()
        {
            Flip();
        }

        private void Start()
        {
            MapNavigation.Instance.AddEntity(transform, false);
        }

        protected virtual void OnStart() {}

        private void Flip()
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            Invoke(nameof(Flip), Random.Range(MinFlipTime, MaxFlipTime * flipTime));
        }

        protected virtual void OnInteract(bool isInteracted) {}

        public void Interact()
        {
            OnInteract(isInteracted);
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
