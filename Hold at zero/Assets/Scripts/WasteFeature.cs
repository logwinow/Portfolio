using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteFeature : Feature
{
    [SerializeField]
    SceneController scContr;
    [SerializeField]
    float incrValue = .05f;
    [SerializeField]
    float timeDelay = .5f;
    OrdinaryCoroutine delay;

    bool gameStarted = false;

    protected override void Awake()
    {
        base.Awake();
        delay = new OrdinaryCoroutine(this, Delay);
        Messenger.AddListener(MyGameEvents.EVENT_GENERATOR_START, OnGeneratorStart);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Messenger.RemoveListener(MyGameEvents.EVENT_GENERATOR_START, OnGeneratorStart);
    }

    void OnGeneratorStart()
    {
        gameStarted = true;
    }
    protected override void Update()
    {
        base.Update();

        if (gameStarted)
        {
            if (scContr.TubeToggle && !delay.IsWorking)
            {
                delay.Start();
            }
            else if (!scContr.TubeToggle && delay.IsWorking)
            {
                delay.Stop();
            }
        }
    }

    IEnumerator Delay()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeDelay);
            AddValue(incrValue);
        }
    }
}
