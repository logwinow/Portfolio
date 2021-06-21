using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectableObject : InteractableObject, ISavable
{
    [SerializeField]
    private SectionID id;

    public SectionID ID => id;
    public bool IsCollected { get; set; }

    public abstract bool TryCollect();

    public void SetPhysics(bool enabled)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.isKinematic = !enabled;

        GetComponent<Collider2D>().enabled = enabled;
    }
    

    public void Load(object obj)
    {
        if (obj is null)
        {
            IsCollected = true;
            gameObject.SetActive(false);

            return;
        }
        
        var colObjSL = (CollectableObjectSL) obj;
        
        transform.position = colObjSL.Position;
        transform.rotation = colObjSL.Rotation;
        gameObject.SetActive(!colObjSL.IsCollected);
    }

    public bool Save(out object obj)
    {
        obj = IsCollected ? null : new CollectableObjectSL(this);

        return true;
    }    

    [Serializable]
    private class CollectableObjectSL
    {
        public CollectableObjectSL(CollectableObject collectableObject)
        {
            _position = collectableObject.transform.position;
            _rotation = collectableObject.transform.rotation;
            _isCollected = collectableObject.IsCollected;
            ID = collectableObject.ID;
        }
        
        private Vector3 _position;
        private Quaternion _rotation;
        private bool _isCollected;

        public Vector3 Position => _position;
        public Quaternion Rotation => _rotation;
        public bool IsCollected => _isCollected;
        public SectionID ID { get; private set; }
    }
}
