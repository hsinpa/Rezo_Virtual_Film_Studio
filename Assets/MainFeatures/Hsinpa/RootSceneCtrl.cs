using Hsinpa.Static;
using Hsinpa.UI;
using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Hsinpa.Ctrl
{
    public class RootSceneCtrl : MonoBehaviour
    {
        private Modals _modals;

        void Awake() {
            _modals = Modals.instance;
        }

        public void ReloadSceneConfig() {
            SceneCtrlModal sceneCtrlModal = _modals.GetModal<SceneCtrlModal>();
            sceneCtrlModal.Setup();
        }

        public async void LoadDefaultScene() {
            await Task.Yield();

            SceneCtrlModal sceneCtrlModal = _modals.GetModal<SceneCtrlModal>();
            sceneCtrlModal.Setup();

            if (sceneCtrlModal.Scene_List != null && sceneCtrlModal.Scene_List.Count > 0) {

                string previous_scene = PlayerPrefs.GetString(StaticFlag.PlayerPref.LoadScenePref, "");

                if (previous_scene == "")
                    previous_scene = sceneCtrlModal.Scene_List[0];

                await sceneCtrlModal.LoadScene(previous_scene);
            }
        }
    }
}
