using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public bool isStarted = false;

    private void Start()
    {
        StartCoroutine(ChangeScene("1"));
    }
    public void LoadGame()
    {
        isStarted = true;
    }
    bool temp = false;
    AsyncOperation asyncOperation;
    Scene newScene;
    public IEnumerator ChangeScene(string sceneName)
    {
        if (!temp)
        {
            newScene = SceneManager.GetSceneByName(sceneName);
            asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            temp = true;
        }

        while (!asyncOperation.isDone)
        {
            if (isStarted)
            {
                SceneManager.SetActiveScene(newScene);

            }

            yield return null;
        }
    }

}
