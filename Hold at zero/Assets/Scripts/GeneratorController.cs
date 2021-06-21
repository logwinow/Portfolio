using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum GeneratorTemperatureType
{
    Normal = 0,
    High = 1,
    Low = -1
}
public class GeneratorController : MonoBehaviour
{
    [SerializeField]
    Sprite tableBlue;
    [SerializeField]
    Sprite tableRed;
    [SerializeField]
    Sprite tableNormal;
    [SerializeField]
    Image tableImage;

    [SerializeField]
    AudioClip normalClip;
    [SerializeField]
    AudioClip sparkClip;

    AudioSource audioSource;
    Animator anim;

    GeneratorTemperatureType curType = GeneratorTemperatureType.Normal;
    void Awake()
    {
        Messenger<GeneratorTemperatureType>.AddListener(MyGameEvents.CHANGE_GENERATOR_VIEW, ChangeGeneratorView);

        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnDestroy()
    {
        Messenger<GeneratorTemperatureType>.RemoveListener(MyGameEvents.CHANGE_GENERATOR_VIEW, ChangeGeneratorView);
    }

    void ChangeGeneratorView(GeneratorTemperatureType type)
    {
        anim.SetInteger("temperature", (int)type);

        if (curType != type)
        {
            switch (type)
            {
                case GeneratorTemperatureType.High:
                    tableImage.sprite = tableRed;
                    break;
                case GeneratorTemperatureType.Low:
                    tableImage.sprite = tableBlue;
                    break;
                case GeneratorTemperatureType.Normal:
                    tableImage.sprite = tableNormal;
                    break;
            }

            audioSource.PlayOneShot(sparkClip);
        }

        curType = type;
    }

    void SetClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
