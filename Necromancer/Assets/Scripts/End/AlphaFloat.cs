using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaFloat : MonoBehaviour
{
    [SerializeField]
    float delta = .25f;
    [SerializeField]
    float duration = .75f;
    float max;
    Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }
    private void Start()
    {
        max = img.color.a;

        StartCoroutine(AlphaChanger());
    }

    IEnumerator AlphaChanger()
    {
        while (true)
        {
            if (img.color.a > max - delta)
            {
                float t = 0;
                while ((t += Time.deltaTime) < duration)
                {
                    img.color = new Color(1, 1, 1, Mathf.Lerp(max, max - 2 * delta, t / duration));

                    yield return null;
                }
            }
            else 
            {
                float t = 0;
                while ((t += Time.deltaTime) < duration)
                {
                    img.color = new Color(1, 1, 1, Mathf.Lerp(max - 2 * delta, max, t / duration));

                    yield return null;
                }
            }
        }
    }
}
