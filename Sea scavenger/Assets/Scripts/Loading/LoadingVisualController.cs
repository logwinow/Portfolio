using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingVisualController : MonoBehaviour
{
    private Animator _visualAnimator;
    private void Awake()
    {
        _visualAnimator = GetComponent<Animator>();
        NextSceneLoadingController.Instance.onNextSceneLoaded += FadeOut;
        
        NextSceneLoadingController.Instance.AllowUnloadCurrentScene = false;
        NextSceneLoadingController.Instance.AllowUnloadLoadingScene = false;
    }

    private void OnDestroy()
    {
        NextSceneLoadingController.Instance.onNextSceneLoaded -= FadeOut;
    }

    public void UnloadLoadingScene()
    {
        NextSceneLoadingController.Instance.AllowUnloadLoadingScene = true;
    }

    public void UnloadCurrentScene()
    {    
        NextSceneLoadingController.Instance.AllowUnloadCurrentScene = true;
    }

    private void FadeOut()
    {
        _visualAnimator.SetBool("FadeOut", true);
    }
}
