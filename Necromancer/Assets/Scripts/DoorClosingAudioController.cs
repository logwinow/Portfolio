using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClosingAudioController : MonoBehaviour
{
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(WaitingEndOfSound());
    }

    IEnumerator WaitingEndOfSound()
    {
        while(audioSource.isPlaying)
        {
            yield return null;
        }

        GameManager.Instance.ObjectiveController.SetObjective(0);
    }
}
