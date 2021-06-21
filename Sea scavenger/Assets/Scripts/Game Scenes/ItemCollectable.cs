using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollectable : CollectableObject
{   
    public override bool TryCollect()
    {
        return SceneManager.Instance.Container.PutIfValidate(this);
    }
}
