using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Patterns;

public class PlayerFirstSuitAnimationController : PlayerAnimationController
{
    [SerializeField]
    private GameObject drill;

    private void Awake()
    {
        List<DragonBonesAnimationController.Parameter<bool>> parametersBool = 
            new List<DragonBonesAnimationController.Parameter<bool>>();

        // Parameters
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsMoving", false)); // 0
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsJumping", false)); // 1
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsTaking", false)); // 2
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsDrilling", false)); // 3
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsFalling", false)); // 4
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsAttacked")); // 5
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsDead")); // 6
        parametersBool.Add(new DragonBonesAnimationController.Parameter<bool>("IsSuffocated")); // 7

        // Animations - Initialize
        var idle = new DragonBonesAnimationController.AnimationSettings("Idle", 0);
        var walk = new DragonBonesAnimationController.AnimationSettings("Walk", 0);
        var jumping = new DragonBonesAnimationController.AnimationSettings("UP_v2", 1, doOnEnd:
            delegate
            {
                parametersBool[1].Value = false;
                parametersBool[4].Value = true;
            }
            );
        var falling = new DragonBonesAnimationController.AnimationSettings("fall_process", 0);
        var landing = new DragonBonesAnimationController.AnimationSettings("fall_end", 1);
        var attacked = new DragonBonesAnimationController.AnimationSettings("damage", 1,
            doOnEnd: () => parametersBool[5].Value = false);
        var dead = new DragonBonesAnimationController.AnimationSettings("Dead", 1);

        var layer2_nullAnim = new DragonBonesAnimationController.AnimationSettings(
            doOnStart: () => parametersBool[2].Value = false);
        var take = new DragonBonesAnimationController.AnimationSettings("pick_up", 1);
        var drill = new DragonBonesAnimationController.AnimationSettings("Drill_action", 0);
        var suffocated = new DragonBonesAnimationController.AnimationSettings("no_oxygen", 1);

        // Animations - Conditions
        DragonBonesAnimationController.FadeSettings fadeSettings = 
            new DragonBonesAnimationController.FadeSettings(0.2f, 0.2f);
        DragonBonesAnimationController.FadeSettings pushFadeSet = 
            new DragonBonesAnimationController.FadeSettings(0.5f, 0.3f);
        DragonBonesAnimationController.FadeSettings toDeadFade  = 
            new DragonBonesAnimationController.FadeSettings(0.2f, 0.4f);

        idle.transitions = new DragonBonesAnimationController.Transition[] {
            new DragonBonesAnimationController.Transition(walk, false, fadeSettings,
            () => parametersBool[0].Value == true),
            new DragonBonesAnimationController.Transition(jumping, false,
            new DragonBonesAnimationController.FadeSettings(0.1f, 0.1f),
                () => parametersBool[1].Value == true),
            new DragonBonesAnimationController.Transition(falling, false,
            new DragonBonesAnimationController.FadeSettings(0.3f, 0.3f),
                () => parametersBool[4].Value == true),
            new DragonBonesAnimationController.Transition(attacked, false,
            pushFadeSet, () => parametersBool[5].Value == true),
            new DragonBonesAnimationController.Transition(dead, false, toDeadFade,
                () => parametersBool[6].Value)
        };
        walk.transitions = new DragonBonesAnimationController.Transition[] {
            new DragonBonesAnimationController.Transition(idle, false, fadeSettings, 
            () => parametersBool[0].Value == false),
            new DragonBonesAnimationController.Transition(jumping, false, 
            new DragonBonesAnimationController.FadeSettings(0.1f, 0.1f),
                () => parametersBool[1].Value == true),
            new DragonBonesAnimationController.Transition(falling, false, 
            new DragonBonesAnimationController.FadeSettings(0.3f, 0.3f),
                () => parametersBool[4].Value == true),
                new DragonBonesAnimationController.Transition(attacked, false,
            pushFadeSet, () => parametersBool[5].Value == true),
                new DragonBonesAnimationController.Transition(dead, false, toDeadFade,
                    () => parametersBool[6].Value)
        };
        jumping.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(dead, false, toDeadFade,
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(attacked, false,
            pushFadeSet, () => parametersBool[5].Value == true),
            new DragonBonesAnimationController.Transition(falling, false,
                new DragonBonesAnimationController.FadeSettings(0.5f, 0.5f),
                () => parametersBool[1].Value == false),
            new DragonBonesAnimationController.Transition(falling, true, 
                new DragonBonesAnimationController.FadeSettings(0.5f, 0.5f))
        };
        falling.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(dead, false, toDeadFade,
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(attacked, false,
            pushFadeSet, () => parametersBool[5].Value == true),
            new DragonBonesAnimationController.Transition(landing, false, 
            new DragonBonesAnimationController.FadeSettings(0.2f, 0.1f), () => parametersBool[4].Value == false)
        };
        landing.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(dead, false, toDeadFade,
                () => parametersBool[6].Value),
            new DragonBonesAnimationController.Transition(attacked, false,
            pushFadeSet, () => parametersBool[5].Value == true),
            new DragonBonesAnimationController.Transition(jumping, false,
            new DragonBonesAnimationController.FadeSettings(0.1f, 0.1f),
                () => parametersBool[1].Value == true),
            new DragonBonesAnimationController.Transition(idle, true, 
            new DragonBonesAnimationController.FadeSettings(0.3f, 0.3f), () => parametersBool[0].Value == false),
            new DragonBonesAnimationController.Transition(walk, true, 
            new DragonBonesAnimationController.FadeSettings(0.3f, 0.3f), () => parametersBool[0].Value == true)
        };
        attacked.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(dead, false, 
            new DragonBonesAnimationController.FadeSettings(0.2f, 0.1f), 
            () => parametersBool[6].Value == true),
            new DragonBonesAnimationController.Transition(idle, true, pushFadeSet,
                () => parametersBool[0].Value == false),
            new DragonBonesAnimationController.Transition(walk, true, pushFadeSet,
                () => parametersBool[0].Value == true)
        };

        layer2_nullAnim.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(take, false, 
            new DragonBonesAnimationController.FadeSettings(0, 0f), () => parametersBool[2].Value == true),
            new DragonBonesAnimationController.Transition(drill, false, 
            new DragonBonesAnimationController.FadeSettings(0, 0.2f), () => parametersBool[3].Value == true)
        };
        take.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(layer2_nullAnim, true, 
            new DragonBonesAnimationController.FadeSettings(0.3f, 0)),
            new DragonBonesAnimationController.Transition(drill, false, new DragonBonesAnimationController.FadeSettings(0.3f, 0.3f), () => parametersBool[3].Value == true)
        };
        drill.transitions = new DragonBonesAnimationController.Transition[]
        {
            new DragonBonesAnimationController.Transition(layer2_nullAnim, false, 
            new DragonBonesAnimationController.FadeSettings(0.3f, 0),
                () => parametersBool[3].Value == false)
        };
        dead.transitions = new[]
        {
            new DragonBonesAnimationController.Transition(suffocated, false, null,
                () => parametersBool[7].Value),
        };

        // Layers
        List<DragonBonesAnimationController.AnimationsLayer> animLayers = 
            new List<DragonBonesAnimationController.AnimationsLayer>()
        {
            new DragonBonesAnimationController.AnimationsLayer(animationController.ArmatureComponent, parametersBool,
                new List<DragonBonesAnimationController.AnimationSettings>() 
                {idle, walk, jumping, falling, landing, attacked },
                idle, 0),
            new DragonBonesAnimationController.AnimationsLayer(animationController.ArmatureComponent, parametersBool,
                new List<DragonBonesAnimationController.AnimationSettings>() 
                {layer2_nullAnim, take, drill }, layer2_nullAnim, 1)
        };

        animationController.Init(parametersBool, animLayers);
    }

    public void SetDrillState(bool value)
    {
        drill.SetActive(value);
        animationController.SetBool("IsDrilling", value);
    }
}
