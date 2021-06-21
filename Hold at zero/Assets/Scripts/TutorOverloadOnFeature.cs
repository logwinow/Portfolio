using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorOverloadOnFeature : MonoBehaviour
{
    protected Slider refSlider;
    protected Feature feature;
    public bool isActive = false;
    //public Action Callback { get; set; }

    void Awake()
    {
        refSlider = GetComponent<Slider>();
        feature = GetComponent<Feature>();
    }

    protected virtual void LateUpdate()
    {
        if (isActive)
        {
            if (refSlider.value == 0)
            {
                feature.CurParameters = null;
                Messenger.Broadcast(MyGameEvents.SHOW_CUR_TIP);

                isActive = false;
            }
        }
    }
}
