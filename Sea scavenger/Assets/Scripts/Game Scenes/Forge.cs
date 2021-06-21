using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Custom.SmartCoroutines;
using Random = UnityEngine.Random;

public class Forge : StuffObject, ISavable
{
    [SerializeField]
    private Jitter jitter;
    [SerializeField]
    private BoatStorage storage;
    [SerializeField]
    private Transform dropPointTr;
    [SerializeField]
    private float dropForce;
    [SerializeField]
    private float dropAngle;
    [SerializeField]
    private Sprite opened;
    [SerializeField]
    private Sprite closed;
    [SerializeField]
    private float openedTime;
    [SerializeField] private AudioSource _audioSource;

    private bool isActive = false;
    private List<GameManager.Item> items;
    private SmartCoroutineCache remeltingCor;
    private SmartWaitingCoroutine openedWaiting;
    private SpriteRenderer sprR;

    private void Awake()
    {
        items ??= new List<GameManager.Item>();
        
        sprR = GetComponent<SpriteRenderer>();
        remeltingCor = new SmartCoroutineCache(this, RemeltingRoutine);
        openedWaiting = new SmartWaitingCoroutine(this);
    }

    public override void Close()
    {
        isActive = false;

        jitter.StopJittering();

        remeltingCor.Stop();
        
        _audioSource.Stop();
    }

    public override void Open()
    {
        StartRemelting(true);
    }

    private void StartRemelting(bool needToTakeTrash)
    {
        if (isActive)
        {
            return;
        }
        
        if (needToTakeTrash)
            TakeIronTrashFromStorage();

        if (items.Count == 0)
        {
            return;
        }
        
        isActive = true;

        jitter.StartJittering();

        remeltingCor.Start();

        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsTaking", true);
        
        _audioSource.Play();
    }

    private void TakeIronTrashFromStorage()
    {
        items = new List<GameManager.Item>();

        while(storage.TryTake(i => i.Section1 == 1 && i.Section2 == 0, out var sectionID))
        {
            items.Add(GameManager.Instance.GetItem(sectionID));
        }
    }

    private IEnumerator RemeltingRoutine()
    {
        float t = 0;

        while (items.Count > 0)
        {
            t += Time.deltaTime;

            if (t >= items[0].RemeltTime)
            {
                openedWaiting.Start(openedTime, () => sprR.sprite = opened, () => sprR.sprite = closed);
                DropPieces(items[0]);
                t = 0;
                items.RemoveAt(0);
                
                AudioManager.Instance.PlayOneShot(AudioManager.FORGE_OPENED);
                AudioManager.Instance.PlayOneShot(AudioManager.FORGE_REMELTED);
            }

            yield return null;
        }

        Close();
    }

    private void DropPieces(GameManager.Item item)
    {
        Rigidbody2D _rb;
        Vector2 _expforce = Vector2.up * dropForce;
        int count = item.RemeltedDropChancesAndCounts.GetCount();

        while (count-- > 0)
        {
            _rb = Instantiate(item.RemeltedPrefab.gameObject, dropPointTr.position, 
                Quaternion.identity).GetComponent<Rigidbody2D>();
            _rb.gameObject.AddComponent<SavableObject>().UpdateGuid();
            _expforce = Quaternion.Euler(0, 0, Random.Range(-dropAngle, dropAngle)) 
                        * _expforce;
            _rb.AddForce(_expforce);
        }
    }

    public void Load(object obj)
    {
        var f = obj as ForgeSL;
        items = new List<GameManager.Item>();

        foreach (var i in f!.RemeltingItems)
        {
            items.Add(GameManager.Instance.GetItem(i));
        }
        
        StartRemelting(false);
    }

    public bool Save(out object obj)
    {
        obj = new ForgeSL(this);

        return true;
    }

    [Serializable]
    private class ForgeSL
    {
        public ForgeSL(Forge forge)
        {
            _remeltingItems = new List<SectionID>();

            foreach (var i in (List<GameManager.Item>) typeof(Forge)
                    .GetField("items", 
                        BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(forge))
            {
                _remeltingItems.Add(i.SectionId);
            }
        }

        private List<SectionID> _remeltingItems;

        public List<SectionID> RemeltingItems => _remeltingItems;
    }
}
