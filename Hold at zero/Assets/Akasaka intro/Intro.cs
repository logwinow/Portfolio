using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Intro : MonoBehaviour {

	// Use this for initialization
	public int loadingScene = 1;
	void Start () {
        CursorModific(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(loadingScene);
            CursorModific(true);
        }
        if (GetComponent<VideoPlayer>().isPlaying == false && Time.time > 1f)
        {
            SceneManager.LoadScene(loadingScene);
            CursorModific(true);
        }
	}
    void CursorModific(bool enable)
    {
        if (enable == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
