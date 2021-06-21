using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BGController : MonoBehaviour
{
    [SerializeField]
    RectTransform topDoor;
    [SerializeField]
    RectTransform bottomDoor;
    [SerializeField]
    float moveSpeed = 2f;
    [SerializeField]
    float yMaxTopDoor;
    [SerializeField]
    float yMinBottomDoor;
    [SerializeField]
    AudioClip soundOnDoorClosed;
    [SerializeField]
    string soundSourceName = "Sound for doors";

    AudioSource audioSource;

    float minPosTopDoor;
    float maxPosBottomDoor;
    float imageHeight;

    public enum DoorMovingMode
    {
        Open = 1,
        Close = -1
    }
    void Awake()
    {
        Messenger<Action>.AddListener(MyMenuEvents.OPEN_DOORS, StartMoveDoor);
        Messenger<Action>.AddListener(MyMenuEvents.CLOSE_DOORS, CloseDoors);
        Messenger<DoorMovingMode>.AddListener(MyMenuEvents.MOMENT_SWITCH_DOORS, OneMomentChageDoorsStatus);

        imageHeight = topDoor.rect.height * topDoor.localScale.y;

        float middlePos = yMaxTopDoor - (yMaxTopDoor - yMinBottomDoor) / 2f;

        minPosTopDoor = middlePos + imageHeight / 2;
        maxPosBottomDoor = middlePos - imageHeight / 2;
    }
    void Start()
    {
        audioSource = GameObject.Find(soundSourceName).GetComponent<AudioSource>();
        audioSource.ignoreListenerVolume = true;
        DontDestroyOnLoad(audioSource.gameObject);
    }
    
    void OnDestroy()
    {
        Messenger<Action>.RemoveListener(MyMenuEvents.OPEN_DOORS, StartMoveDoor);
        Messenger<Action>.RemoveListener(MyMenuEvents.CLOSE_DOORS, CloseDoors);
        Messenger<DoorMovingMode>.RemoveListener(MyMenuEvents.MOMENT_SWITCH_DOORS, OneMomentChageDoorsStatus);
    }

    void StartMoveDoor(Action d)
    {
        StartCoroutine(DoorMoving(d, DoorMovingMode.Open));
    }

    void CloseDoors(Action d)
    {
        StartCoroutine(DoorMoving(d, DoorMovingMode.Close));
    }

    void OneMomentChageDoorsStatus(DoorMovingMode mode)
    {
        Vector2 tmp;

        if (mode == DoorMovingMode.Close)
        {
            tmp = topDoor.localPosition;
            tmp.y = minPosTopDoor;
            topDoor.localPosition = tmp;

            tmp = bottomDoor.localPosition;
            tmp.y = maxPosBottomDoor;
            bottomDoor.localPosition = tmp;

            BGStatus.status = BGStatus.Status.Closed;
        }
        else
        {
            tmp = topDoor.localPosition;
            tmp.y = yMaxTopDoor;
            topDoor.localPosition = tmp;

            tmp = bottomDoor.localPosition;
            tmp.y = yMinBottomDoor;
            bottomDoor.localPosition = tmp;

            BGStatus.status = BGStatus.Status.Opened;
        }
    }

    IEnumerator DoorMoving(Action d, DoorMovingMode mode)
    {
        while(true)
        {
            if ((mode == DoorMovingMode.Open && topDoor.localPosition.y < yMaxTopDoor) || (mode == DoorMovingMode.Close && topDoor.localPosition.y > minPosTopDoor))
                topDoor.localPosition += (int)mode * Vector3.up * Time.deltaTime * moveSpeed;
            if ((mode == DoorMovingMode.Open && bottomDoor.localPosition.y > yMinBottomDoor) || (mode == DoorMovingMode.Close && bottomDoor.localPosition.y < maxPosBottomDoor ))
                bottomDoor.localPosition += (int)mode * Vector3.down * Time.deltaTime * moveSpeed;


            if (mode == DoorMovingMode.Close)
            {
                if (topDoor.localPosition.y <= minPosTopDoor && bottomDoor.localPosition.y >= maxPosBottomDoor)
                {
                    Vector2 tmp = topDoor.localPosition;
                    tmp.y = minPosTopDoor;
                    topDoor.localPosition = tmp;

                    tmp = bottomDoor.localPosition;
                    tmp.y = maxPosBottomDoor;
                    bottomDoor.localPosition = tmp;

                    BGStatus.status = BGStatus.Status.Closed;

                    if (audioSource != null && soundOnDoorClosed != null)
                    {
                        audioSource.PlayOneShot(soundOnDoorClosed);
                    }
                    break;
                }
            }
            else
            {
                if (topDoor.localPosition.y >= yMaxTopDoor && bottomDoor.localPosition.y <= yMinBottomDoor)
                {
                    Vector2 tmp = topDoor.localPosition;
                    tmp.y = yMaxTopDoor;
                    topDoor.localPosition = tmp;

                    tmp = bottomDoor.localPosition;
                    tmp.y = yMinBottomDoor;
                    bottomDoor.localPosition = tmp;

                    BGStatus.status = BGStatus.Status.Opened;
                    break;
                }
            }

            yield return null;
        }

        if (d != null)
            d.Invoke();
    }
}
