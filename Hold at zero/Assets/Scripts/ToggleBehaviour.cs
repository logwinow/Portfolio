using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBehaviour : MonoBehaviour
{
    [SerializeField]
    ToggleType type;
    [SerializeField]
    Sprite pressedToggle;
    [SerializeField]
    Sprite defaultToggle;
    [SerializeField]
    AudioClip switchClip;

    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }
    public void SwitchToggleStatus()
    {
        SceneController.globalAudio.PlayOneShot(switchClip);
        Messenger<ToggleType>.Broadcast(MyGameEvents.TOGGLE_PRESSED, type);

        if (img.sprite == pressedToggle)
            img.sprite = defaultToggle;
        else
            img.sprite = pressedToggle;
    }
}
