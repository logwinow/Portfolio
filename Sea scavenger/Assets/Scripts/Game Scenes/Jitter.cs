using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.SmartCoroutines;

public class Jitter : MonoBehaviour
{
    [SerializeField]
    private Vector3 fluctuationMin;
    [SerializeField]
    private Vector3 fluctuationMax;

    private SmartCoroutineCache jitterCor;
    private Vector3 offset = Vector3.zero;
    private Vector3 targetPosition;

    public Vector3 TargetPosition { get; set; }
    public Vector3 Offset => offset;

    private void Awake()
    {
        jitterCor = new SmartCoroutineCache(this, JitterRoutine);
        TargetPosition = transform.position;
    }

    public void StartJittering()
    {
        jitterCor.Start();
    }

    public void StopJittering()
    {
        jitterCor.Stop();
    }

    public void OneJitter()
    {
        offset.x = Random.Range(fluctuationMin.x, fluctuationMax.x);
        offset.y = Random.Range(fluctuationMin.y, fluctuationMax.y);

        transform.position = TargetPosition + offset;
    }

    private IEnumerator JitterRoutine()
    {
        while(true)
        {
            OneJitter();

            yield return null;
        }
    }
}
