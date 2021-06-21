using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.SmartCoroutines;

public class PlayerCollectorController : MonoBehaviour
{
    [SerializeField]
    private float transitSpeed = 1f;
    [SerializeField]
    private Transform pointOfCollect;

    private Material closestMat;
    private SmartMultipleCoroutine<Object, CollectableObject> moveItemToPlayerRout;

    private void Awake()
    {
        moveItemToPlayerRout = new SmartMultipleCoroutine<Object, CollectableObject>(this);
    }

    public void Take(CollectableObject cObj)
    {
        if (!cObj.TryCollect())
            return;
        
        AudioManager.Instance.PlayOneShot(AudioManager.TAKE);
        
        cObj.IsCollected = true;
        
        PlayerController.Instance.AnimationController.DBAnimationController.SetBool("IsTaking", true);
        moveItemToPlayerRout.Start(cObj, cObj, MoveItemToPlayer);
    }

    IEnumerator MoveItemToPlayer(CollectableObject cObj)
    {
        cObj.SetPhysics(false);

        yield return null;

        Vector3 curStPos = PlayerController.Instance.transform.InverseTransformPoint(cObj.transform.position);
        Vector3 curEndPos = PlayerController.Instance.transform.InverseTransformPoint(pointOfCollect.position);
        float startDist = (curStPos - curEndPos).magnitude;

        while (true)
        {
            cObj.transform.position = PlayerController.Instance.transform.TransformPoint(
                Vector3.Lerp(curStPos, curEndPos, Time.deltaTime * transitSpeed));

            curStPos = PlayerController.Instance.transform.InverseTransformPoint(cObj.transform.position);
            curEndPos = PlayerController.Instance.transform.InverseTransformPoint(pointOfCollect.position);

            cObj.transform.localScale = Vector3.one * (curEndPos - curStPos).magnitude / startDist;

            yield return null;

            if ((curStPos - curEndPos).magnitude <= 1f)
                break;
        }

        cObj.gameObject.SetActive(false);
    }

    public void Throw(CollectableObject item)
    {
        if (moveItemToPlayerRout.IsWorking(item))
            moveItemToPlayerRout.Stop(item);

        item.gameObject.SetActive(true);
        item.SetPhysics(true);

        item.transform.position = pointOfCollect.position;
        item.transform.rotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;

        item.IsCollected = false;
    }
}
