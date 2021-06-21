using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class ManualOverload : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    Manual manual;
    OrdinaryCoroutine<MoveDirection, Action> movingCor;
    [SerializeField]
    float leftEdgePosX;
    float rightEdgePosX;
    [SerializeField]
    float speed = 4f;
    [SerializeField]
    Image icon;
    [SerializeField]
    Image field;
    [SerializeField]
    float fieldDefaultAlpha = .5f;

    Vector3 startFieldGlobalPos;

    public bool mouseUnderManual;

    enum MoveDirection
    {
        Left = -1,
        Right = 1
    }

    void Awake()
    {
        manual = GetComponent<Manual>();
        movingCor = new OrdinaryCoroutine<MoveDirection, Action>(this, Moving);
        rightEdgePosX = transform.localPosition.x;
        startFieldGlobalPos = field.transform.position;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            manual.Next();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            manual.Back();
        }
    }

    void LateUpdate()
    {
        field.transform.position = startFieldGlobalPos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        icon.color -= new Color(0, 0, 0, 1f);
        field.color -= new Color(0, 0, 0, 1f);
        if (movingCor.FirstArgumentValue != MoveDirection.Left && transform.localPosition.x > leftEdgePosX)
            movingCor.Start(MoveDirection.Left, null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (movingCor.FirstArgumentValue != MoveDirection.Right && transform.localPosition.x < rightEdgePosX)
            movingCor.Start(MoveDirection.Right, () =>
            {
                Color tmp = icon.color;
                tmp.a = 1f;
                icon.color = tmp;

                tmp = field.color;
                tmp.a = fieldDefaultAlpha;
                field.color = tmp;
            }
            );

        
    }

    IEnumerator Moving(MoveDirection dir, Action callback)
    {
        while(true)
        {
            transform.localPosition += (int)dir * Vector3.right * Time.deltaTime * speed;

            if (dir == MoveDirection.Left)
            {
                if (transform.localPosition.x <= leftEdgePosX)
                {
                    Vector3 tmp = transform.localPosition;
                    tmp.x = leftEdgePosX;
                    transform.localPosition = tmp;

                    break;
                }
            }
            else
            {
                if (transform.localPosition.x >= rightEdgePosX)
                {
                    Vector3 tmp = transform.localPosition;
                    tmp.x = rightEdgePosX;
                    transform.localPosition = tmp;

                    break;
                }
            }

            yield return null;
        }

        if (callback != null)
            callback.Invoke();
    }
}
