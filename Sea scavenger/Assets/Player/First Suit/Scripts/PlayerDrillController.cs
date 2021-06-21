using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.SmartCoroutines;

public class PlayerDrillController : MonoBehaviour
{
    [SerializeField] private float drillDamageScaler;
    [SerializeField] private SectionID _drillDamageParameterID;
    [ReadOnly]
    [SerializeField] private float drillDamage;

    private DrillableObject drillableObject;
    private SmartCoroutineCache drillingCor;
    private bool isActive = false;
    
    public bool IsActive => isActive;

    private void Awake()
    {
        drillingCor = new SmartCoroutineCache(this, DrillingRout);
    }

    private void Start()
    {
        drillDamage = GameManager.Instance.GetParameter(_drillDamageParameterID).Value;
    }

    public void StartDrilling(DrillableObject drObj)
    {
        if (!drObj.CheckCondition())
        {
            return;
        }
        
        isActive = true;

        drillableObject = drObj;

        drillingCor.Start();
        
        if (PlayerController.Instance.AnimationController is PlayerFirstSuitAnimationController firstAnimContr)
            firstAnimContr.SetDrillState(true);
        else
            PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsDrilling", true);
        AudioManager.Instance.PlayDrill();
    }
    
    public void StopDrilling()
    {
        isActive = false;

        drillingCor.Stop();
        if (PlayerController.Instance.AnimationController is PlayerFirstSuitAnimationController firstAnimContr)
            firstAnimContr.SetDrillState(false);
        else
            PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsDrilling", false);
        AudioManager.Instance.StopDrill();
    }

    private IEnumerator DrillingRout()
    {
        while (true)
        {
            if (drillableObject == null)
            {
                StopDrilling();
                break;
            }

            drillableObject.Drill(
                drillDamageScaler * drillDamage * Time.deltaTime);

            yield return null;
        }
    }
}
