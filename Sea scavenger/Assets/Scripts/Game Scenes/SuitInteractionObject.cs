using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitInteractionObject : InteractableObject
{
    [SerializeField] private SectionID _selectedSuitParameter;
    [SerializeField] private int _selectedSuitNumber;
    [SerializeField] private GameManager.ItemUI.Condition _condition;
    [SerializeField] private bool _haveCondition;

    private void Awake()
    {
        GameSaver.onLoadAfterCheck += OnLoad;
    }

    private void OnDestroy()
    {
        GameSaver.onLoadAfterCheck -= OnLoad;
    }

    public void Select()
    {
        //AudioManager.Instance.PlayOneShot(AudioManager.SUIT_PUT_ON);
        
        GameManager.Instance.GetParameter(_selectedSuitParameter)
            .Set(_selectedSuitNumber);
        GameManager.Instance.GoToScene(2);
    }

    public bool CheckCondition()
    {
        if (!_haveCondition)
            return true;

        return _condition.Check();
    }

    private void OnLoad()
    {
        if (!CheckCondition())
            gameObject.layer = 0;
    }
}
