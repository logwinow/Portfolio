using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowJellyfishAnimationController : MonoBehaviour
{
    [SerializeField]
    private DragonBonesAnimationController animationContr;

    private void Awake()
    {
        var idle = new DragonBonesAnimationController.AnimationSettings("Idle");

        List<DragonBonesAnimationController.AnimationsLayer> layers =
            new List<DragonBonesAnimationController.AnimationsLayer>()
            {
                new DragonBonesAnimationController.AnimationsLayer(animationContr.ArmatureComponent,
                null, new List<DragonBonesAnimationController.AnimationSettings>()
                {
                    idle
                }, idle, 0)
            };
        animationContr.Init(null, layers);
    }
}
