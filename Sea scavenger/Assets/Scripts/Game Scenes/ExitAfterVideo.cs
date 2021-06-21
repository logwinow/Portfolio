using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ExitAfterVideo : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += Exit;

    }

    private void OnDestroy()
    {
        _videoPlayer.loopPointReached -= Exit;
    }

    private void Exit(VideoPlayer _player)
    {
        GameManager.Instance.GoToScene(0, false, false);
    }
}
