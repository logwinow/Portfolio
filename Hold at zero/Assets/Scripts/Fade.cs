using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField]
    Manual manual;
    [SerializeField]
    float riseSpeed = .5f;
    [SerializeField]
    Text counter;
    [SerializeField]
    float alphaNumberDecrease = .5f;
    [SerializeField]
    bool isTutorial = false;
    [SerializeField]
    float delayTime = 0f;
    [SerializeField]
    ChangableColorItem[] toFadeImages;
    [SerializeField]
    ChangableColorItem[] toRiseImages;
    [SerializeField]
    float fadeSpeed = 4f;

    public static bool IsTutorial;

    Image img;
    enum ApparanceDirection
    {
        Fade = -1,
        Rise = 1
    }

    void Awake()
    {
        Messenger<Action>.AddListener(MyMenuEvents.CLOSE_DOORS, StartFading);

        img = GetComponent<Image>();
        IsTutorial = isTutorial;
    }

    void OnDestroy()
    {
        Messenger<Action>.RemoveListener(MyMenuEvents.CLOSE_DOORS, StartFading);
    }

    void Start()
    {
        StartCoroutine(Appearance(ApparanceDirection.Fade, new ChangableColorItem[] { new ChangableColorItem {value = img, customSpeed = .5f}}, () => { 
            if (!isTutorial)
                StartCoroutine(Count());
            else
            
                StartCoroutine(Delay());
            }
            ));

        StartCoroutine(Appearance(ApparanceDirection.Rise, toRiseImages));
    }

    delegate void ColorChange(ChangableColorItem i);

    IEnumerator Appearance(ApparanceDirection dir, ChangableColorItem[] images, Action callback = null)
    {
        if (images != null)
        {
            int numReady = 0;

            ColorChange colorChanger = delegate(ChangableColorItem i)
            {
                Color tmp = i.value.color;
                tmp.a += (int)dir * (i.customSpeed == 0 ? (dir == ApparanceDirection.Rise ? riseSpeed : fadeSpeed) : i.customSpeed) * Time.deltaTime;
                i.value.color = tmp;
            };

            while(numReady < images.Length)
            {
                numReady = 0;

                foreach(ChangableColorItem i in images)
                {
                    if (dir == ApparanceDirection.Rise)
                    {
                        if (i.value.color.a < i.maxAlpha)
                            colorChanger(i);
                        else
                            numReady++;
                    }
                    else
                    {
                        if (i.value.color.a > 0)
                            colorChanger(i);
                        else
                            numReady++;
                    }
                }

                yield return null;
            }
        }

        if (callback != null)
            callback.Invoke();
    }

    void StartFading(Action a)
    {
        StartCoroutine(Appearance(ApparanceDirection.Fade, toFadeImages));
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayTime);

        Messenger.Broadcast(MyGameEvents.TUTORIAL_START);
    }

    IEnumerator Count()
    {
        int i = 3;

        counter.text = i.ToString();
        counter.gameObject.SetActive(true);

        while (i != 1)
        {
            while (counter.color.a > 0)
            {
                counter.color -= new Color(0, 0, 0, alphaNumberDecrease * Time.deltaTime);

                yield return null;
            }

            yield return null;

            counter.text = (--i).ToString();
            counter.color += new Color(0, 0, 0, 1f);

            yield return null;
        }

        while (counter.color.a > 0)
        {
            counter.color -= new Color(0, 0, 0, alphaNumberDecrease * Time.deltaTime);

            yield return null;
        }

        if (!isTutorial)
        {
            Messenger.Broadcast(MyGameEvents.START_TIMER);
            Messenger.Broadcast(MyGameEvents.EVENT_GENERATOR_START);
        }
        else
        {
            Messenger.Broadcast(MyGameEvents.TUTORIAL_START);
        }
    }

    [Serializable]
    class ChangableColorItem
    {
        public ChangableColorItem()
        {
            maxAlpha = 1f;
        }
        public Image value;
        [Range(0, 1f)]
        public float maxAlpha = 1f;
        public float customSpeed = 0;
    }
}
