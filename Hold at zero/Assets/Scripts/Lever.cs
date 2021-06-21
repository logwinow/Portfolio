using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ControlButton
{
    [SerializeField]
    ControlFeature temperature;
    [SerializeField]
    ControlFeature pressure;
    [SerializeField]
    ControlFeature waste;
    [SerializeField]
    ControlFeature antifreeze;
    [SerializeField]
    AudioClip pull;
    [SerializeField]
    AudioClip wring;

    [SerializeField]
    AudioClip onDecreaseClip;

    AudioSource audioSource;

    bool mainButtonPressed = false;
    Animator anim;
    protected override void Awake()
    {
        base.Awake();
        Messenger<bool>.AddListener(MyGameEvents.MAIN_BUTTON_PRESSED, MainButtonStatus);
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnDestroy()
    {
        Messenger<bool>.RemoveListener(MyGameEvents.MAIN_BUTTON_PRESSED, MainButtonStatus);
    }
    protected override void ChangeCurrentFeature()
    {
        if (!sceneController.BrakingToggle && !sceneController.ControlToggle && !sceneController.PlugOFFToggle && !sceneController.TubeToggle)
        {
            pressure.feature.AddValue(-pressure.DecreaseValue);

            if (!audioSource.isPlaying)
            {
                audioSource.clip = onDecreaseClip;
                audioSource.Play();
            }
        }
        else if (sceneController.TubeToggle && sceneController.PlugOFFToggle)
        {
            if (!mainButtonPressed)
            {
                temperature.feature.AddValue(-temperature.DecreaseValue);
                if (temperature.feature.CurrentValue != 0)
                    antifreeze.feature.AddValue(antifreeze.DecreaseValue);

            }
            else
            {
                temperature.feature.AddValue(temperature.DecreaseValue);
            }

            if (!audioSource.isPlaying)
            {
                audioSource.clip = onDecreaseClip;
                audioSource.Play();
            }
        }
        else if (sceneController.ControlToggle && sceneController.BrakingToggle && !sceneController.TubeToggle)
        {
            waste.feature.AddValue(-waste.DecreaseValue);

            if (!audioSource.isPlaying)
            {
                audioSource.clip = onDecreaseClip;
                audioSource.Play();
            }
        }
    }

    protected override void InitializeKeyPressedEvent()
    {
        keyPressedEvent = MyGameEvents.LEVER_PULLED;
    }

    void MainButtonStatus(bool value)
    {
        mainButtonPressed = value;
    }

    protected override void EventAfterPressed()
    {
        base.EventAfterPressed();
        anim.SetBool("pressed", true);
        SceneController.globalAudio.PlayOneShot(pull);
    }

    protected override void EventAfterWring()
    {
        base.EventAfterWring();
        anim.SetBool("pressed", false);
        SceneController.globalAudio.PlayOneShot(wring);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
