using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerControl : MonoBehaviour
{
    [SerializeField]
    int time = 10;

    Text label;

    void Awake()
    {
        Messenger.AddListener(MyGameEvents.START_TIMER, OnStartTime);
        label = GetComponent<Text>();
    }

    void Start()
    {
        if (!Fade.IsTutorial)
            label.text = ToTimeFormatter();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(MyGameEvents.START_TIMER, OnStartTime);
    }

    void OnStartTime()
    {
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while(true)
        {
            label.text = ToTimeFormatter();

            time--;

            yield return new WaitForSeconds(1f);

            if (time == -1)
                break;
        }
        Messenger<bool>.Broadcast(MyGameEvents.GAME_END, true);
    }

    string ToTimeFormatter()
    {
        int minutes = (int)(time / 60);
        int seconds = time % 60;

        return (minutes < 10 ? "0" + minutes : minutes.ToString()) + ":" + (seconds < 10 ? "0" + seconds : seconds.ToString());
    }
}
