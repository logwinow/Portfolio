using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondSuitAnimationController : PlayerAnimationController
{
    private void Awake()
    {
        var parametersBool = new List<DragonBonesAnimationController.Parameter<bool>>()
        {
            new DragonBonesAnimationController.Parameter<bool>("IsMoving"), //0
            new DragonBonesAnimationController.Parameter<bool>("IsFlying"), //1
            new DragonBonesAnimationController.Parameter<bool>("IsFalling"), //2
            new DragonBonesAnimationController.Parameter<bool>("IsTaking"), //3
            new DragonBonesAnimationController.Parameter<bool>("IsDrilling"), //4
            new DragonBonesAnimationController.Parameter<bool>("IsAttacked"), //5 
            new DragonBonesAnimationController.Parameter<bool>("IsDead"), //6
            new DragonBonesAnimationController.Parameter<bool>("IsSuffocated") //7
        };
        
        var idle = new DragonBonesAnimationController.AnimationSettings("Idle", 0);
        var walk = new DragonBonesAnimationController.AnimationSettings("Walk", 0);
        var flying = new DragonBonesAnimationController.AnimationSettings("jetpak_process", 
            0, doOnEnd:
            delegate
            {
                parametersBool[2].Value = true;
            }
        );
        var falling = new DragonBonesAnimationController.AnimationSettings("Fall_process", 0,
            doOnEnd: () => parametersBool[2].Value = false);
        var landing = new DragonBonesAnimationController.AnimationSettings("Fall_end", 1);
        var attacked = new DragonBonesAnimationController.AnimationSettings("damage", 1,
            doOnEnd: () => parametersBool[5].Value = false);
        var dead = new DragonBonesAnimationController.AnimationSettings("Dead", 1);

        var layer2_nullAnim = new DragonBonesAnimationController.AnimationSettings(
            );
        var take = new DragonBonesAnimationController.AnimationSettings("pick_up", 1, 
            doOnEnd: () => parametersBool[3].Value = false);
        var drill = new DragonBonesAnimationController.AnimationSettings("Drill_action", 0);
        var suffocated = new DragonBonesAnimationController.AnimationSettings("no_oxygen", 1);
        
        DragonBonesAnimationController.FadeSettings defaultFade = 
            new DragonBonesAnimationController.FadeSettings(0.3f, 0.3f);
        DragonBonesAnimationController.FadeSettings attackedFade = 
            new DragonBonesAnimationController.FadeSettings(0.2f, 0.2f);

        idle.transitions = new[] {
            new DragonBonesAnimationController.Transition(dead, false, defaultFade, 
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(walk, false, defaultFade,
            () => parametersBool[0].Value == true),
            new DragonBonesAnimationController.Transition(flying, false, defaultFade,
                () => parametersBool[1].Value == true),
            new DragonBonesAnimationController.Transition(falling, false, defaultFade,
                () => parametersBool[2].Value == true),
            new DragonBonesAnimationController.Transition(attacked, false,
            attackedFade, () => parametersBool[5].Value == true) 
        };
        walk.transitions = new[] {
            new DragonBonesAnimationController.Transition(dead, false, defaultFade, 
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(idle, false, defaultFade,
                () => !parametersBool[0].Value),
            new DragonBonesAnimationController.Transition(flying, false, defaultFade,
                () => parametersBool[1].Value == true),
            new DragonBonesAnimationController.Transition(falling, false, defaultFade,
                () => parametersBool[2].Value == true),
            new DragonBonesAnimationController.Transition(attacked, false,
                attackedFade, () => parametersBool[5].Value == true)
        };
        flying.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(dead, false, defaultFade, 
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(attacked, false,
            attackedFade, () => parametersBool[5].Value == true),
            new DragonBonesAnimationController.Transition(falling, false, defaultFade,
                () => parametersBool[1].Value == false)
        };
        falling.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(dead, false, defaultFade, 
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(attacked, false,
            attackedFade, () => parametersBool[5].Value == true),
            new DragonBonesAnimationController.Transition(landing, false, 
            null, () => parametersBool[2].Value == false),
            new DragonBonesAnimationController.Transition(flying, false, defaultFade,
                () => parametersBool[1].Value)
        };
        landing.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(dead, false, defaultFade, 
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(attacked, false,
            attackedFade, () => parametersBool[5].Value == true),
            new DragonBonesAnimationController.Transition(flying, false, defaultFade,
                () => parametersBool[1].Value == true),
            new DragonBonesAnimationController.Transition(idle, true, 
                defaultFade, () => parametersBool[0].Value == false),
            new DragonBonesAnimationController.Transition(walk, true, 
                defaultFade, () => parametersBool[0].Value == true)
        };
        attacked.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(dead, false, 
                new DragonBonesAnimationController.FadeSettings(0.2f, 0.1f), 
            () => parametersBool[6].Value == true),
            new DragonBonesAnimationController.Transition(idle, true, defaultFade,
                () => parametersBool[0].Value == false),
            new DragonBonesAnimationController.Transition(walk, true, defaultFade,
                () => parametersBool[0].Value == true)
        };
        dead.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(suffocated, false, null,
                () => parametersBool[7].Value)
        };

        layer2_nullAnim.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(take, false, 
            new DragonBonesAnimationController.FadeSettings(0, 0.1f), 
            () => parametersBool[3].Value == true),
            new DragonBonesAnimationController.Transition(drill, false, 
            new DragonBonesAnimationController.FadeSettings(0, 0.3f), 
            () => parametersBool[4].Value == true)
        };
        take.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(layer2_nullAnim, true, 
            new DragonBonesAnimationController.FadeSettings(0.3f, 0)),
            new DragonBonesAnimationController.Transition(drill, false, defaultFade, 
                () => parametersBool[4].Value == true)
        };
        drill.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(layer2_nullAnim, false, 
                new DragonBonesAnimationController.FadeSettings(0.3f, 0),
                () => parametersBool[4].Value == false)
        };

        // Layers
        List<DragonBonesAnimationController.AnimationsLayer> animLayers = 
            new List<DragonBonesAnimationController.AnimationsLayer>()
        {
            new DragonBonesAnimationController.AnimationsLayer(animationController.ArmatureComponent, parametersBool,
                new List<DragonBonesAnimationController.AnimationSettings>() 
                { idle, walk, flying, falling, landing, attacked },
                idle, 0),
            new DragonBonesAnimationController.AnimationsLayer(animationController.ArmatureComponent, parametersBool,
                new List<DragonBonesAnimationController.AnimationSettings>() 
                {layer2_nullAnim, take, drill }, layer2_nullAnim, 1)
        };
        
        DBAnimationController.Init(parametersBool, animLayers);
    }
}
