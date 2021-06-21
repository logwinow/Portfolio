using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.SmartCoroutines;
using Custom.Patterns;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField]
    private Vector2 interactionAreaSize;
    [SerializeField]
    private PlayerCollectorController collectorController;
    [SerializeField]
    private PlayerDrillController drillController;
    [SerializeField]
    private Transform checkDistPointTr;

    private InteractableObject interactedObject;
    private Material closestMat;

    public PlayerCollectorController CollectorController => collectorController;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, interactionAreaSize);
    }
#endif

    private void Update()
    {
        Collider2D closestCol = GetClosestInteractableItemCollider2D();
        OutlineCol2D(closestCol);

        if (closestCol != null && Input.GetKeyDown(KeyCode.E))
        {
            var interObj = closestCol.GetComponent<InteractableObject>();

            if (interObj is null)
                return;

            interactedObject = interObj;

            switch (interObj)
            {
                case DrillableObject drObj:
                    if (drillController != null)
                        drillController.StartDrilling(drObj);
                    break;
                case CollectableObject clObj:
                    collectorController.Take(clObj);
                    break;
                case StuffObject stObj:
                    stObj.Open();
                    break;
                case SuitInteractionObject suitOBj:
                    suitOBj.Select();
                    break;
            }
        }

        if (drillController != null && drillController.IsActive &&
            PlayerController.Instance.AnimationController.DBAnimationController.GetBool("IsDrilling"))
        {
            if (!Input.GetKey(KeyCode.E) || closestCol == null || 
                closestCol.gameObject != interactedObject.gameObject)
            {
                drillController.StopDrilling();
            }
        }
    }

    private void OutlineCol2D(Collider2D col)
    {
        if (col == null)
        {
            if (closestMat == null)
                return;

            closestMat.SetInt("_Outline", 0);
            closestMat = null;

            return;
        }

        Material colMat = col.gameObject.GetComponentInChildren<SpriteRenderer>().material;

        if (colMat != closestMat)
        {
            colMat.SetInt("_Outline", 1);
            closestMat?.SetInt("_Outline", 0);
            closestMat = colMat;
        }
    }

    private Collider2D GetClosestInteractableItemCollider2D()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, interactionAreaSize, 0, 1 << 9);

        if (cols.Length == 0)
            return null;

        Collider2D closestCol = cols[0];
        float curDist = DistToCol(closestCol);
        float minDist = curDist;

        for (int i = 1; i < cols.Length; i++)
        {
            curDist = DistToCol(cols[i]);

            if (curDist < minDist)
            {
                closestCol = cols[i];
                minDist = curDist;
            }
        }

        return closestCol;
    }

    private float DistToCol(Collider2D col)
    {
        Vector3 distV = col.transform.position - checkDistPointTr.position;

        return Mathf.Abs(distV.x) + distV.y * distV.y;
    }
}
