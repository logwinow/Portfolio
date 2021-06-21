using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SuitController : MonoBehaviour, ISavable
{
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private string title;
        [SerializeField] private SectionID[] upgradesID;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private bool _haveCondition;
        [SerializeField] private GameManager.ItemUI.Condition _doneCondition;
        [SerializeField] private Sprite _brokenSprite;
        [SerializeField] private Sprite _doneSprite;
        [SerializeField] private SuitInteractionObject _interactionSuit;
        
        private Vector3? center;
        private List<WorkbenchItem> instantiatedUpgrades;
        private List<WorkbenchItemSL> _savedUpgrades;
        
        public bool IsAvailable { get; set; } = true;

        private Vector3 Center
        {
            get
            {
                return center ??= _spriteRenderer.bounds.center;
            }
        }
        
        public string Title => title;

        private void Awake()
        {
            _interactionSuit = GetComponent<SuitInteractionObject>();
        }

        public void UpdateSprite()
        {
            if (_haveCondition && !_doneCondition.Check())
            {
                _spriteRenderer.sprite = _brokenSprite;
            }
            else
            {
                _spriteRenderer.sprite = _doneSprite;
            }
        }
        
        public void FocusCamera(CameraShift cameraShift, float shift)
        {
            cameraShift.Shift(Center + Vector3.right * shift);
        }

        public void CloseUpgrades()
        {
            if (instantiatedUpgrades != null)
            {
                foreach (var u in instantiatedUpgrades)
                    u.gameObject.SetActive(false);
            }
        }

        public void Disable()
        {
            IsAvailable = false;
            _spriteRenderer.gameObject.SetActive(false);
        }

        public void OutputUpgrades()
        {
            if (instantiatedUpgrades == null)
            {
                instantiatedUpgrades = new List<WorkbenchItem>();
                WorkbenchItem upg;

                for (int i = 0; i < upgradesID.Length; i++)
                {
                    upg = Instantiate(GameManager.Instance.WorkbenchItemPrefab, _scrollRect.content);
                    upg.OnUpgradeCallback = delegate
                    {
                        UpdateSprite();
                        OutputUpgrades();
                        if (_interactionSuit.CheckCondition())
                            _interactionSuit.gameObject.layer = 9;
                    };
                    upg.Set(upgradesID[i], _savedUpgrades?[i].CurrentUpgrade ?? 0);
                    upg.Show();
                    instantiatedUpgrades.Add(upg);
                }
            }
            else
            {
                foreach (var u in instantiatedUpgrades)
                {
                    u.Show();
                }
            }
        }
        
        public void Load(object obj)
        {
            var sc = obj as SuitControllerSL;

            _savedUpgrades = (obj as SuitControllerSL)!.SuitWorkbenchItems;
        }

        public bool Save(out object obj)
        {
            obj = new SuitControllerSL(this);

            return (obj as SuitControllerSL).SuitWorkbenchItems != null;
        }
        
    [Serializable]
    private class SuitControllerSL
    {
        public SuitControllerSL(SuitController suit)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var instItems = typeof(SuitController)
                    .GetField("instantiatedUpgrades", flags)!
                .GetValue(suit) as List<WorkbenchItem>;
            
            if (instItems == null)
                return;

            _suitWorkbenchItems = new List<WorkbenchItemSL>();
            
            foreach (var i in instItems)
            {
                _suitWorkbenchItems.Add(new WorkbenchItemSL(i));
            }
        }

        private List<WorkbenchItemSL> _suitWorkbenchItems;

        public List<WorkbenchItemSL> SuitWorkbenchItems => _suitWorkbenchItems;
    }
    
    [Serializable]
    private class WorkbenchItemSL
    {
        public WorkbenchItemSL(WorkbenchItem wi)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            var cUpg = (typeof(WorkbenchItem)
                        .GetField("currentUpgradeIcon", flags)!
                    .GetValue(wi)
                as UpgradeIcon);

            if (cUpg == null)
            {
                _currentUpgrade = -1;
            }
            else
            {
                _currentUpgrade = ((List<UpgradeIcon>) typeof(WorkbenchItem)
                            .GetField("upgradeIcons", flags)!
                        .GetValue(wi))
                    .IndexOf(cUpg);
            }
        }
            
        private int _currentUpgrade;
        public int CurrentUpgrade => _currentUpgrade;
    }
    }
