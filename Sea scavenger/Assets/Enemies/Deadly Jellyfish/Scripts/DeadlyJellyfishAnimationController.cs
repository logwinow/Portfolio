using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyJellyfishAnimationController : MonoBehaviour
{
    private void Awake()
    {
        var dbAnimContr = GetComponent<DragonBonesAnimationController>();

        var idle = new DragonBonesAnimationController.AnimationSettings("Idle");

        dbAnimContr.Init(null, new List<DragonBonesAnimationController.AnimationsLayer>()
        {
            new DragonBonesAnimationController.AnimationsLayer(dbAnimContr.ArmatureComponent,
            null, new List<DragonBonesAnimationController.AnimationSettings>() {idle }, idle, 0)
        });
    }
}
