using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.SmartCoroutines;
using System.Linq;
using System.Reflection;
using Random = UnityEngine.Random;

public class DrillableObject : InteractableObject, ISavable
{
    private const float DEEPENING_TIME = 0.3f;

    [SerializeField]
    private Jitter jitter;
    [SerializeField]
    private Stage[] stages;
    [SerializeField]
    private Rigidbody2D piecePrefabRb;
    [SerializeField]
    private float explosionForce;
    [SerializeField]
    private float angle;

    [SerializeField] private GameManager.ItemUI.Condition _condition;
    [SerializeField] private bool _haveCondition;

    private int curStageIndex = 0;
    private Stage curStage;
    private Vector3 deepening;
    private SmartCoroutineCache<System.Action> deepeningCor;
    private Vector3 throwPieceOffset;

    private void Awake()
    {
        curStage = stages[0];
        throwPieceOffset = Vector3.up * jitter.gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
        deepening = throwPieceOffset.y / stages.Length * Vector3.down;
        deepeningCor = new SmartCoroutineCache<System.Action>(this, DeepeningRoutine);
    }

    public void Drill(float dmg)
    {
        jitter.OneJitter();

        DecreaseStat(dmg);
    }

    private void GoToNextStage()
    {
        DropPiece(curStage.GetDropCount());

        if (++curStageIndex == stages.Length)
        {
            Kill();
            return;
        }

        curStage = stages[curStageIndex];
        deepeningCor.Start(null);
    }

    private void DropPiece(int count)
    {
        Rigidbody2D _rb;
        Vector2 _expforce = Vector2.up * explosionForce;

        while(count-- > 0)
        {
            _rb = Instantiate(piecePrefabRb.gameObject, transform.position + throwPieceOffset, 
                Quaternion.identity).GetComponent<Rigidbody2D>();
            _rb.gameObject.AddComponent<SavableObject>().UpdateGuid();
            _expforce = Quaternion.Euler(0, 0, Random.Range(-angle, angle)) * _expforce;
            _rb.AddForce(_expforce);
        }
    }

    private void DecreaseStat(float dmg)
    {
        curStage.Health -= dmg;

        if (curStage.Health <= 0)
        {
            GoToNextStage();
        }
    }

    private void Kill()
    {
        gameObject.layer = 0;
        deepeningCor.Start(() => gameObject.SetActive(false));
    }

    private IEnumerator DeepeningRoutine(System.Action doAtEnd)
    {
        float t = 0;
        Vector3 startPos = jitter.transform.position;
        Vector3 endPos = startPos + deepening;

        while ((t += Time.deltaTime) < DEEPENING_TIME)
        {
            jitter.TargetPosition = Vector3.Lerp(startPos, endPos, t / DEEPENING_TIME);

            jitter.transform.position = jitter.TargetPosition + jitter.Offset;

            yield return null;
        }

        doAtEnd?.Invoke();
    }

    public bool CheckCondition()
    {
        if (!_haveCondition)
            return true;
        
        return _condition.Check();
    }
    
    public void Load(object obj)
    {
        if (obj is null)
        {
            gameObject.layer = 0;
            gameObject.SetActive(false);

            return;
        }

        var drObj = (DrillableObjectSL) obj;
        
        curStageIndex = drObj.CurrentStageIndex;
        curStage = stages[curStageIndex];
        curStage.Health = drObj.CurrentStageHealth;
        jitter.TargetPosition = drObj.JitterTargetPosition;
        jitter.transform.position = jitter.TargetPosition;
    }

    public bool Save(out object obj)
    {
        obj = gameObject.layer == 0 ? null : new DrillableObjectSL(this);

        return true;
    }

    [System.Serializable]
    private struct Stage
    {
        [SerializeField]
        private ChancesAndCounts chancesAndCounts;
        [SerializeField]
        private float health;

        public float Health
        {
            get => health;
            set => health = value;
        }

        public int GetDropCount()
        {
            return chancesAndCounts.GetCount(); 
        }
    }

    [Serializable]
    private class DrillableObjectSL
    {
        public DrillableObjectSL(DrillableObject drillableObject)
        {
            var type = drillableObject.GetType();
            
            _currentStageIndex = (int)type
                .GetField("curStageIndex",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(drillableObject);
            
            var stageObj = type
                .GetField("curStage", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(drillableObject);
            _currentStageHealth = (float)stageObj.GetType().GetField("health",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(stageObj);

            _jitterTargetPosition = ((Jitter) type.GetField("jitter",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(drillableObject)).TargetPosition;
        }

        private int _currentStageIndex;
        private float _currentStageHealth;
        private Vector3 _jitterTargetPosition;

        public int CurrentStageIndex => _currentStageIndex;
        public float CurrentStageHealth => _currentStageHealth;
        public Vector3 JitterTargetPosition => _jitterTargetPosition;
    }
}
