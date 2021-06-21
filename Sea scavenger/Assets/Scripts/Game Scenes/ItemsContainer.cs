using System.Collections;
using System.Collections.Generic;
using Custom.Patterns;
using UnityEngine;

public abstract class ItemsContainer : MonoBehaviour
{
    protected abstract void Put(CollectableObject collectableObject);
    protected abstract bool PutValidate();

    public bool PutIfValidate(CollectableObject collectableObject)
    {
        if (!PutValidate())
            return false;
        
        Put(collectableObject);
        return true;
    }
}
