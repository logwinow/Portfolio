using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    // 989 frame
    [SerializeField] private VideoPlayer _firstPlayer;
    [SerializeField] private VideoPlayer _secondPlayer;
    [SerializeField] private VideoClip _loopClip;
    [SerializeField] private MainMenuController _mainMenu;
    
    private bool _isMenuVisible;

    private void Awake()
    {
        _firstPlayer.loopPointReached += SetSecondPlayer;
    }

    private void OnDestroy()
    {
        _firstPlayer.loopPointReached -= SetSecondPlayer;
    }
    
    private void Start()
    {
        _secondPlayer.clip = _loopClip;
        _secondPlayer.Prepare();
    }

    private void Update()
    {
        if (_isMenuVisible)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _firstPlayer.frame = 989;
        }
    
        if (_firstPlayer.frame >= 989)
        {
            _mainMenu.FadeIn();
            _isMenuVisible = true;
        }
    }

    private void SetSecondPlayer(VideoPlayer videoPlayer)
    {
        _firstPlayer.Stop();
        _firstPlayer.clip = null;

        _secondPlayer.targetCamera = _firstPlayer.targetCamera;
        _secondPlayer.Play();
    }
}
