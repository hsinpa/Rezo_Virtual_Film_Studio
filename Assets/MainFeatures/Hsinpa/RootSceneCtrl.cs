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

        public System.Action SceneActiveEvent;

        void Start() {
            _modals = Modals.instance;

            LoadDefaultScene();
        }

        private async void LoadDefaultScene() {
            await Task.Yield();

            SceneCtrlModal sceneCtrlModal = _modals.GetModal<SceneCtrlModal>();
            sceneCtrlModal.Setup();

            if (sceneCtrlModal.Scene_List != null && sceneCtrlModal.Scene_List.Count > 0) {
               await sceneCtrlModal.LoadScene(sceneCtrlModal.Scene_List[0]);

                if (SceneActiveEvent != null) SceneActiveEvent();
            }
        }
    }
}
