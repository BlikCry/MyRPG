using System;
using System.Collections.Generic;
using System.Linq;
using Saves;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "new Quest", menuName = "QuestData", order = -1)]
    public class QuestData: ScriptableObject
    {
        public enum State
        {
            IsNotAccepted,
            IsAccepted,
            IsDone,
            IsComplete
        }

        public string QuestID;

        public string QuestName;
        [TextArea]
        public string QuestDescription;
        public string[] LogStrings;
        public ItemTemplate[] RewardItems;
        public int RewardCoins;
        public State QuestState => isDone ? State.IsComplete : !isActive ? State.IsNotAccepted : logStringsComplete < logCount.Count ? State.IsAccepted : State.IsDone;

        [NonSerialized]
        private bool isActive;
        [NonSerialized]
        private bool isDone;
        [NonSerialized]
        private Dictionary<string, int> logCount;
        [NonSerialized]
        private int logStringsComplete;

# if UNITY_EDITOR
        private void Awake()
        {
            if (QuestID == "" || QuestID is null)
                QuestID = GUID.Generate() + DateTime.Now.ToLongDateString();
        }
#endif

        public void Activate()
        {
            isActive = true;
            if (logCount is null)
            {
                logCount = new Dictionary<string, int>();
                LogStrings.ToList().ForEach(s =>
                {
                    if (logCount.ContainsKey(s))
                        logCount[s] += 1;
                    else
                        logCount[s] = 1;
                });
            }
            GameLog.OnEntry += GameLogOnEntry;
            GameLog.Entries.ForEach(GameLogOnEntry);
        }

        public void Done()
        {
            isDone = true;
            foreach (var itemTemplate in RewardItems)
            {
                CharacterBody.Instance.AddItem(Item.GetItem(itemTemplate));
            }
            CharacterBody.Instance.AddCoins(RewardCoins);
        }

        private void GameLogOnEntry(string entry)
        {
            if (logCount.ContainsKey(entry))
            {
                logCount[entry] -= 1;
                if (logCount[entry] == 0)
                    logStringsComplete += 1;
            }

            if (logStringsComplete >= logCount.Count)
            {
                GameLog.OnEntry -= GameLogOnEntry;
            }
        }

        public void LoadState(byte[] bytes)
        {
            var buffer = new ByteBuffer(bytes);
            isActive = buffer.ReadBoolean();
            isDone = buffer.ReadBoolean();
            var count = buffer.ReadInt32();
            logCount = new Dictionary<string, int>();
            for (var i = 0; i < count; i++)
            {
                logCount[buffer.ReadString()] = buffer.ReadInt32();
                
            }
            logStringsComplete = logCount.Count(e => e.Value <= 0);
            if (isActive)
                GameLog.OnEntry += GameLogOnEntry;
        }

        public byte[] SaveState()
        {
            var buffer = new ByteBuffer();
            buffer.WriteBoolean(isActive);
            buffer.WriteBoolean(isDone);
            buffer.WriteInt32(logCount?.Count ?? 0);
            if (logCount != null)
            {
                foreach (var keyValuePair in logCount)
                {
                    buffer.WriteString(keyValuePair.Key);
                    buffer.WriteInt32(keyValuePair.Value);
                }
            }

            return buffer.ToArray();
        }
    }
}
