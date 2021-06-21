using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecieveObjective : MonoBehaviour
{
    [SerializeField]
    Image objectiveImage;
    [SerializeField]
    Objective[] objectives;
    int currentObjective = 0;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip gaveObjectiveSound;

    public void SetObjective(int num)
    {
        if (!objectives[num].activated)
        {
            audioSource.PlayOneShot(gaveObjectiveSound);
            GameManager.Instance.teachWinodw.gameObject.SetActive(false);
            currentObjective = num;
            objectiveImage.transform.parent.gameObject.SetActive(true);
            if (Language.laguage == Language.LaguageType.Russian)
                objectiveImage.sprite = objectives[num].rusSprite;
            else // eng
                objectiveImage.sprite = objectives[num].engSprite;

            GameManager.Instance.IsPaused = true;

            objectives[num].activated = true;

            StartCoroutine(ClickWaiting());
        }
    }

    public void SetNextObjective()
    {
        currentObjective++;
        SetObjective(currentObjective);
    }

    void CloseObjective()
    {
        objectiveImage.transform.parent.gameObject.SetActive(false);

        GameManager.Instance.IsPaused = false;
    }

    IEnumerator ClickWaiting()
    {
        yield return null;

        while(!Input.GetKeyDown(KeyCode.E) && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        CloseObjective();
    }
}

[System.Serializable]
public class Objective
{
    public Sprite rusSprite;
    public Sprite engSprite;
    public bool activated = false;
}
