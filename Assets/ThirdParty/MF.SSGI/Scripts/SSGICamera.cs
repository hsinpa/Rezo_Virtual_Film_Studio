using UnityEngine;
using UnityEngine.SceneManagement;

namespace MF.SSGI {
    [RequireComponent(typeof(Camera))]
    public class SSGICamera : MonoBehaviour {

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "scene@forest")
            {
                this.enabled = true;

                return;
            }

            this.enabled = false;
        }

        //Makes sure the toggle is visible in the Inspector
        private void OnEnable() { }

        void OnDisable()
        {
            Debug.Log("OnDisable");
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}