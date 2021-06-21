using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HedgehogAnimationController : MonoBehaviour
{
    [SerializeField]
    private DragonBonesAnimationController animationController;

    private void Awake()
    {
        List<DragonBonesAnimationController.Parameter<bool>> parametersBool = 
            new List<DragonBonesAnimationController.Parameter<bool>>();
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsAggressive"));

        var idle = new DragonBonesAnimationController.AnimationSettings("Idle");
        var aggressive = new DragonBonesAnimationController.AnimationSettings("Aggressive", 1);

        idle.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(aggressive, false,
            new DragonBonesAnimationController.FadeSettings(0.1f, 0.1f), () => parametersBool[0].Value == true)
        };
        aggressive.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(idle, false,
            new DragonBonesAnimationController.FadeSettings(0.1f, 0.1f), () => parametersBool[0].Value == false)
        };

        List<DragonBonesAnimationController.AnimationsLayer> layers =
            new List<DragonBonesAnimationController.AnimationsLayer>()
        {
                new DragonBonesAnimationController.AnimationsLayer(animationController.ArmatureComponent,
                parametersBool, new List<DragonBonesAnimationController.AnimationSettings>() {idle, aggressive},
                idle, 0)
        };

        animationController.Init(parametersBool, layers);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            animationController.SetBool("IsAggressive", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            animationController.SetBool("IsAggressive", false);
        }
    }
}
