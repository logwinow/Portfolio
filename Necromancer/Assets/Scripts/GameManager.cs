using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    GameObject mainPopup;
    [SerializeField]
    GameObject labelPopup;
    [SerializeField]
    GameObject underGravePopup;
    [SerializeField]
    Image m_cursor;
    public Image teachWinodw;
    [SerializeField]
    AudioClip teachSound;
    [SerializeField]
    Image revivalImage;
    [SerializeField]
    float revivalGlowDuration = 2f;
    [SerializeField]
    float revivalAppearanceDuration = 1f;
    [SerializeField]
    float revivalTranslateDuration = 1f;
    [SerializeField]
    float revivalRotationSpeed = 1f;
    [SerializeField]
    float revivalIconScaleModif = 2f;
    [SerializeField]
    AudioClip newHeroSound;
    [SerializeField]
    AudioClip revivalSound;
    [SerializeField]
    List<Character> characters;
    [SerializeField]
    Text labelName;
    [SerializeField]
    Text labelYearsOfLife;
    [SerializeField]
    Text labelBiography;
    [SerializeField]
    Image underGraveBG;
    [SerializeField]
    Image swordImage;
    [SerializeField]
    Image bowImage;
    [SerializeField]
    Image staffImage;
    [SerializeField]
    Image shieldImage;
    [SerializeField]
    Image subjectImage;
    [SerializeField]
    Image shovelIcon;
    [SerializeField]
    Image takeItAliveIcon;
    [SerializeField]
    GameObject purpleGlow;
    [SerializeField]
    float midGlow;
    [SerializeField]
    float blinkDuration = .5f;
    [SerializeField]
    Image[] unitImage;
    [SerializeField]
    AudioSource audioSource;
    public AudioSource Audio
    {
        get
        {
            return audioSource;
        }
    }
    [SerializeField]
    AudioClip eyeEquippedSound;
    [SerializeField]
    AudioClip shovelEquippedSound;
    public AudioClip dirtDestroiedSound;
    public AudioClip tombstoneLabelInteractSound;
    public AudioClip calebSound;
    public AudioClip[] tombstoneGraveInteractSound;

    [SerializeField]
    int unitsCount = 0;
    public int UnitsCount
    {
        get
        {
            return unitsCount;
        }
    }

    bool isCurrentShovel = true;
    public bool IsCurrentShovel
    {
        get
        {
            return isCurrentShovel;
        }
    }

    [SerializeField]
    RecieveObjective objectiveController;
    public RecieveObjective ObjectiveController
    {
        get
        {
            return objectiveController;
        }
    }
    public GameObject ActivePopup { get; private set; }

    public List<Character> Characters
    {
        get
        {
            return characters;
        }
    }

    bool isPaused = false;
    internal int noNamedGraveCount = 2;

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
        set
        {
            if (value == true)
                Time.timeScale = 0;
            else
                Time.timeScale = 1f;

            isPaused = value;
        }
    }

    [SerializeField]
    Feature health;
    [SerializeField]
    Feature attack;
    [SerializeField]
    Feature defence;
    [SerializeField]
    Feature morality;

    [SerializeField]
    float enemieHealth = 80f;
    [SerializeField]
    float enemieDefence = 60f;
    [SerializeField]
    float enemieAttack = 50f;

    int eviCount = 0;
    int goodCount = 0;
    int cowardCount = 0;
    int braveCount = 0;

    AdvancedCoroutine<int> glowAlphaCor;
    [SerializeField]
    float glowAppearanceDuration = .5f;

    int personsInAir = 0;

    private void Start()
    {
        glowAlphaCor = new AdvancedCoroutine<int>(this, GlowAlphaAdjustment);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !IsPaused)
        {
            Audio.PlayOneShot(teachSound);
            teachWinodw.gameObject.SetActive(true);
        }

        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            teachWinodw.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetVisibleMenuPopup()
    {
        SetVisiblePopup(mainPopup);
    }

    public void SetVisibleLabelPopup()
    {
        SetVisiblePopup(labelPopup);
    }

    public void SetVisibleUnderGravePopup()
    {
        SetVisiblePopup(underGravePopup);
    }

    private void SetVisiblePopup(GameObject popup)
    {
        SetCursor(CursorType.Unvisible);

        popup.SetActive(true);

        ActivePopup = popup;

        isPaused = true;
    }

    public void SetCursor(CursorType cursorType)
    {
        if (cursorType == CursorType.Unvisible)
        {
            m_cursor.color = new Color(1f, 1f, 1f, 0);
        }
        else if (cursorType == CursorType.Visible)
        {
            m_cursor.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            m_cursor.sprite = Resources.Load<Sprite>("Cursors/cursor_" + (int)cursorType);
            m_cursor.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    void SwitchWeapon()
    {
        if (isCurrentShovel)
        {
            Audio.PlayOneShot(eyeEquippedSound);
            shovelIcon.sprite = Resources.Load<SpriteAtlas>("UI/WeaponIconsAtlas").GetSprite("shovel_disactive");
            takeItAliveIcon.sprite = Resources.Load<SpriteAtlas>("UI/WeaponIconsAtlas").GetSprite("takeitalive_active");
        }
        else
        {
            Audio.PlayOneShot(shovelEquippedSound);
            shovelIcon.sprite = Resources.Load<SpriteAtlas>("UI/WeaponIconsAtlas").GetSprite("shovel_active");
            takeItAliveIcon.sprite = Resources.Load<SpriteAtlas>("UI/WeaponIconsAtlas").GetSprite("takeitalive_disactive");
        }

        glowAlphaCor.Start(IsCurrentShovel == true ? 1 : -1);
        isCurrentShovel = !isCurrentShovel;
    }

    IEnumerator GlowAlphaAdjustment(int dir)
    {
        float timer = 0;
        float from = purpleGlow.GetComponent<Image>().color.a;
        float to = dir == 1 ? 1 : 0;

        while ((timer += Time.deltaTime) < glowAppearanceDuration)
        {
            purpleGlow.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(from, to, timer / glowAppearanceDuration));

            yield return null;
        }

        float delta = 1f - midGlow;
        while(!isCurrentShovel)
        {
            if (purpleGlow.GetComponent<Image>().color.a > midGlow)
            {
                float t = 0;
                while((t += Time.deltaTime) < blinkDuration)
                {
                    purpleGlow.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(1f, midGlow - delta, t / blinkDuration));

                    yield return null;
                }
            }
            else // a < mid
            {
                float t = 0;
                while ((t += Time.deltaTime) < blinkDuration)
                {
                    purpleGlow.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(midGlow - delta, 1f, t / blinkDuration));

                    yield return null;
                }
            }
        }

        if (dir == -1)
            purpleGlow.GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void CloseActivePopup()
    {
        SetCursor(CursorType.Visible);

        ActivePopup?.SetActive(false);

        ActivePopup = null;

        isPaused = false;
    }

    public void InitializeLabelPopup(Character c)
    {
        labelName.text = c.name.GetText();
        labelBiography.text = c.text.GetText();

        labelYearsOfLife.text = c.yearOfLife;
    }

    public void InitializeUnderGravePopup(Skeleton si)
    {
        underGraveBG.sprite = Resources.Load<Sprite>("Skeletons/skeleton_" + si.pose);

        if (si.mageWeapon != MageWeaponType.None)
        {
            staffImage.color = new Color(1f, 1f, 1f, 1f);
            staffImage.sprite = Resources.Load<Sprite>("Items/" + si.mageWeapon.ToString());
        }
        else
        {
            staffImage.color = new Color(1f, 1f, 1f, 0f);
        }

        if (si.meleeWeapon != MeleeWeaponType.None)
        {
            swordImage.color = new Color(1f, 1f, 1f, 1f);
            swordImage.sprite = Resources.Load<Sprite>("Items/" + si.meleeWeapon.ToString());
        }
        else
        {
            swordImage.color = new Color(1f, 1f, 1f, 0f);
        }

        if (si.arhcerWeapon != ArhcerWeaponType.None)
        {
            bowImage.color = new Color(1f, 1f, 1f, 1f);
            bowImage.sprite = Resources.Load<Sprite>("Items/" + si.arhcerWeapon.ToString());
        }
        else
        {
            bowImage.color = new Color(1f, 1f, 1f, 0f);
        }

        if (si.shieldType != ShieldType.None)
        {
            shieldImage.color = new Color(1f, 1f, 1f, 1f);
            shieldImage.sprite = Resources.Load<Sprite>("Items/" + si.shieldType.ToString());
        }
        else
        {
            shieldImage.color = new Color(1f, 1f, 1f, 0f);
        }

        if (si.subjectClassifier != SubjectClassifierType.None)
        {
            subjectImage.color = new Color(1f, 1f, 1f, 1f);
            subjectImage.sprite = Resources.Load<Sprite>("Items/" + si.subjectClassifier.ToString());
        }
        else
        {
            subjectImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    

    public Character TakeCharacter()
    {
        Character c = characters[Random.Range(0, characters.Count)];

        characters.Remove(c);

        return c;
    }

    public void AddUnitIcon(Skeleton skeleton)
    {
        unitImage[unitsCount].gameObject.SetActive(true);
        unitImage[unitsCount].sprite = Resources.Load<SpriteAtlas>("FacesAtlas").GetSprite("faces_" + (skeleton.character.id - 1).ToString());
        unitImage[unitsCount].SetNativeSize();

        StartCoroutine(RevivalUnit(unitImage[unitsCount]));

        CalculateFeatures(skeleton);
    }

    IEnumerator RevivalUnit(Image img)
    {
        personsInAir++;
        float timer = 0;
        Vector2 revivalMaxSize;
        Vector2 imgMaxSize = img.rectTransform.GetSize() / 2f;
        img.rectTransform.SetSize(img.rectTransform.GetSize() / 100f);
        revivalMaxSize = new Vector2(900f, 900f);
        Vector3 endPos = img.rectTransform.position;
        img.rectTransform.position = revivalImage.rectTransform.position;

        Audio.PlayOneShot(revivalSound, 1.5f);

        while ((timer += Time.deltaTime) < revivalGlowDuration)
        {
            float part = timer / revivalGlowDuration;
            revivalImage.rectTransform.SetSize(revivalMaxSize * part);
            revivalImage.rectTransform.rotation *= Quaternion.Euler(new Vector3(0, 0, revivalRotationSpeed * Time.deltaTime));
            img.rectTransform.SetSize(imgMaxSize * revivalIconScaleModif * part);

            yield return null;
        }

        timer = 0;

        while ((timer += Time.deltaTime) < revivalAppearanceDuration)
        {
            revivalImage.rectTransform.rotation *= Quaternion.Euler(new Vector3(0, 0, revivalRotationSpeed * Time.deltaTime));

            yield return null;
        }

        timer = 0;

        Vector3 startPos = img.rectTransform.position;

        while ((timer += Time.deltaTime) < revivalTranslateDuration)
        {
            float part = timer / revivalTranslateDuration;
            revivalImage.rectTransform.SetSize(revivalMaxSize * (1 - part));
            revivalImage.rectTransform.rotation *= Quaternion.Euler(new Vector3(0, 0, revivalRotationSpeed * Time.deltaTime));
            img.rectTransform.SetSize(Vector2.Lerp(imgMaxSize * revivalIconScaleModif, imgMaxSize, part));
            img.rectTransform.position = Vector3.Lerp(startPos, endPos, part * part);

            yield return null;
        }

        img.rectTransform.SetSize(imgMaxSize);
        revivalImage.rectTransform.SetSize(Vector2.zero);

        Audio.PlayOneShot(newHeroSound, 0.7f);

        personsInAir--;

        if (unitsCount == 10 && personsInAir == 0)
        {
            objectiveController.SetObjective(1);
        }
    }

    int GetAttackValueFromWeapon(WeaponType wt, int w)
    {
        int midAttack = 14;
        int maxAttack = 20;

        switch(wt)
        {
            case WeaponType.Archer:
                switch((ArhcerWeaponType)w)
                {
                    case ArhcerWeaponType.Bow:
                        return midAttack + 4;
                    case ArhcerWeaponType.Crossbow + 4:
                        return maxAttack;
                }
                break;
            case WeaponType.Mage:
                switch ((MageWeaponType)w)
                {
                    case MageWeaponType.StaffCommon:
                        return midAttack;
                    case MageWeaponType.StaffRare:
                        return maxAttack;
                }
                break;
            case WeaponType.Melee:
                switch ((MeleeWeaponType)w)
                {
                    case MeleeWeaponType.Sword:
                        return midAttack;
                    case MeleeWeaponType.Sabre:
                        return midAttack;
                    case MeleeWeaponType.Rapier:
                        return maxAttack;
                }
                break;
            case WeaponType.None:
                break;
        }
        return 7;
    }

    int GetDefence(ShieldType st, CareerType career)
    {
        switch(st)
        {
            case ShieldType.Iron:
                return 15;
            case ShieldType.Wooden:
                return 10;
        }

        if (career == CareerType.Mage)
            return 15;

        return 0;
    }

    void CalculateFeatures(Skeleton s)
    {
        if (s.character.fractionType == FractionType.Good)
            goodCount++;
        else
            eviCount++;

        unitsCount++;

        // BUFF
        if (s.character.careerType == CareerType.Smith)
        {
            defence.buff *= 1.5f;
        }
        else if (s.character.careerType == CareerType.Cleric)
        {
            health.buff *= 1.5f;
        }

        // ATTACK
        AddValueToFeature(attack, GetAttackValueFromWeapon(s.weaponType, s.weapon), (attack.sprites[0], 0), (attack.sprites[1], enemieHealth), (attack.sprites[2], enemieHealth + 20f));
        // Defence
        AddValueToFeature(defence, GetDefence(s.shieldType, s.character.careerType), (defence.sprites[0], 0), (defence.sprites[1], defence.mid), (defence.sprites[2], defence.max));
        // HEALTH
        AddValueToFeature(health, ((int)(s.character.healthType) + 1) * 5, (health.sprites[0], 0), (health.sprites[1], enemieAttack), (health.sprites[2], enemieAttack + 30f));
        // MORALITY
        float moralCoeff = (float)Mathf.Max(goodCount, eviCount) / (float)unitsCount;
        float cb = ((float)cowardCount / (float)unitsCount);

        morality.value = Mathf.Max(goodCount, eviCount);

        if (moralCoeff < .7 || cb > .5)
        {
            morality.icon.sprite = morality.sprites[0];
        }
        else if (moralCoeff < .9f)
        {
            morality.icon.sprite = morality.sprites[1];
        }
        else 
        {
            morality.icon.sprite = morality.sprites[2];
        }
    }

    void AddValueToFeature(Feature feature, int deltaValue, params (Sprite sprite, float floor)[] stageInfo)
    {
        feature.value += deltaValue;

        int i = 0;

        while (i < stageInfo.Length - 1)
        {
            if (feature.value >= stageInfo[i + 1].floor)
                i++;
            else
                break;
        }

        feature.icon.sprite = stageInfo[i].sprite;
    }

    public void EndGame()
    {
        bool coolMoral = false;

        if (morality.value >= 9 || braveCount >= 7)
        {
            attack.buff = 1.5f;
            coolMoral = true;
        }

        // defeats
        if (attack.value <= 20f && defence.value < defence.max)
        {
            PlayerPrefs.SetInt("Result", 0);
            PlayerPrefs.SetInt("Cause", 2);

            return;
        }   

        if (cowardCount > 5 || (5 <= morality.value && morality.value <= 6))
        {
            PlayerPrefs.SetInt("Result", 0);
            PlayerPrefs.SetInt("Cause", 0);

            return;
        }

        if (health.value < enemieHealth && ((defence.value >= defence.max && attack.value >= enemieAttack + 20f) || (defence.value >= defence.mid && attack.value >= enemieAttack + 20f && coolMoral)))
        {
            PlayerPrefs.SetInt("Result", 1);
            PlayerPrefs.SetInt("Cause", 1);

            return;
        }

        health.value = (int)((float)health.value * health.buff);

        while(true)
        {
            enemieHealth -= attack.value * attack.buff * Mathf.Clamp(1 - enemieDefence / 100f, 0, .8f);

            if (enemieHealth <= 0)
            {
                PlayerPrefs.SetInt("Result", 1);
                PlayerPrefs.SetInt("Cause", 0);

                return;
            }

            health.value -= (int)(enemieAttack * Mathf.Clamp(1 - defence.value * defence.buff / 100f, 0, .8f));

            if (health.value <= 0)
            {
                PlayerPrefs.SetInt("Result", 0);
                PlayerPrefs.SetInt("Cause", 1);

                return;
            }
        }
    }

    public void SaveData()
    {
        string path = Path.Combine(Application.dataPath + "/Database/CharactersInClassicJson.txt");

        SaveObject so = new SaveObject() { characters = characters };

        File.WriteAllText(path, JsonUtility.ToJson(so));

        Debug.Log("Saved path: " + path);
    }

    public void LoadData()
    {
        string path = Path.Combine(Application.dataPath + "/Database/CharactersInClassicJson.txt");

        characters = JsonUtility.FromJson<SaveObject>(File.ReadAllText(path)).characters;
    }
}

[System.Serializable]
class SaveObject
{
    public List<Character> characters;
}

[System.Serializable]
public class Character
{
    public int id;
    public string yearOfLife;
    public LanguageNode name;
    public LanguageNode text;
    public HealthType healthType;
    public MoralityType moralityType;
    public FractionType fractionType;
    public CareerType careerType;
}