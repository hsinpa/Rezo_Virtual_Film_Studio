using Hsinpa.Event;
using Hsinpa.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitySceneManagement : MonoBehaviour
{
    public delegate void OnSceneEventDelegate(string scenename);
    public OnSceneEventDelegate OnSceneLoadEvent;
    private string _scenename;

    public bool SetActiveScene(string scenename)
    {
        Scene selectScene = SceneManager.GetSceneByName(scenename);
        return SceneManager.SetActiveScene(selectScene);
    }


    public async Task<bool> LoadScene(string scenename)
    {
        if (_scenename == scenename) return false;

        if (_scenename != null)
        {
            var unload_async = UnloadSceneProcess(_scenename);
            await UtilityFunc.WaitUntilUIThread(() => unload_async.isDone);
        }

        _scenename = scenename;

        var load_async = LoadSceneProcess(scenename, LoadSceneMode.Additive);

        await UtilityFunc.WaitUntilUIThread(() => load_async.isDone);

        SetActiveScene(scenename);

        SimpleEventSystem.Send(MessageEventFlag.HsinpaEvent.General.SceneStructLoaded);

        return true;
    }

    #region Private
    AsyncOperation UnloadSceneProcess(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(scene);

        StartCoroutine(LoadSceneProcess(asyncLoad));

        _scenename = null;

        return asyncLoad;
    }

    AsyncOperation LoadSceneProcess(string scene, LoadSceneMode loadSceneMode)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, loadSceneMode);
        StartCoroutine(LoadSceneProcess(asyncLoad));
        return asyncLoad;
    }

    IEnumerator LoadSceneProcess(AsyncOperation asyncLoad)
    {
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    #endregion

}
