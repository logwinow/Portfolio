using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWithoutSuitAnimationController : PlayerAnimationController
{
    private void Awake()
    {
        var parametersBool =
            new List<DragonBonesAnimationController.Parameter<bool>>()
            {
                new DragonBonesAnimationController.Parameter<bool>("IsMoving"), // 0
                new DragonBonesAnimationController.Parameter<bool>("IsJumping"), // 1
                new DragonBonesAnimationController.Parameter<bool>("IsFalling"), // 2
                new DragonBonesAnimationController.Parameter<bool>("IsInteracting"), // 3
                new DragonBonesAnimationController.Parameter<bool>("IsTaking") // 4
            };

        var idle = new DragonBonesAnimationController.AnimationSettings("Idle");
        var walk = new DragonBonesAnimationController.AnimationSettings("Walk");
        var jump = new DragonBonesAnimationController.AnimationSettings("UP", 1, doOnEnd:
            delegate
            {
                parametersBool[1].Value = false;
                parametersBool[2].Value = true;
            });
        var falling = new DragonBonesAnimationController.AnimationSettings("Fall_process");
        var landing = new DragonBonesAnimationController.AnimationSettings("Fall_end", 1);

        var layer2_null = new DragonBonesAnimationController.AnimationSettings();
        var interaction = new DragonBonesAnimationController.AnimationSettings("use", 1);
        var taking = new DragonBonesAnimationController.AnimationSettings("pick_up", 1, 
            doOnEnd: () => parametersBool[4].Value = false);

        var defaultFade = new DragonBonesAnimationController.FadeSettings(0.3f, 0.3f);

        idle.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(walk, false, defaultFade,
                () => parametersBool[0].Value),
            new DragonBonesAnimationController.Transition(jump, false, null,
                () => parametersBool[1].Value),
            new DragonBonesAnimationController.Transition(falling, false, defaultFade,
                () => parametersBool[2].Value)
        };
        walk.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(idle, false, defaultFade,
                () => parametersBool[0].Value == false),
            new DragonBonesAnimationController.Transition(jump, false, null,
                () => parametersBool[1].Value),
            new DragonBonesAnimationController.Transition(falling, false, defaultFade,
                () => parametersBool[2].Value)
        };
        jump.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(falling, false, defaultFade,
            () => parametersBool[1].Value == false),
            new DragonBonesAnimationController.Transition(falling, true, defaultFade)
        };
        falling.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(landing, false, null, 
            () => parametersBool[2].Value == false)
        };
        landing.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(jump, false, null,
                () => parametersBool[1].Value),
            new DragonBonesAnimationController.Transition(idle, true, defaultFade,
                () => parametersBool[0].Value == false),
            new DragonBonesAnimationController.Transition(walk, true, defaultFade,
                () => parametersBool[0].Value)
        };

        layer2_null.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(interaction, false,
                new DragonBonesAnimationController.FadeSettings(0, 0.3f),
                () => parametersBool[3].Value),
            new DragonBonesAnimationController.Transition(taking, false, 
            new DragonBonesAnimationController.FadeSettings(0, 0.2f), 
            () => parametersBool[4].Value)
        };
        interaction.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(layer2_null, true,
                new DragonBonesAnimationController.FadeSettings(0.3f, 0), 
                () => parametersBool[3].Value == false)
        };
        taking.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(layer2_null, true,
                new DragonBonesAnimationController.FadeSettings(0.2f, 0))
        };

        animationController.Init(parametersBool, new List<DragonBonesAnimationController.AnimationsLayer>()
        {
            new DragonBonesAnimationController.AnimationsLayer(animationController.ArmatureComponent,
                parametersBool, new List<DragonBonesAnimationController.AnimationSettings>()
                { idle, walk, jump, falling, landing}, idle, 0
            ),
            new DragonBonesAnimationController.AnimationsLayer(animationController.ArmatureComponent,
                parametersBool, new List<DragonBonesAnimationController.AnimationSettings>()
                { layer2_null, interaction, taking}, layer2_null, 1
            ),
        });
    }
}
