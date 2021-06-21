using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuskFloat : MonoBehaviour
{
    Vector3 startPos;
    RectTransform rectTrans;
    [SerializeField]
    float delta = 10f;
    [SerializeField]
    float duration = 1f;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    private void Start()
    {
        startPos = rectTrans.position;

        StartCoroutine(PositionChanger());
    }

    IEnumerator PositionChanger()
    {
        while (true)
        {
            if (rectTrans.position.y > startPos.y - delta)
            {
                float t = 0;
                while ((t += Time.deltaTime) < duration)
                {
                    rectTrans.position = new Vector3(startPos.x,  Mathf.Lerp(startPos.y, startPos.y - 2 * delta, t / duration), startPos.z);

                    yield return null;
                }
            }
            else
            {
                float t = 0;
                while ((t += Time.deltaTime) < duration)
                {
                    rectTrans.position = new Vector3(startPos.x, Mathf.Lerp(startPos.y - 2 * delta, startPos.y, t / duration), startPos.z);

                    yield return null;
                }
            }
        }
    }
}
