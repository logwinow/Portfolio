using System.Collections;
using System.Collections.Generic;
using Custom.Patterns;
using UnityEngine;
using  Custom.SmartCoroutines;

public class SceneManager : Singleton<SceneManager>
{
    [SerializeField] private ItemsContainer _container;
    [SerializeField] private PauseMenuController _pauseMenuController;

    private ArrayList _openedSubmenuList;
    private SmartWaitingCoroutine _waitingForRemove;

    public ItemsContainer Container => _container;
    public PauseMenuController PauseMenuController => _pauseMenuController;
    public bool IsOpenedAnySubmenu => _openedSubmenuList.Count > 0;
    
    protected override void Init()
    {
        _openedSubmenuList = new ArrayList();
        _waitingForRemove = new SmartWaitingCoroutine(this);
    }

    public void AddActiveSubmenu(object submenu)
    {
        _openedSubmenuList.Add(submenu);
    }

    public void RemoveActiveSubmenu(object submenu)
    {
        // костыль
        
        _waitingForRemove.Start(0, methodAfter: () => _openedSubmenuList.Remove(submenu));
    }
}
