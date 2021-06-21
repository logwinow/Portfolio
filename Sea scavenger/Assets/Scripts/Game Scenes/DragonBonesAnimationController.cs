using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using Custom.SmartCoroutines;
using System;

[RequireComponent(typeof(UnityArmatureComponent))]
public partial class DragonBonesAnimationController : MonoBehaviour // Unity functions
{
    private void Awake()
    {
        armComp = GetComponent<UnityArmatureComponent>();
    }

    internal void Init(List<Parameter<bool>> parametersBool, List<AnimationsLayer> animationsLayers)
    {
        this.parametersBool = parametersBool;
        this.animLayers = animationsLayers;
    }

    private void Start()
    {
        StartLayers();
    }

    private void Update()
    {
        UpdateLayers();
    }
}

public partial class DragonBonesAnimationController // LAYER
{
    internal class AnimationsLayer
    {
        public AnimationsLayer(UnityArmatureComponent armatureComponent, List<Parameter<bool>> parametersBool,
            List<AnimationSettings> animations, AnimationSettings defaultAnimation, int layer)
        {
            armComp = armatureComponent;
            this.layer = layer;
            this.parametersBool = parametersBool;
            this.animations = animations;
            this.defaultAnimation = defaultAnimation;

            current = new AnimationCombine();
        }

        private UnityArmatureComponent armComp;
        private List<AnimationSettings> animations;
        private AnimationSettings defaultAnimation;
        private AnimationCombine current;
        private List<Parameter<bool>> parametersBool;
        public readonly int layer;

        public void StartAnimations()
        {
            current.state = Play(defaultAnimation, null);
            current.settings = defaultAnimation;
        }

        public void UpdateAnimations()
        {
            Transition c = current.settings.GetFirstAvailableTransitionOrNull();
            
            if (c == null)
            {
                return;
            }

            if (c.hasExitTime)
            {
                if (current.state.isCompleted)
                {
                    GoFromCurrentByTransition(c);
                }
            }
            else
            {
                GoFromCurrentByTransition(c);
            }
        }

        private void GoFromCurrentByTransition(Transition transition)
        {
            current.settings.DoOnEnd();

            if (current.settings.name != null)
            {
                if (transition.fadeSettings != null)
                    current.state.FadeOut(transition.fadeSettings.fadeOutTime);
                else
                    current.state.FadeOut(0);
            }

            current.state = Play(transition.nextAnim, transition.fadeSettings);
            current.settings = transition.nextAnim;

            current.settings.DoOnStart();
        }

        private DragonBones.AnimationState Play(AnimationSettings animSettings, FadeSettings fadeSettings)
        {
            if (animSettings.name == null)
                return null;

            if (fadeSettings == null)
            {
                return armComp.animation.FadeIn(animSettings.name, 0,
                    animSettings.playTimes, layer);
            }
            else
            {
                return armComp.animation.FadeIn(animSettings.name, fadeSettings.fadeInTime,
                    animSettings.playTimes, layer);
            }
        }
    }
}

public partial class DragonBonesAnimationController
{
    private UnityArmatureComponent armComp;
    private List<AnimationsLayer> animLayers;
    private List<Parameter<bool>> parametersBool;

    public UnityArmatureComponent ArmatureComponent => armComp;

    private void StartLayers()
    {
        animLayers.ForEach(a => a.StartAnimations());
    }

    private void UpdateLayers()
    {
        animLayers.ForEach(a => a.UpdateAnimations());
    }

    public void SetBool(string title, bool value)
    {
        parametersBool.Find(p => p.Title == title).Value = value;
    }

    public bool GetBool(string title)
    {
        return parametersBool.Find(p => p.Title == title).Value;
    }

    public void FlipX(bool X)
    {
        armComp.armature.flipX = X;
    }

    public void FlipY(bool Y)
    {
        armComp.armature.flipY = Y;
    }
}

public partial class DragonBonesAnimationController
{
    internal delegate bool Condition();

    internal class AnimationSettings
    {
        public AnimationSettings(string name = null, int playTimes = 0, 
            Action doOnStart = null, Action doOnEnd = null, params Transition[] transitions)
        {
            this.name = name;
            this.playTimes = playTimes;
            this.transitions = transitions;
            this.doOnStart = doOnStart;
            this.doOnEnd = doOnEnd;
        }

        public readonly string name;
        public readonly int playTimes;
        internal Transition[] transitions;
        public readonly Action doOnStart;
        public readonly Action doOnEnd;

        public Transition GetFirstAvailableTransitionOrNull()
        {
            foreach (var c in transitions)
            {
                if (c.Check())
                    return c;
            }

            return null;
        }

        public void DoOnStart()
        {
            doOnStart?.Invoke();
        }

        public void DoOnEnd()
        {
            doOnEnd?.Invoke();
        }
    }

    internal class FadeSettings
    {
        public FadeSettings(float fadeOutTime = 0, float fadeInTime = 0)
        {
            this.fadeOutTime = fadeOutTime;
            this.fadeInTime = fadeInTime;
        }

        public readonly float fadeOutTime;
        public readonly float fadeInTime;
    }

    internal class Transition
    {
        public Transition(AnimationSettings nextAnim, bool hasExitTime = false, 
            FadeSettings fadeSettings = null, params Condition[] conditions)
        {
            this.nextAnim = nextAnim;
            this.fadeSettings = fadeSettings;
            this.conditions = conditions;
            this.hasExitTime = hasExitTime;
        }

        public readonly AnimationSettings nextAnim;
        public readonly FadeSettings fadeSettings;
        public readonly bool hasExitTime;
        private Condition[] conditions;
        

        public bool Check()
        {
            foreach (var c in conditions)
            {
                if (c.Invoke() == false)
                    return false;
            }

            return true;
        }
    }

    internal class Parameter<TValue>
    {
        public Parameter(string title, TValue value = default(TValue))
        {
            this.title = title;
            this.value = value;
        }

        private string title;
        private TValue value;

        public string Title => title;
        public TValue Value
        {
            get => value;
            set => this.value = value;
        }
    }

    private class AnimationCombine
    {
        public DragonBones.AnimationState state;
        public AnimationSettings settings;
    }
}
