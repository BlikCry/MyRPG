using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Misc;
using Saves;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
internal class SaveController: MonoBehaviour
{

    private const string LogUniqueId = "GameLogProvider";
    private const string SaveName = "save.sv";
    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveName);
    [SerializeField]
    private MonoBehaviour[] items;
    [SerializeField]
    private QuestData[] quests;

    public static SaveController Instance { get; private set; }

    private ISaveDataProvider[] providers;
    private IUniqueSaveDataProvider[] uniqueProviders;

    private static Dictionary<string, byte[]> scenes;
    private static Dictionary<string, byte[]> uniqueData;
    private static Dictionary<string, byte[]> questData;

    private void Awake()
    {
        if (!Application.isPlaying)
            return;

        Instance = this;
        providers = items.Cast<ISaveDataProvider>().ToArray();
        uniqueProviders = FindObjectsOfType<MonoBehaviour>().OfType<IUniqueSaveDataProvider>().ToArray();
       
        if (scenes is null)
            LoadAll();
        Load();
    }
    
# if UNITY_EDITOR
    private void Update()
    {
        
        if (Application.isPlaying)
            return;
        items = FindObjectsOfType<MonoBehaviour>().Where(o => o is ISaveDataProvider && !(o is IUniqueSaveDataProvider)).ToArray();
        quests = Helpers.GetAllInstances<QuestData>();
        
    }
#endif

    private void OnApplicationQuit()
    {
        Save();
        SaveAll();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!Application.isPlaying)
            return;
        Save();
        SaveAll();
    }

    public static void SaveAll()
    {
        foreach (var quest in Instance.quests)
        {
            questData[quest.QuestID] = quest.SaveState();
        }

        uniqueData[LogUniqueId] = GameLog.SaveState();

        using var stream = File.OpenWrite(SavePath);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, (scenes, uniqueData, questData, SceneManager.GetActiveScene().name));
    }

    public static void LoadAll()
    {
        if (!File.Exists(SavePath))
        {
            scenes = new Dictionary<string, byte[]>();
            uniqueData = new Dictionary<string, byte[]>();
            questData = new Dictionary<string, byte[]>();
            return;
        }
        using var stream = File.OpenRead(SavePath);
        var formatter = new BinaryFormatter();
        string sceneName;
        (scenes, uniqueData, questData, sceneName) = ((Dictionary<string, byte[]>, Dictionary<string, byte[]>, Dictionary<string, byte[]>, string)) formatter.Deserialize(stream);
        
        foreach (var quest in Instance.quests)
        {
            if (questData.ContainsKey(quest.QuestID))
                quest.LoadState(questData[quest.QuestID]);
        }

        if (uniqueData.ContainsKey(LogUniqueId))
            GameLog.LoadState(uniqueData[LogUniqueId]);
        SceneManager.LoadScene(sceneName);
    }

    public void Save()
    {
        var stream = new ByteBuffer();
        foreach (var provider in providers)
        {
            var bytes = provider.SaveState();
            stream.WriteInt32(bytes.Length);
            stream.WriteBytes(bytes);
        }

        foreach (var uniqueProvider in uniqueProviders)
        {
            uniqueData[uniqueProvider.UniqueId] = uniqueProvider.SaveState();
        }

        scenes[SceneManager.GetActiveScene().name] = stream.ToArray();
    }

    public void Load()
    {
        var key = SceneManager.GetActiveScene().name;
        if (scenes.ContainsKey(key))
        {
            var stream = new ByteBuffer(scenes[key]);
            foreach (var provider in providers)
            {
                var count = stream.ReadInt32();
                var bytes = stream.ReadBytes(count);
                provider.LoadState(bytes);
            }
        }

        foreach (var uniqueProvider in uniqueProviders)
        {
            if (uniqueData.ContainsKey(uniqueProvider.UniqueId))
                uniqueProvider.LoadState(uniqueData[uniqueProvider.UniqueId]);
        }
    }
}