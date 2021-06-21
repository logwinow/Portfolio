using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField]
    Image signImage;
    [SerializeField]
    List<Sprite> signSprites;
    [SerializeField]
    Image eyes;
    AsyncOperation asyncOperation;

    private void Start()
    {
        asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(NextScene.Index);
        asyncOperation.allowSceneActivation = false;

        StartCoroutine(SceneLoading());
    }

    IEnumerator SceneLoading()
    {
        int lastSprite = -1;

        while(signImage.sprite != signSprites[signSprites.Count - 1] || asyncOperation.progress < .8f)
        {
            if ((int)Mathf.Floor(asyncOperation.progress / (1f / signSprites.Count)) > lastSprite)
            {
                signImage.sprite = signSprites[++lastSprite];
                yield return new WaitForSeconds(.15f);
            }
            else
                yield return null;
        }

        eyes.gameObject.SetActive(true);

        yield return new WaitForSeconds(.5f);

        asyncOperation.allowSceneActivation = true;
    }
}
