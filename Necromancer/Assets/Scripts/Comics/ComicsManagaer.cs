using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicsManagaer : MonoBehaviour
{
    [SerializeField]
    List<ComicsBlock> comicsBlocks;
    [SerializeField]
    AudioSource audioSource;

    int currentImage = 0;

    Coroutine apCor;

    private void Start()
    {
        apCor = StartCoroutine(Appearance());
    }

    IEnumerator Appearance()
    {
        float timer = 0;

        if (comicsBlocks[currentImage].clip != null)
        {
            if (audioSource != null)
            {
                audioSource.clip = comicsBlocks[currentImage].clip;
                audioSource.Play();
            }
        }

        while ((timer += Time.deltaTime) < comicsBlocks[currentImage].beforeAppearanceTime && !Input.anyKeyDown)
        {
            yield return null;
        }

        if (comicsBlocks[currentImage].duration != 0 && Input.anyKeyDown)
            yield return null;

        timer = 0;
        float from = comicsBlocks[currentImage].co[0].GetAlpha(); ;
        float fromAud = audioSource?.volume ?? 0;
        float to = comicsBlocks[currentImage].direction == 1 ? 1 : 0;

        while (((timer += Time.deltaTime) < comicsBlocks[currentImage].duration) && !Input.anyKeyDown)
        {
            for (int i = 0; i < comicsBlocks[currentImage].co.Count; i++)
            {
                comicsBlocks[currentImage].co[i].SetAlpha(Mathf.Lerp(from, to, timer / comicsBlocks[currentImage].duration));
            }

            if (comicsBlocks[currentImage].audioFade && comicsBlocks[currentImage].fadePart == FadePart.Duration)
            {
                if (audioSource != null)
                    audioSource.volume = Mathf.Lerp(fromAud, 0f, timer / comicsBlocks[currentImage].duration);
            }

            yield return null;
        }

        if (comicsBlocks[currentImage].audioFade && comicsBlocks[currentImage].fadePart == FadePart.Duration)
        {
            if (audioSource != null)
                audioSource.volume = 0;
        }

        if (comicsBlocks[currentImage].duration != 0 && Input.anyKeyDown)
            yield return null;

        for (int i = 0; i < comicsBlocks[currentImage].co.Count; i++)
        {
            comicsBlocks[currentImage].co[i].SetAlpha(comicsBlocks[currentImage].direction == 1 ? 1 : 0);
        }

        timer = 0;

        
        while (((timer += Time.deltaTime) < comicsBlocks[currentImage].afterAppearanceTime && !Input.anyKeyDown) || comicsBlocks[currentImage].isWaitingPlayerClick == true ? !Input.anyKeyDown : false)
        {
            if (comicsBlocks[currentImage].audioFade && comicsBlocks[currentImage].fadePart == FadePart.After)
            {
                if (audioSource != null)
                    audioSource.volume = Mathf.Lerp(fromAud, 0f, timer / comicsBlocks[currentImage].afterAppearanceTime);
            }

            yield return null;
        }

        if (comicsBlocks[currentImage].audioFade && comicsBlocks[currentImage].fadePart == FadePart.After)
        {
            if (audioSource != null)
                audioSource.volume = 0;
        }

        if (comicsBlocks[currentImage].duration != 0 && Input.anyKeyDown)
            yield return null;

        if (++currentImage < comicsBlocks.Count)
        {
            apCor = StartCoroutine(Appearance());
        }
        else
        {
            NextScene.StartLoading();
        }
    }
}

[System.Serializable]
public class ComicsBlock
{
    public List<ComicsObject> co;
    public float beforeAppearanceTime = 0f;
    public float afterAppearanceTime = 0f;
    public bool isWaitingPlayerClick = false;
    public float duration = .5f;
    public int direction = 1;
    public bool audioFade = false;
    public FadePart fadePart = FadePart.Duration;
    public AudioClip clip;
}

public enum FadePart
{
    Duration = 0,
    After = 1
}
