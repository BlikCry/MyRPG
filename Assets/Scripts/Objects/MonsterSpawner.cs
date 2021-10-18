using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Misc;
using UnityEngine;

namespace Objects
{
    internal class MonsterSpawner: MonoBehaviour, ISaveDataProvider
    {
        [SerializeField]
        private RectZone zone;
        [SerializeField]
        private int monsterMapCount;
        [SerializeField]
        private GameObject[] monsters;
        [SerializeField]
        private MonsterSpawner[] monsterSpawners;
        
        private static int monsterCount;
        private int monsterStartCount;
        private int monsterPerSpawner;

        private List<(GameObject obj, int id)> monstersObjects = new List<(GameObject, int)>();

        private void Awake()
        {
            monsterSpawners = FindObjectsOfType<MonsterSpawner>();
            monsterCount = 0;
        }

        private void Start()
        {
            var spawnerCount = monsterSpawners.Length;
            monsterStartCount = MapNavigation.Instance.GetMonsterCount();
            monsterPerSpawner = Mathf.CeilToInt(((float)monsterMapCount - monsterStartCount) / spawnerCount - 0.01f);
            var i = monsterPerSpawner;
            while (monsterStartCount + monsterCount < monsterMapCount && i > 0)
            {
                SpawnMonster();
                i--;
            }
        }

        private void SpawnMonster()
        {
            var usedPoints = monstersObjects.Select(m => MapNavigation.Instance.GetCellPosition(m.obj.transform.position)).ToArray();
            var points = zone.GetVoidCellPoint().Where(p => !usedPoints.Contains(p)).ToArray();
            if (points.Length == 0)
                return;
            var point = (Vector2)points.GetRandomItem();
            var monster = monsters.GetRandomItem(out var index);

            monstersObjects.Add((Instantiate(monster, point, Quaternion.identity), index));
            monsterCount++;
        }

        public byte[] SaveState()
        {
            var buffer = new ByteBuffer();
            monstersObjects = monstersObjects.Where(m => m.obj).ToList();
            buffer.WriteInt32(monstersObjects.Count);
            foreach (var monsterObject in monstersObjects)
            {
                buffer.WriteInt32(monsterObject.id);
                var saveDataProviders = monsterObject.obj.GetComponentsInChildren<MonoBehaviour>().OfType<ISaveDataProvider>().ToList();

                foreach (var bytes in saveDataProviders.Select(saveDataProvider => saveDataProvider.SaveState()))
                {
                    buffer.WriteInt32(bytes.Length);
                    buffer.WriteBytes(bytes);
                }
            }

            return buffer.ToArray();
        }

        public void LoadState(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            var count = buffer.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                monsterCount++;
                var index = buffer.ReadInt32();

                var monster = Instantiate(monsters[index]);
                var saveDataProviders = monster.GetComponentsInChildren<MonoBehaviour>().OfType<ISaveDataProvider>().ToList();

                foreach (var saveDataProvider in saveDataProviders)
                {
                    var bytes = buffer.ReadBytes(buffer.ReadInt32());
                    saveDataProvider.LoadState(bytes);
                }
            }
        }
    }
}
