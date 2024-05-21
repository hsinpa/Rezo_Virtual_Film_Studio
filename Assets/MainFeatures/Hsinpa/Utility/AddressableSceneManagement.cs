using Hsinpa.Event;
using Hsinpa.Utility;
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

        private AsyncOperationHandle<SceneInstance> _asyncOpsHandle;

        public async Task<bool> LoadAddScene(string key) {
            if (c_scene_key == key) return false;

            await UnloadPreviousScene();

            c_scene_key = key;

            this._asyncOpsHandle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);
            
            var result = await this._asyncOpsHandle.Task;

            if (this._asyncOpsHandle.Status == AsyncOperationStatus.Succeeded) {
                c_scene_instance = result;

                SceneManager.SetActiveScene(c_scene_instance.Scene);

                SimpleEventSystem.Send(MessageEventFlag.HsinpaEvent.General.SceneStructLoaded);

                return true;
            }

            return false;
        }

        public float GetCompletePercentage()
        {
            if (!_asyncOpsHandle.IsValid()) return 0;
            return _asyncOpsHandle.PercentComplete;
        }

        public async Task UnloadPreviousScene() {
            c_scene_key = "";
            if (c_scene_instance.Scene == null) return;

            var ophandle = Addressables.UnloadSceneAsync(c_scene_instance);
            await ophandle.Task;

            c_scene_instance = default(SceneInstance);
        }
    }
}
