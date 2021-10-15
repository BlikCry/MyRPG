using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saves
{
    internal static class GameLog
    {
        public static readonly List<string> Entries = new List<string>();

        public static event Action<string> OnEntry;

        public static void AddEntry(string entry)
        {
            Entries.Add(entry);
            OnEntry?.Invoke(entry);
        }

        public static void LoadState(byte[] bytes)
        {
            var buffer = new ByteBuffer(bytes);
            var count = buffer.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                Entries.Add(buffer.ReadString());
            }
        }

        public static byte[] SaveState()
        {
            var buffer = new ByteBuffer();
            buffer.WriteInt32(Entries.Count);
            Entries.ForEach(e => buffer.WriteString(e));
            return buffer.ToArray();
        }
    }
}
