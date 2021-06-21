using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ToggleType
{
    Tube = 0,
    Plug = 1,
    Braking = 2,
    Control = 3
}
public class SceneController : MonoBehaviour
{
    [SerializeField]
    Feature temp;
    [SerializeField]
    Feature overload;
    [SerializeField]
    Feature pressure;
    [SerializeField]
    Feature antifreeze;
    [SerializeField]
    Feature waste;
    [SerializeField]
    Feature particle;
    [SerializeField]
    float startDelay = 4f;
    [SerializeField]
    float timeStep = 10f;
    [SerializeField]
    float antifreezeDecrease = .05f;
    [SerializeField]
    GameObject finishMessageBox;
    [SerializeField]
    GameObject looseMessageBox;
    [SerializeField]
    float temperatureIncrease = 4f;
    [SerializeField]
    float pressureIncrease = 4f;
    [SerializeField]
    float chargedPartIncrease = 4f;
    [SerializeField]
    float wasteIncrease = 4f;

    bool gameStarted = false;

    public static Feature s_overload;
    public static AudioSource globalAudio;

    public bool TubeToggle { get; set; }
    public bool PlugOFFToggle { get; set; }
    public bool BrakingToggle { get; set; }
    public bool ControlToggle { get; set; }

    IEnumerator eventGen;

    void Awake()
    {
        Messenger<ToggleType>.AddListener(MyGameEvents.TOGGLE_PRESSED, SwitchToggleStatus);
        Messenger<bool>.AddListener(MyGameEvents.GAME_END, OnGameFinish);
        Messenger.AddListener(MyGameEvents.EVENT_GENERATOR_START, OnGeneratorStart);
        Messenger.AddListener(MyGameEvents.TUTORIAL_START, OnTutorialStart);
        Messenger<TipDeclaration.ImpactSwitch>.AddListener(MyGameEvents.TUTORIAL_FEATURE_CHANGER, OnFeatureChangeCall);


        s_overload = overload;
        globalAudio = GetComponent<AudioSource>();
        AudioListener.volume = 1f;
    }

    void OnDestroy()
    {
        Messenger<ToggleType>.RemoveListener(MyGameEvents.TOGGLE_PRESSED, SwitchToggleStatus);
        Messenger<bool>.RemoveListener(MyGameEvents.GAME_END, OnGameFinish);
        Messenger.RemoveListener(MyGameEvents.EVENT_GENERATOR_START, OnGeneratorStart);
        Messenger.RemoveListener(MyGameEvents.TUTORIAL_START, OnTutorialStart);
        Messenger<TipDeclaration.ImpactSwitch>.RemoveListener(MyGameEvents.TUTORIAL_FEATURE_CHANGER, OnFeatureChangeCall);
    }

    void Start()
    {
        Messenger.Broadcast(MyMenuEvents.REFRASH_LANGUAGE);

        if (finishMessageBox != null)
            finishMessageBox.SetActive(false);
        if (looseMessageBox != null)
            looseMessageBox.SetActive(false);
    }

    void OnGeneratorStart()
    {
        StartCoroutine(eventGen = EventGenerator());
        gameStarted = true;
    }

    void OnTutorialStart()
    {
        Messenger.Broadcast(MyGameEvents.SHOW_CUR_TIP);
        gameStarted = true;
    }

    void OnFeatureChangeCall(TipDeclaration.ImpactSwitch impactType)
    {
        switch(impactType)
        {
            case TipDeclaration.ImpactSwitch.ICPC:
                particle.ChangeValue(chargedPartIncrease * Time.deltaTime);
                particle.gameObject.GetComponent<TutorOverloadOnFeature>().isActive = true;
                break;
            case TipDeclaration.ImpactSwitch.Pressure:
                pressure.ChangeValue(pressureIncrease * Time.deltaTime);
                pressure.gameObject.GetComponent<TutorOverloadOnFeature>().isActive = true;
                break;
            case TipDeclaration.ImpactSwitch.REFR:
                antifreeze.AddValue(-100f);
                antifreeze.AddValue(62f);
                antifreeze.gameObject.GetComponent<TutorOverloadOnFeature>().isActive = true;
                break;
            case TipDeclaration.ImpactSwitch.TemperatureInc:
                temp.ChangeValue(temperatureIncrease * Time.deltaTime);
                temp.gameObject.GetComponent<TutorOverloadOnFeature>().isActive = true;
                break;
            case TipDeclaration.ImpactSwitch.WASAC:
                waste.ChangeValue(wasteIncrease * Time.deltaTime);
                waste.gameObject.GetComponent<TutorOverloadOnFeature>().isActive = true;
                break;
            case TipDeclaration.ImpactSwitch.TemperatureDecr:
                temp.ChangeValue(-temperatureIncrease * Time.deltaTime);
                temp.gameObject.GetComponent<TutorOverloadOnFeature>().isActive = true;
                break;
        }
    }

    void Update()
    {
        if (antifreeze.CurrentValue != 0)
            antifreeze.ChangeValue(-antifreezeDecrease * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStarted)
            {
                if (eventGen != null)
                    StopCoroutine(eventGen);
                Messenger<Action>.Broadcast(MyMenuEvents.CLOSE_DOORS, () => UnityEngine.SceneManagement.SceneManager.LoadScene(1));
                AudioListener.volume = 0f;
            }
        }
    }

    IEnumerator EventGenerator()
    {
        while(true)
        {
            int evId = UnityEngine.Random.Range(0, 4);
            switch (evId)
            {
                case 0:
                    temp.ChangeValue(temperatureIncrease * Time.deltaTime);
                    break;
                case 1:
                    pressure.ChangeValue(pressureIncrease * Time.deltaTime);
                    break;
                case 2:
                    particle.ChangeValue(chargedPartIncrease * Time.deltaTime);
                    break;
                case 3:
                    temp.ChangeValue(- temperatureIncrease * Time.deltaTime);
                    break;
            }
            yield return new WaitForSeconds(timeStep);
        }
    }

    void SwitchToggleStatus(ToggleType toggle)
    {
        switch(toggle)
        {
            case ToggleType.Tube:
                TubeToggle = !TubeToggle;

                if (!ControlToggle && TubeToggle)
                    particle.StopIncrease();
                break;
            case ToggleType.Plug:
                PlugOFFToggle = !PlugOFFToggle;
                break;
            case ToggleType.Control:
                ControlToggle = !ControlToggle;

                if (!ControlToggle && TubeToggle)
                    particle.StopIncrease();
                break;
            case ToggleType.Braking:
                BrakingToggle = !BrakingToggle;
                break;
        }
    }

    void OnGameFinish(bool value)
    {
        if (eventGen != null)
            StopCoroutine(eventGen);
        Messenger<Action>.Broadcast(MyMenuEvents.CLOSE_DOORS, () => StartCoroutine(FinishMessageBox(value)));
        AudioListener.volume = 0f;
    }

    IEnumerator FinishMessageBox(bool value)
    {
        if (value)
        {
            finishMessageBox.SetActive(true);
        }
        else
            looseMessageBox.SetActive(true);

        yield return new WaitForSeconds(5f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}

public static class FeatureExtension
{
    public static void ChangeValue(this Feature feature, float value, IncreaseType mode = IncreaseType.Constant)
    {
        feature.CurParameters = new FeatureParameters(mode, value);
    }
}
