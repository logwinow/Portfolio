using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondSuitPart : CollectableObject
{
    [SerializeField] private SectionID _partsCountParameterId;
    public override bool TryCollect()
    {
        //GameManager.Instance.SetParameterAndSaveIt(_partsCountParameterId);
        GameManager.Instance.GetParameter(_partsCountParameterId).ChangeAt(1);

        return true;
    }
}
