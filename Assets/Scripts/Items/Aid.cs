using System;

namespace Items
{
    [Serializable]
    public class Aid : Item
    {
        public readonly float AidValue;

        public override string Description => $"aid: {AidValue}";

        public Aid(AidTemplate template) : base(template)
        {
            AidValue = template.AidValue;
        }
    }
}
