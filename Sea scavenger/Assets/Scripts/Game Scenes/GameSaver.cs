using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Runtime.Serialization;
using Custom.SerializationSurrogates;
using System.Reflection;

public static class GameSaver
{
    static GameSaver()
    {
        _dirSavesPath = Path.Combine(Application.dataPath, "Resources", "Saves");
        _dirScenesPath = Path.Combine(_dirSavesPath, "Scenes");
        _dirGlobalPath = Path.Combine(_dirSavesPath, "Global");
        _surrogateSelector = SerializationSurrogatesUtilities.ChainSurrogates(
            Vector3SerializationSurrogate.SurrogateSelector, QuaternionSerializationSurrogate.SurrogateSelector);
    }

    private static string _dirSavesPath;
    private static string _dirScenesPath;
    private static string _dirGlobalPath;
    private static SceneData _sceneData;
    private static GlobalData _globalData;
    private static SurrogateSelector _surrogateSelector;

    public static SceneData SavedSceneData => _sceneData;
    public static GlobalData SavedGlobalData => _globalData;
    
    public static event Action onSave;
    public static event Action onLoadBeforeCheck;
    public static event Action onLoadAfterCheck;

    private static bool CheckPath(string filePath)
    {
        if (File.Exists(filePath))
            return true;

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        return false;
    }

    private static string GetFilePath(string dirPath, int num)
    {
        return Path.Combine(dirPath, $"save_{num}.dat");
    }

    private static bool Load<T>(string filePath, out T loadedData)
        where T : new()
    {
        if (!CheckPath(filePath))
        {
            loadedData = new T();
            
            return false;
        }
        
        loadedData = BinarySaver.Load<T>(filePath, _surrogateSelector);

        return true;
    }

    public static void Load(int number)
    {
        var loadSceneResult = Load(GetFilePath(_dirScenesPath, number), out _sceneData);
        var loadGlobalResult = Load(GetFilePath(_dirGlobalPath, 0), out _globalData);
        
        onLoadBeforeCheck?.Invoke();
        
        if (loadSceneResult)
        {
            _sceneData.RestoreSavableObjects();
        }

        if (loadGlobalResult)
        {
            _globalData.LoadParameters();
        }
        
        onLoadAfterCheck?.Invoke();
    }

    public static void Save(int number)
    {
        onSave?.Invoke();
        
        BinarySaver.Save(_sceneData, GetFilePath(_dirScenesPath, number), _surrogateSelector);
        BinarySaver.Save(_globalData, GetFilePath(_dirGlobalPath, 0), _surrogateSelector);
    }

    public static bool CheckSaves()
    {
        return Directory.Exists(_dirSavesPath);
    }

    public static void DeleteAllSaves()
    {
        if (!CheckSaves())
            return;
        
        Directory.Delete(_dirSavesPath, true);
    }

    [Serializable]
    public class SceneData
    {
        public SceneData()
        {
            _sceneObjectsDict = new Dictionary<Guid, object>();
            _sceneSavableObjects = new List<SavableObject>();
        }
        
        private Dictionary<Guid, object> _sceneObjectsDict;
        [NonSerialized]
        private List<SavableObject> _sceneSavableObjects;
        
        public void AddSceneObject(SavableObject savableObject, object savingData)
        {
            if (_sceneObjectsDict.ContainsKey(savableObject.GUID))
            {
                _sceneObjectsDict[savableObject.GUID] = savingData;
            }
            else
                _sceneObjectsDict.Add(savableObject.GUID, savingData);
        }

        public void RegisterSavableObject(SavableObject savableObject)
        {
            _sceneSavableObjects ??= new List<SavableObject>();
            
            _sceneSavableObjects.Add(savableObject);
        }

        public void Remove(SavableObject savableObject)
        {
            _sceneObjectsDict.Remove(savableObject.GUID);
        }

        // public bool TryGetSavableObjectData(SavableObject savableObject, out object data)
        // {
        //     return _sceneObjectsDict.TryGetValue(savableObject.GUID, out data);
        // }

        public void RestoreSavableObjects()
        {
            foreach (var o in _sceneObjectsDict)
            {
                var savableObject = _sceneSavableObjects.FirstOrDefault(so => so.GUID == o.Key);

                if (savableObject != null)
                    savableObject.Load(o.Value);
                else
                    CreateSavableObject(o.Key, o.Value);
            }
        }

        private void CreateSavableObject(Guid guid, object data)
        {
            var so = UnityEngine.Object.Instantiate(
                GameManager.Instance.GetItem(
                    (SectionID)(data.GetType().GetProperty("ID")!.GetValue(data)))
                    .Prefab)
                .AddComponent<SavableObject>();
            so.GetType().GetField("_guid", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(so, guid);
            so.Load(data);
        }
    }

    [Serializable]
    public class GlobalData
    {
        public GlobalData()
        {
            _inventoryDict = new Dictionary<SectionID, int>();
            _parameters = new Dictionary<SectionID, int>();
        }

        private Dictionary<SectionID, int> _parameters;
        private Dictionary<SectionID, int> _inventoryDict;

        public void AddParameter(GameManager.Parameter parameter)
        {
            AddParameter(parameter.SectionId, parameter.Value);
        }

        public void AddParameter(SectionID parameterID, int value)
        {
            _parameters[parameterID] = value;
        }

        public void LoadParameters()
        {
            foreach (var p in _parameters)
            {
                GameManager.Instance.GetParameter(p.Key).Set(p.Value, false);
            }
        }

        public void AddItemCount(SectionID itemID, int count = 1)
        {
            SetItem(itemID, _inventoryDict.ContainsKey(itemID) ?
                _inventoryDict[itemID] + count :
                count);
        }
        
        public void SetItem(SectionID itemID, int count)
        {
            if (count <= 0)
            {
                _inventoryDict.Remove(itemID);
                return;
            }

            _inventoryDict[itemID] = count;
        }

        public void LoadToStorage(BoatStorage storage)
        {
            foreach (var i in _inventoryDict)
            {
                storage.Put(i.Key, i.Value);
            }
        }
    }
}
