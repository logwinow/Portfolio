using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverloadFeature : Feature
{
    [SerializeField]
    Sprite attention;
    [SerializeField]
    Sprite normalState;
    [SerializeField]
    float timeDelay = .2f;
    [SerializeField]
    Image backgroundImage;
    [SerializeField]
    float constantDecrease = .05f;
    [SerializeField]
    AudioClip attentionClip;

    AudioSource audioSource;

    OrdinaryCoroutine delay;

    bool gameEnd = false;
    protected override void Awake()
    {
        base.Awake();
        Messenger.AddListener(MyGameEvents.ATTENTION, OnOverloadBarIncrease);
        delay = new OrdinaryCoroutine(this, Delay);
        audioSource = GetComponent<AudioSource>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Messenger.RemoveListener(MyGameEvents.ATTENTION, OnOverloadBarIncrease);
    }

    protected override void Update()
    {
        base.Update();

        if (controlSlider.value == controlSlider.maxValue)
        {
            if (!gameEnd)
            {
                Messenger<bool>.Broadcast(MyGameEvents.GAME_END, false);
                gameEnd = true;
            }
        }

        if (controlSlider.value > 0)
            controlSlider.value -= constantDecrease * Time.deltaTime;
    }

    void OnOverloadBarIncrease()
    {
        if (!delay.IsWorking)
            delay.Start();
    }

    IEnumerator Delay()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = attentionClip;
            audioSource.Play();
        }

        backgroundImage.sprite = attention;

        yield return new WaitForSeconds(timeDelay);

        backgroundImage.sprite = normalState;

        yield return new WaitForSeconds(timeDelay);
    }
}
