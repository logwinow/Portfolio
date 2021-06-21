using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureTutorial : TutorOverloadOnFeature
{
    protected override void LateUpdate()
    {
        if (isActive)
        {
            if (!feature.CurParameters.HasValue)
            {
                Messenger.Broadcast(MyGameEvents.SHOW_CUR_TIP);
                isActive = false;
            }
        }
    }

    void Update()
    {
        if (!isActive)
        {
            if (refSlider.value != 0)
            {
                refSlider.value = 0;
            }
        }
    }
}
