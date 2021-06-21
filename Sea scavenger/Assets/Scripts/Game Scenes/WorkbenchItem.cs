using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class WorkbenchItem : DoubleClickableUI, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private HorizontalLayoutGroup horLayout;

    private List<UpgradeIcon> upgradeIcons;
    private UpgradeIcon currentUpgradeIcon;
    private RectTransform rt;
    private bool isMouseOver;
    private GameManager.ItemUI itemUI;
    
    public Action OnUpgradeCallback { get; set; }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isMouseOver)
        {
            ContextMenu.Instance.SetOnPosition(Input.mousePosition);
        }
    }

    protected override void OnDoubleClick(PointerEventData eventData)
    {
        if (OnUpgrade())
        {
            OnPointerExit(eventData);
            OnPointerEnter(eventData);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        int i = -1;
        
        while(++i < upgradeIcons.Count)
        {
            if (!upgradeIcons[i].IsUpgraded)
            {
                break;
            }
        }

        ContextMenu.Instance.ShowAndSetWithDelay(eventData.position, itemUI, i, () => isMouseOver = true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        ContextMenu.Instance.Close();
        
        isMouseOver = false;
    }

    public void Show()
    {
        gameObject.SetActive(itemUI.CheckAvailability());
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Set(SectionID sectionId, int currentIconIndex = 0)
    {
        upgradeIcons = new List<UpgradeIcon>();
        itemUI = GameManager.Instance.GetItemUI(sectionId);

        for (int i = 0; i < itemUI.UpgradesCount; i++)
        {    
            var uIcon = Instantiate(GameManager.Instance.UpgradeIconPrefab, horLayout.transform);
            _ = itemUI.GetUpgrade(i, out var upg);
            
            uIcon.Set(upg, currentIconIndex > i);
            
            upgradeIcons.Add(uIcon);
        }
        
        icon.sprite = itemUI.IconSprite;
        currentUpgradeIcon = currentIconIndex == -1 ? null : upgradeIcons[currentIconIndex];
        
        if (currentIconIndex == -1)
            foreach (var ui in upgradeIcons)
            {
                ui.SetUpgraded();
            }
    }

    private bool OnUpgrade()
    {
        if (currentUpgradeIcon == null)
            return false;
        
        if (currentUpgradeIcon.Upgrade.Resources.Any(r => 
            !(SceneManager.Instance.Container as BoatStorage)!.Contains(r.ID, r.Count)))
        {
            return false;
        }
        
        foreach (var r in currentUpgradeIcon.Upgrade.Resources)
        {
            _ = (SceneManager.Instance.Container as BoatStorage)!
                .TryTake(sid => sid == r.ID, out _, r.Count);
        }
        
        GameManager.Instance.UpgradeParameters(currentUpgradeIcon.Upgrade);

        currentUpgradeIcon.SetUpgraded();
        currentUpgradeIcon = GetFirstNonUpgradedIcon();
        OnUpgradeCallback?.Invoke();
        
        AudioManager.Instance.PlayOneShot(AudioManager.CRAFTED, true);
        
        return true;
    }
    
    private UpgradeIcon GetFirstNonUpgradedIcon()
    {
        return upgradeIcons.FirstOrDefault(i => !i.IsUpgraded);
    }
}
