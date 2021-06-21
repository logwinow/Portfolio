using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SavableObject : MonoBehaviour, ISerializationCallbackReceiver
{ 
    [NonReorderable]
    [ReadOnly]
    [SerializeField] private byte[] _guidBytes;
    [SerializeField] private bool _isPermanent;

    private Guid _guid;
    private ISavable _target; 
        
    public Guid GUID => _guid;

    private void Awake()
    {
        _target = GetComponent<ISavable>();
        
        GameSaver.onLoadBeforeCheck += OnLoadBeforeCheckCallback;
        GameSaver.onSave += OnSaveCallback;
    }

    public void UpdateGuid()
    {
        _guid = Guid.NewGuid();
    }

    private void OnDestroy()
    {
        GameSaver.onLoadBeforeCheck -= OnLoadBeforeCheckCallback;
        GameSaver.onSave -= OnSaveCallback;
    }

    private void OnLoadBeforeCheckCallback()
    {
        GameSaver.SavedSceneData.RegisterSavableObject(this);
    }

    private void OnSaveCallback()
    {
        if (!_target.Save(out var obj))
            return;
        
        if (obj is null && !_isPermanent)
        {
            GameSaver.SavedSceneData.Remove(this);
            
            return;
        }

        GameSaver.SavedSceneData.AddSceneObject(this, obj);
    }

    public void OnBeforeSerialize()
    {
        if (_guid == Guid.Empty)
        {
            _guid = Guid.NewGuid();
        }

        _guidBytes = _guid.ToByteArray();
    }

    public void OnAfterDeserialize()
    {
        _guid = _guidBytes != null ? new Guid(_guidBytes) : Guid.NewGuid();
    }

    public void Load(object savedObject)
    {
        _target.Load(savedObject);
    }
}

public interface ISavable
{
    void Load(object obj);
    bool Save(out object obj);
}
