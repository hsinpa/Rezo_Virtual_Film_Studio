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
using UnityEngine.Rendering; // .Universal for urp
using UnityEngine.Rendering.Universal;

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
        private Slider contrast_slider;

        [SerializeField]
        private Slider saturate_slider;

        [SerializeField]
        private Button color_adjustment;

        [Header("Bottom")]
        [SerializeField]
        private Button loadBtn;

        private AddressableSceneManagement addressableSceneManagement;
        private SimpleJSON.JSONNode sceneListJson;
        private string cache_key;
        private List<TextMeshProUGUI> scene_selectors = new List<TextMeshProUGUI>();

        private List<string> scene_list = new List<string>();
        public List<string> Scene_List => scene_list;

        private Volume _volume;

        protected override void Start() {
            addressableSceneManagement = new AddressableSceneManagement();
            string add_path = Path.Combine(Application.streamingAssetsPath, Hsinpa.Static.StaticFlag.Path.AddressableList);

            if (File.Exists(add_path)) {
                string json_raw = File.ReadAllText(add_path);
                sceneListJson = SimpleJSON.JSON.Parse(json_raw);
            }

            UtilityFunc.SetSimpleBtnEvent(loadBtn, OnSceneLoadClick);
            fog_intensity_slider.onValueChanged.AddListener(OnFogSliderChange);
            saturate_slider.onValueChanged.AddListener(OnSaturateToggleChange);
            contrast_slider.onValueChanged.AddListener(OnContrastToggleChange);

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

            //Contrast
            _volume = GameObject.FindFirstObjectByType<Volume>();
            if (_volume != null && _volume.profile.TryGet(out ColorAdjustments colorAdjust)) {
                float contrastValue = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.ContrastTogglePref, 0.5f);
                float saturateValue = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.SaturateTogglePref, 0.5f);

                contrast_slider.SetValueWithoutNotify(contrastValue);
                saturate_slider.SetValueWithoutNotify(saturateValue);

                colorAdjust.contrast.value = RescaleVariable(contrastValue, scale: 100);
                colorAdjust.saturation.value = RescaleVariable(saturateValue, scale: 100);
            }
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

        private void OnContrastToggleChange(float p_value) {
            if (_volume != null && _volume.profile.TryGet(out ColorAdjustments colorAdjust)) {
                colorAdjust.contrast.value = RescaleVariable(p_value, scale: 100);
            }

            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.ContrastTogglePref, p_value);
            PlayerPrefs.Save();
        }

        private void OnSaturateToggleChange(float p_value) {
            if (_volume != null && _volume.profile.TryGet(out ColorAdjustments colorAdjust)) {
                colorAdjust.saturation.value = RescaleVariable(p_value, scale: 100); ;
            }

            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.SaturateTogglePref, p_value);
            PlayerPrefs.Save();
        }

        private void OnColorToggleChange(Color color) {
            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.Color_RTogglePref, color.r);
            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.Color_GTogglePref, color.g);
            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.Color_BTogglePref, color.b);

            PlayerPrefs.Save();
        }

        private float RescaleVariable(float value, float scale) {
            return ((value * 2) - 1) * scale;
        }
        #endregion
    }
}
