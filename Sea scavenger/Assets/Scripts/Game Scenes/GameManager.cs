using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Custom.Patterns;
using System;
using System.Linq;

using sceneManagement = UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Item[] items;
    [SerializeField] private WorkbenchItem _workbenchItemPrefab;
    [SerializeField] private UpgradeIcon _upgradeIconPrefab;
    [SerializeField] private ItemUI[] _itemsUI;
    [SerializeField] private Parameter[] _parameters;
    // [SerializeField] private int _mainMenuSceneIndex = 0;
    // [SerializeField] private int _boatSceneIndex = 1;
    // [SerializeField] private int _divingSceneIndex = 2;

    public WorkbenchItem WorkbenchItemPrefab => _workbenchItemPrefab;
    public UpgradeIcon UpgradeIconPrefab => _upgradeIconPrefab;
    // public int MainMenuSceneIndex => _mainMenuSceneIndex;
    // public int BoatSceneIndex => _boatSceneIndex;
    // public int DivingSceneIndex => _divingSceneIndex;

    protected override void Init()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        
        
#if UNITY_EDITOR
        GameSaver.Load(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        AudioManager.Instance.PlayAmbient(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
#endif
    }
    
    public void GoToScene(int nextSceneId, bool saveCurrentScene = true, bool loadNextSceneData = true)
    {
        if (saveCurrentScene)
            SaveCurrentScene();

        NextSceneLoadingController.Instance.LoadScene(nextSceneId,
            loadNextSceneData ? 
                (Action)(() => GameSaver.Load(nextSceneId)) : null);
        
        AudioManager.Instance.PlayAmbient(nextSceneId);
    }

    public Item GetItem(SectionID id)
    {
        return items.First(i => i.SectionId == id);
    }

    public ItemUI GetItemUI(SectionID id)
    {
        return _itemsUI.First(i => i.SectionId == id);
    }

    public Parameter GetParameter(SectionID id)
    {
        return _parameters.First(p => p.SectionId == id);
    }
    
    public void UpgradeParameters(ItemUI.Upgrade upgrade)
    {
        foreach (var p in upgrade.Parameters)
        {
            GetParameter(p.ParameterID).Set(p.Value);
        }
    }

    public void SaveCurrentScene()
    {
        GameSaver.Save(sceneManagement::SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit(bool saveCurrentScene = true)
    {
        if (saveCurrentScene)
            SaveCurrentScene();
        
        Application.Quit();
    }
    

    [Serializable]
    public struct Item
    {
        [SerializeField]
        private string title;
        [SerializeField]
        private SectionID sectionID;
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private float remeltTime;
        [SerializeField]
        private ChancesAndCounts remeltedDropChancesAndCounts;
        [SerializeField]
        private GameObject remeltedPrefab;

        public SectionID SectionId => sectionID;
        public string Title => title;
        public GameObject Prefab => prefab;
        public float RemeltTime => remeltTime;
        public ChancesAndCounts RemeltedDropChancesAndCounts => remeltedDropChancesAndCounts;
        public GameObject RemeltedPrefab => remeltedPrefab;
    }

    [Serializable]
    public struct ItemUI
    {
        [SerializeField] private string _name;
        [SerializeField] private SectionID _sectionId;
        [SerializeField] private Sprite _iconSprite;
        [SerializeField] private Upgrade[] _upgrades;
        [SerializeField] private Condition _condition;
        [SerializeField] private bool _haveCondition;

        public SectionID SectionId => _sectionId;
        public Sprite IconSprite => _iconSprite;
        public int UpgradesCount => _upgrades.Length;
        public string Name => _name;
        public Condition ConditionValue => _condition;

        public bool GetUpgrade(int index, out Upgrade upgrade)
        {
            if (index >= _upgrades.Length)
            {
                upgrade = default(Upgrade);
                
                return false;
            }

            upgrade = _upgrades[index];
            
            return true;
        }
        
        public bool CheckAvailability()
        {
            return !_haveCondition || _condition.Check();
        }

        [Serializable]
        public struct Upgrade
        {
            [SerializeField] private string _name;
            [SerializeField] private string _description;
            [SerializeField] private IdAndCount[] _resources;
            [SerializeField] private Parameter[] _parameters;
            
            
            public string Name => _name;
            public string Description => _description;
            public IdAndCount[] Resources => _resources;
            public Parameter[] Parameters => _parameters;

            [Serializable]
            public struct IdAndCount
            {
                [SerializeField] private SectionID resourceID;
                [SerializeField] private int count;

                public SectionID ID => resourceID;
                public int Count => count;
            }
            
            [Serializable]
            public struct Parameter
            {
                [SerializeField] private SectionID _parameterID;
                [SerializeField] private int _value;

                public SectionID ParameterID => _parameterID;
                public int Value => _value;
            }
        }
        
        [Serializable]
        public class Condition
        {
            [SerializeField] private SectionID _parameterID;
            [SerializeField] private ComparisonType _comparisonType;
            [SerializeField] private int _value;

            private GameManager.Parameter _parameter;

            public bool Check()
            {
                _parameter ??= GameManager.Instance.GetParameter(_parameterID);

                switch (_comparisonType)
                {
                    case ComparisonType.Equals:
                        return _parameter.Value == _value;
                    case ComparisonType.Greater:
                        return _parameter.Value > _value;
                    case ComparisonType.Less:
                        return _parameter.Value < _value;
                    default:
                        return false;
                }
            }

            private enum ComparisonType
            {
                Greater, Equals, Less
            }
        }
    }
    
    [Serializable]
    public class Parameter
    {
        [SerializeField] private SectionID _sectionId;
        [SerializeField] private int _baseValue;

        private int? _value;
    
        public SectionID SectionId => _sectionId;

        public int Value => _value ??= _baseValue;

        public void Set(int newValue, bool saveParameter = true)
        {
            _value = newValue;
            
            if (saveParameter)
                GameSaver.SavedGlobalData.AddParameter(_sectionId, newValue);
        }

        public void ChangeAt(int value, bool saveParameter = true)
        {
            Set(Value + value, saveParameter);
        }
    }
}
