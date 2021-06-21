using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndController : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    GameObject goodBG;
    [SerializeField]
    AudioClip winAudioClip;
    [SerializeField]
    GameObject evilBG;
    [SerializeField]
    AudioClip defeatAudioClip;
    [SerializeField]
    Text winText;
    [SerializeField]
    Text defeatText;

    [SerializeField]
    List<LanguageNode> winCause;
    [SerializeField]
    List<LanguageNode> defeatCause;

    int result;
    int cause;

    private void Start()
    {
        result = PlayerPrefs.GetInt("Result", 0);
        cause = PlayerPrefs.GetInt("Cause", 0);

        if (result == 0)
        {
            evilBG.SetActive(true);

            defeatText.text = defeatCause[cause].GetText();

            audioSource.clip = defeatAudioClip;
        }
        else // 1
        {
            goodBG.SetActive(true);

            winText.text = winCause[cause].GetText();

            audioSource.clip = winAudioClip;
        }

        audioSource.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.Application.Quit();
        }
    }
}
