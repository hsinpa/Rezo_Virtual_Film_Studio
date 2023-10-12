using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Hsinpa.Addressable;
using System.IO;
using Hsinpa.Utility;
using UnityEngine.UI;
using System.Threading.Tasks;
using Hsinpa.Static;

namespace Hsinpa.UI
{
    public class SceneCtrlModal : Modal
    {
        [Header("Scene Selector")]
        [SerializeField]
        private TextMeshProUGUI scene_selector_prefab;

        [SerializeField]
        private Transform scene_selector_container;

        [Header("Other configuration")]
        [SerializeField]
        private Toggle day_night_toggle;

        [SerializeField]
        private Slider fog_intensity_slider;

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
            fog_intensity_slider.onValueChanged.AddListener(OnFogSliderChange);
            day_night_toggle.onValueChanged.AddListener(OnDayNightToggleChange);
        }

        public override void Show(bool isShow)
        {
            base.Show(isShow);

            if (isShow) Setup();
        }

        public void Setup() {
            SetSceneListUI();
            SetOtherConfiguration();
        }

        private void SetSceneListUI() {
            UtilityFunc.ClearChildObject(scene_selector_container);
            scene_list.Clear();
            scene_selectors.Clear();

            var scene_array = sceneListJson["scene"].AsArray;

            foreach (var scene_j in scene_array) {
                string add_key = scene_j.ToString().Replace("\"", "");

                var ui_text = UtilityFunc.CreateObjectToParent<TextMeshProUGUI>(scene_selector_container, scene_selector_prefab.gameObject);
                var ui_button = ui_text.GetComponent<Button>();

                Debug.Log("add_key " + add_key);
                ui_text.color = (add_key == cache_key) ? Color.magenta : Color.white;

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

        private void SetOtherConfiguration() {
            // Day Night Toggle
            bool day_night_bool = PlayerPrefs.GetInt(StaticFlag.PlayerPref.DayNightTogglePref, 0) == 0;
            day_night_toggle.isOn = (day_night_bool);

            // Fog intensity
            float fog_intensity_normalize = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.FogIntensityPref, 0);
            fog_intensity_slider.SetValueWithoutNotify(fog_intensity_normalize);
            float intensity = (Mathf.Lerp(StaticFlag.Config.FOG_INTENSITY_MIN, StaticFlag.Config.FOG_INTENSITY_MAX, fog_intensity_normalize));
            RenderSettings.fogDensity = intensity;
        }

        public async Task<bool> LoadScene(string addressable_key) {
            cache_key = addressable_key;

            Debug.Log("addressable_key " + addressable_key);

            bool load_sucess = await addressableSceneManagement.LoadAddScene(addressable_key);

            //Skybox and Fog
            SetOtherConfiguration();

            return load_sucess;
        }

        #region UI Event
        private async void OnSceneLoadClick() {
            if (string.IsNullOrEmpty(cache_key)) return;

            bool result =  await LoadScene(cache_key);

            if (result) Modals.instance.Close();
        }

        private void OnFogSliderChange(float p_value) {
            float intensity = (Mathf.Lerp(StaticFlag.Config.FOG_INTENSITY_MIN, StaticFlag.Config.FOG_INTENSITY_MAX, p_value));
            RenderSettings.fogDensity = intensity;

            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.FogIntensityPref, p_value);
            PlayerPrefs.Save();
        }

        private void OnDayNightToggleChange(bool p_value) {
            PlayerPrefs.SetInt(StaticFlag.PlayerPref.DayNightTogglePref, p_value ? 1 : 0);
            PlayerPrefs.Save();
        }
        #endregion
    }
}
