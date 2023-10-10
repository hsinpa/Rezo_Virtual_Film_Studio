using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Hsinpa.Addressable
{
    public class AddressableSceneManagement
    {
        public SceneInstance c_scene_instance;
        public string c_scene_key;

        public async Task<bool> LoadAddScene(string key) {
            if (c_scene_key == key) return false;

            await UnloadPreviousScene();

            c_scene_key = key;

            var ophandle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);
            var result = await ophandle.Task;

            if (ophandle.Status == AsyncOperationStatus.Succeeded) {
                c_scene_instance = result;
                return true;
            }

            return false;
        }

        public async Task<bool> UnloadPreviousScene() {
            c_scene_key = "";
            if (c_scene_instance.Scene == null) return true;

            var ophandle = Addressables.UnloadSceneAsync(c_scene_instance);
            await ophandle.Task;

            c_scene_instance = default(SceneInstance);

            if (ophandle.Status == AsyncOperationStatus.Succeeded) {
                return true;
            }

            return false;
        }

    }
}
