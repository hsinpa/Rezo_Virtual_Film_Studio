using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Hsinpa.Utility
{
    public class ExternalAddressableTool
    {
        private Dictionary<string, GameObject> _cacheObjTable = new Dictionary<string, GameObject>();
        private Dictionary<string, UnityEngine.Object> _cacheUniversalTable = new Dictionary<string, UnityEngine.Object>();


        public async Task<T> AsyncLoadAsset<T>(string id) where T : UnityEngine.Object
        {
            if (_cacheUniversalTable.TryGetValue(id, out UnityEngine.Object material))
            {
                return (T)(material);
            }

            var ophandle = Addressables.LoadAssetAsync<T>(id);
            var result = await ophandle.Task;

            if (ophandle.Status == AsyncOperationStatus.Succeeded)
            {
                _cacheUniversalTable.Add(id, result);

                return result;
            }

            return null;
        }

        public async Task<T> AsyncLoadGameObject<T>(string id) where T : UnityEngine.Object
{
            if (_cacheObjTable.TryGetValue(id, out GameObject gameObject))
            {
                return (gameObject.GetComponent<T>());
            }

            var ophandle = Addressables.LoadAssetAsync<GameObject>(id);
            var result = await ophandle.Task;

            if (ophandle.Status == AsyncOperationStatus.Succeeded)
            {
                return result.GetComponent<T>();
            }

            return null;
        }

        public T LoadGameObject<T>(string id) where T : MonoBehaviour
        {

            if (_cacheObjTable.TryGetValue(id, out GameObject gameObject)) {
                return gameObject.GetComponent<T>();
            }

            var ophandle = Addressables.LoadAssetAsync<GameObject>(id);

            var loadObject = ophandle.WaitForCompletion();

            if (ophandle.Status == AsyncOperationStatus.Succeeded)
            {
                _cacheObjTable.Add(id, loadObject);

                return loadObject.GetComponent<T>();
            }
            else
            {
                Debug.LogError("AssetReference failed to load.");
                return null;
            }
        }

        public void Dispose() {
            ReleaseTableMemory(_cacheObjTable);
            ReleaseTableMemory(_cacheUniversalTable);

            _cacheObjTable.Clear();
            _cacheUniversalTable.Clear();
        }

        private void ReleaseTableMemory<T>(Dictionary<string, T> table) {
            foreach (var keypair in table)
            {
                if (keypair.Value != null)
                    Addressables.Release(keypair.Value);
            }
        }
    }
}