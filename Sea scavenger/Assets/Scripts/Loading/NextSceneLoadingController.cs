using System;
using System.Collections;
using System.Collections.Generic;
using Custom.Patterns;
using Custom.SmartCoroutines;
using UnityEngine;
using SceneManagement = UnityEngine.SceneManagement;

public class NextSceneLoadingController : Singleton<NextSceneLoadingController>
{
    [SerializeField] private int _loadingSceneBuildIndex;

    private int _nextSceneBuildIndex;
    private int _currentSceneBuildIndex;
    private AsyncOperation _loadingAsyncOperation;
    private SmartCoroutineCache _loadingCor;
    //private ArrayList _blockersList;
    private Action _doAfterLoad;
    private AsyncOperation _unloadAsyncOperation;
    private SmartWaitingCoroutine _waitingLoadingScene;

    public int CurrentSceneBuildIndex => _nextSceneBuildIndex;
    public bool AllowUnloadCurrentScene { get; set; } = true;
    public bool AllowLoadNextScene { get; set; } = true;
    public bool AllowUnloadLoadingScene { get; set; } = true;
    public event Action onNextSceneLoaded;
    
    
    protected override void Init()
    {
        _loadingCor = new SmartCoroutineCache(this, LoadingRoutine);
        _waitingLoadingScene = new SmartWaitingCoroutine(this);
        //_blockersList = new ArrayList();
        
        DontDestroyOnLoad(this);
    }

    // public void AddLoadBlocker(object obj)
    // {
    //     _blockersList.Add(obj);
    // }
    //
    // public void RemoveLoadBlocker(object obj)
    // {
    //     _blockersList.Remove(obj);
    // }

    public void LoadScene(int sceneIndex, Action doAfterLoad = null)
    {
        if (_loadingCor.IsWorking)
        {
            if (!_waitingLoadingScene.IsWorking)
            {
                _waitingLoadingScene.StartWhile(() => _loadingCor.IsWorking, methodAfter: () => LoadScene(sceneIndex, doAfterLoad));
            }

            return;
        }
        
        _currentSceneBuildIndex = SceneManagement::SceneManager.GetActiveScene().buildIndex;
        _nextSceneBuildIndex = sceneIndex;
        _doAfterLoad = doAfterLoad;
        
        _loadingCor.Start();
    }

    private IEnumerator LoadingRoutine()
    {
        SceneManagement::SceneManager.LoadScene(
            _loadingSceneBuildIndex, SceneManagement.LoadSceneMode.Additive);

        yield return null;
        
        SceneManagement::SceneManager.SetActiveScene(
            SceneManagement::SceneManager.GetSceneByBuildIndex(_loadingSceneBuildIndex));

        while (!AllowUnloadCurrentScene)
        {
            yield return null;
        }
        
        _unloadAsyncOperation = SceneManagement::SceneManager
            .UnloadSceneAsync(_currentSceneBuildIndex);
        
        while (!_unloadAsyncOperation.isDone)
        {   
            yield return null;
        }
        
        _loadingAsyncOperation = SceneManagement::SceneManager
            .LoadSceneAsync(_nextSceneBuildIndex, SceneManagement.LoadSceneMode.Additive);
        _loadingAsyncOperation.allowSceneActivation = false;
        
        while (!_loadingAsyncOperation.isDone)
        {
            if (!_loadingAsyncOperation.allowSceneActivation && 
                _loadingAsyncOperation.progress >= 0.9f)
            {
                if (AllowLoadNextScene)
                    _loadingAsyncOperation.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        yield return null;
        
        SceneManagement::SceneManager.SetActiveScene(
            SceneManagement::SceneManager.GetSceneByBuildIndex(_nextSceneBuildIndex));

        yield return null;
        
        onNextSceneLoaded?.Invoke();
        _doAfterLoad?.Invoke();

        yield return null;

        while (!AllowUnloadLoadingScene)
        {
            yield return null;
        }
        
        _unloadAsyncOperation = SceneManagement::SceneManager
            .UnloadSceneAsync(_loadingSceneBuildIndex);
        
        while (!_unloadAsyncOperation.isDone)
        {   
            yield return null;
        }
    }
}
