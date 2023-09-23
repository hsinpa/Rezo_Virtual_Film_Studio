using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Hsinpa.Addressable;
using System.IO;
using UnityEditor.SearchService;
using Hsinpa.Utility;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace Hsinpa.UI
{
    public class SceneCtrlModal : Modal
    {
        [SerializeField]
        private TextMeshProUGUI scene_selector_prefab;

        [SerializeField]
        private Transform scene_selector_container;

        [SerializeField]
        private Button loadBtn;

        private AddressableSceneManagement addressableSceneManagement;
        private SimpleJSON.JSONNode sceneListJson;
        private string cache_key;
        private List<TextMeshProUGUI> scene_selectors = new List<TextMeshProUGUI>();

        private List<string> scene_list = new List<string>();
        public List<string> Scene_List => scene_list;


        protected override void Start() {
            addressableSceneManagement = new AddressableSceneManagement();
            string add_path = Path.Combine(Application.streamingAssetsPath, Hsinpa.Static.StaticFlag.Path.AddressableList);

            if (File.Exists(add_path)) {
                string json_raw = File.ReadAllText(add_path);
                sceneListJson = SimpleJSON.JSON.Parse(json_raw);
            }

            UtilityFunc.SetSimpleBtnEvent(loadBtn, OnSceneLoadClick);
        }

        public void Setup() {
            scene_list.Clear();
            scene_selectors.Clear();

            var scene_array = sceneListJson["scene"].AsArray;

            foreach (var scene_j in scene_array) {
                string add_key = scene_j.ToString().Replace("\"", "");

                var ui_text = UtilityFunc.CreateObjectToParent<TextMeshProUGUI>(scene_selector_container, scene_selector_prefab.gameObject);
                var ui_button = ui_text.GetComponent<Button>();

                scene_selectors.Add(ui_text);
                scene_list.Add(add_key);

                UtilityFunc.SetSimpleBtnEvent(ui_button, () => {

                    scene_selectors.ForEach(x => {
                        x.color = Color.white;
                    });
                    ui_text.color = Color.magenta;

                    cache_key = add_key;
                });
            }
        }

        public async Task<bool> LoadScene(string addressable_key) {
            return await addressableSceneManagement.LoadAddScene(addressable_key);
        }

        private async void OnSceneLoadClick() {
            if (string.IsNullOrEmpty(cache_key)) return;

            bool result =  await LoadScene(cache_key);

            if (result) Modals.instance.Close();
        }
    }
}
